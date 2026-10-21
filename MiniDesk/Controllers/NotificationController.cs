using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using MiniDesk.Services;
using System.Security.Claims;

namespace MiniDesk.Controllers
{
    public class NotificationController: Controller
    {
        private readonly NotificationService _notificationService;

        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // 알림목록
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var notifications = await _notificationService.GetRecentAsync(userId);

            return View(notifications);
        }

        // 알림 읽음 처리
        public async Task<IActionResult> Read(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _notificationService.MarkAsReadAsync(id, userId);

            //if (ticketId.HasValue)
            //{
            //    return RedirectToAction("Detail", "Ticket", new { id = ticketId.Value });
            //}

            // await _notificationService.MarkAsReadAsync(id, userId);

            return RedirectToAction(nameof(Index));
        }
    }
}
