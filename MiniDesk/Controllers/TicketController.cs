using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniDesk.Data;
using MiniDesk.Models;
using MiniDesk.Services;
using MiniDesk.ViewModels;
using System.Security.Claims;

namespace MiniDesk.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _environment;
        private readonly TicketService _ticketService;
        private readonly TicketAttachmentService _ticketAttachmentService;
        private readonly UserService _userService;
        private readonly NotificationService _notificationService;

        public TicketController(AppDbContext db, 
            IWebHostEnvironment environment, 
            TicketService ticketService, 
            UserService userService, 
            TicketAttachmentService ticketAttachmentService, 
            NotificationService notificationService)
        {
            _db = db;
            _environment = environment;
            _ticketService = ticketService;
            _userService = userService;
            _ticketAttachmentService = ticketAttachmentService;
            _notificationService = notificationService;
        }

        // 목록
        public async Task<IActionResult> Index(
            string? keyword, 
            TicketStatus? status, 
            TicketPriority? priority,
            string? sort,
            int page = 1)
        {
            const int pageSize = 10;

            if (page < 1)
            {
                page = 1;
            }

            var result = await _ticketService.GetTicketsAsync(keyword, status, priority, sort, page, pageSize);

            var model = new TicketListViewModel
            {
                Tickets = result.Tickets,
                TotalCount = result.TotalCount,
                Keyword = keyword,
                Status = status,
                Priority = priority,
                Sort = sort,
                Page = page,
                PageSize = pageSize
            };

            return View(model);
        }

        // 등록
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TicketCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var ticket = await _ticketService.CreateAsync(
                model.Title,
                model.Content,
                model.Category,
                model.Priority,
                userId);

            return RedirectToAction(nameof(Detail), new { id = ticket.Id });
        }

        // 상세
        public async Task<IActionResult> Detail(int id)
        {
            var ticket = await _ticketService.GetTicketAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            var users = await _userService.GetUsersAsync();

            var model = new TicketDetailViewModel
            {
                Ticket = ticket,
                Users = users,
                Comments = ticket.Comments
                    .OrderBy(x => x.CreatedAt)
                    .ToList(),
                Histories = ticket.Histories
                    .OrderByDescending(x => x.CreatedAt)
                    .ToList()
            };

            return View(model);
        }

        // 상태변경
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, TicketStatus status)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            try
            {
                await _ticketService.ChangeStatusAsync(id, status, userId);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Detail), new { id });
        }

        // 댓글
        [HttpPost]
        public async Task<IActionResult> AddComment(int id, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return RedirectToAction(nameof(Detail), new { id });
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            try
            {
                await _ticketService.AddCommentAsync(id, content, userId);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Detail), new { id });
        }

        // 담당자 변경
        [HttpPost]
        public async Task<IActionResult> ChangeAssignee(int id, int? assigneeId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // 변경 전 Ticket 조회
            var ticket = await _ticketService.GetTicketAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            // 변경 전 담당자
            var beforeAssigneeId = ticket.AssigneeId;

            // 담당자 변경
            await _ticketService.ChangeAssigneeAsync(id, assigneeId, userId);

            // 새로운 담당자가 지정된 경우
            // 실제 담당자가 변견된 경우에만 알림
            if (assigneeId.HasValue && beforeAssigneeId != assigneeId)
            {
                await _notificationService.CreateAsync(
                        assigneeId.Value,
                        "새로운 요청이 배정되었습니다.",
                        $"[{ticket.Title}] 요청의 담당자로 지정되었습니다.",
                        id);
            }

            return RedirectToAction(nameof(Detail), new { id });
        }


        // 파일 업로드
        [HttpPost]
        public async Task<IActionResult> UploadAttachment(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return RedirectToAction(nameof(Detail), new { id });
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            try
            {
                await _ticketAttachmentService.UploadAsync(id, file, userId);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Detail), new { id });
        }

        // 파일 다운로드
        [HttpGet]
        public async Task<IActionResult> DownloadAttachment(int id)
        {
            var attachment = await _db.TicketAttachments.FirstOrDefaultAsync(x => x.Id == id);

            if (attachment == null)
            {
                return NotFound();
            }

            var physicalPath = Path.Combine(_environment.WebRootPath, attachment.FilePath);

            if (!System.IO.File.Exists(physicalPath))
            {
                return NotFound();
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(physicalPath);

            return File(bytes, attachment.ContentType, attachment.FileName);
        }

        // 파일 삭제
        [HttpPost]
        public async Task<IActionResult> DeleteAttachment(int id)
        {
            var ticketId = await _ticketAttachmentService.GetTicketIdAsync(id);

            if (!ticketId.HasValue)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            try
            {
                await _ticketAttachmentService.DeleteAsync(id, userId);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Detail), new { id = ticketId.Value });
        }
    }
}
