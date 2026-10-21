using Microsoft.EntityFrameworkCore;
using MiniDesk.Data;
using MiniDesk.Models;

namespace MiniDesk.Services
{
    public class NotificationService
    {
        private readonly AppDbContext _db;

        public NotificationService(AppDbContext db)
        {
            _db = db;
        }

        // 알림 생성
        public async Task CreateAsync(int userId, string title, string message, int? ticketId = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                TicketId = ticketId,
                Title = title,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync();
        }

        // 읽지 않은 알림 개수
        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _db.Notifications
                .CountAsync(x => x.UserId == userId && !x.IsRead);
        }

        // 최근 알림
        public async Task<List<Notification>> GetRecentAsync(int userId, int count = 10)
        {
            return await _db.Notifications
                .Include(x => x.Ticket)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        // 알림 읽음 처리
        public async Task MarkAsReadAsync(int notificationId, int userId)
        {
            var notification = await _db.Notifications
                .FirstOrDefaultAsync(x => x.Id == notificationId && x.UserId == userId); // 다른 사용자의 알림 ID를 알아내도 읽음 처리할 수 없음

            if (notification == null)
            {
                return;
            }

            notification.IsRead = true;
            await _db.SaveChangesAsync();
        }

        public async Task<int?> MarkAsReadAndGetTicketIdAsync(int notificationId, int userId)
        {
            var notification = await _db.Notifications.FirstOrDefaultAsync(x => x.Id == notificationId && x.UserId == userId);

            if (notification == null)
            {
                return null;
            }

            notification.IsRead = true;
            await _db.SaveChangesAsync();
            return notification.TicketId;
        }
    }
}
