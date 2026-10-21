using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniDesk.Dtos;
using MiniDesk.Models;
using MiniDesk.Services;
using System.Security.Claims;

namespace MiniDesk.Controllers.Api
{
    [ApiController]
    [Route("api/tickets")] // 기본 url
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly TicketService _ticketService;
        private readonly NotificationService _notificationService;

        public TicketsController(TicketService ticketService, NotificationService notificationService)
        {
            _ticketService = ticketService;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTickets(string? keyword, TicketStatus? status, TicketPriority? priority, string? sort, int page = 1)
        {
            const int pageSize = 10;

            if (page < 1)
            {
                page = 1;
            }

            var result = await _ticketService.GetTicketsAsync(keyword, status, priority, sort, page, pageSize);

            var items = result.Tickets.Select(x => new TicketListResponseDto
            {
                Id = x.Id,
                Title = x.Title,
                Status = x.Status,
                Priority = x.Priority,
                RequesterName = x.Requester?.Name ?? "",
                AssigneeName = x.Assignee?.Name,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return Ok(new
            {
                items,
                totalCount = result.TotalCount,
                page,
                pageSize
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTicket(int id)
        {
            var ticket = await _ticketService.GetTicketAsync(id);

            if (ticket == null)
            {
                return NotFound(new
                {
                    message = "Ticket을 찾을 수 없습니다."
                });
            }

            var result = new TicketDetailResponseDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Content = ticket.Content,
                Category = ticket.Category,
                Status = ticket.Status,
                Priority = ticket.Priority,
                RequesterName = ticket.Requester?.Name ?? "",
                AssigneeName= ticket.Assignee?.Name,
                CreatedAt= ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt,
                Comments = ticket.Comments
                    .OrderBy(x => x.CreatedAt)
                    .Select(x => new CommentDto
                    {
                        Id= x.Id,
                        UserName = x.User?.Name ?? "",
                        Content = x.Content,
                        CreatedAt = x.CreatedAt
                    }).ToList(),
                Histories = ticket.Histories
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => new HistoryDto
                    {
                        Id = x.Id,
                        UserName = x.User?.Name ?? "",
                        Action = x.Action,
                        BeforeValue = x.BeforeValue,
                        AfterValue = x.AfterValue,
                        CreatedAt = x.CreatedAt
                    }).ToList(),
                Attachments = ticket.Attachments
                    .Select(x => new AttachmentDto
                    {
                        Id = x.Id,
                        FileName = x.FileName,
                        ContentType = x.ContentType,
                        FileSize = x.FileSize,
                        CreatedAt = x.CreatedAt
                    }).ToList()
            };

            return Ok(result);
        }

        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, TicketStatusUpdateRequestDto request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            try
            {
                await _ticketService.ChangeStatusAsync(id, request.Status, userId);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new
                {
                    message = "Ticket을 찾을 수 없습니다."
                });
            }

            return Ok(new
            {
                message = "상태가 변경되었습니다.",
                ticketId = id,
                status = request.Status
            });
        }

        [HttpPut("{id:int}/assignee")]
        public async Task<IActionResult> UpdateAssignee(int id, TicketAssigneeUpdateRequestDto request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // 변경 전 Ticket조회
            var ticket = await _ticketService.GetTicketAsync(id);

            if (ticket == null)
            {
                return NotFound(new
                {
                    message = "Ticket을 찾을 수 없습니다."
                });
            }

            // 변경 전 담당자
            var beforeAssigneeId = ticket.AssigneeId;

            try
            {
                // 담당자 변경
                await _ticketService.ChangeAssigneeAsync(id, request.AssigneeId, userId);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }

            // 새로운 담당자가 지정되었고, 실제 담당자가 변경된 경우 알림 생성
            if (request.AssigneeId.HasValue && beforeAssigneeId != request.AssigneeId)
            {
                await _notificationService.CreateAsync(
                    request.AssigneeId.Value,
                    "새로운 요청이 배정되었습니다.",
                    $"[{ticket.Title}] 요청의 담당자로 지정되었습니다.",
                    id);
            }

            return Ok(new
            {
                message = "담당자가 변경되었습니다.",
                ticketid = id,
                assigneeId = request.AssigneeId
            });
        }

        [HttpPost("{id:int}/comments")]
        public async Task<IActionResult> AddComment(int id, CommentCreateRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest(new
                {
                    message = "댓글 내용을 입력해 주세요."
                });
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            try
            {
                var comment = await _ticketService.AddCommentAsync(id, request.Content, userId);

                return Ok(new
                {
                    id = comment.Id,
                    ticketId = comment.TicketId,
                    userId = comment.UserId,
                    content = comment.Content,
                    createdAt = comment.CreatedAt,
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new
                {
                    message = "Ticket을 찾을 수 없습니다."
                });
            }
        }

    }
}
