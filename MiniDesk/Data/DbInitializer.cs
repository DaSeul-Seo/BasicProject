using MiniDesk.Models;

namespace MiniDesk.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext db)
        {
            if (db.Users.Any())
            {
                return;
            }

            var admin = new User
            {
                Email = "admin@minidesk.com",
                Password = BCrypt.Net.BCrypt.HashPassword("poi098)(*"),
                Name = "관리자",
                Role = UserRole.Admin,
            };

            var user = new User
            {
                Email = "user@minidesk.com",
                Password = BCrypt.Net.BCrypt.HashPassword("poi098)(*"),
                Name = "김개발",
                Role = UserRole.User,
            };

            db.Users.AddRange(admin, user);
            db.SaveChanges();

            var tickets = new List<Ticket>
            {
                new()
                {
                    Title = "API 오류",
                    Content = "API 호출 시 500 오류가 발생합니다.",
                    Category = "시스템",
                    Priority = TicketPriority.High,
                    Status = TicketStatus.InProgress,
                    RequesterId = user.Id,
                    AssigneeId = admin.Id
                },
                new()
                {
                    Title = "사용자 권한 요청",
                    Content = "SharePoint 사이트 접근 권한을 요청합니다.",
                    Category = "권한",
                    Priority = TicketPriority.Normal,
                    Status = TicketStatus.Pending,
                    RequesterId = user.Id,
                },
                new()
                {
                    Title = "URL 변경 요청",
                    Content = "기존 URL을 새로운 주소로 변경해주세요.",
                    Category = "기타",
                    Priority = TicketPriority.Low,
                    Status = TicketStatus.Completed,
                    RequesterId = user.Id,
                    AssigneeId = admin.Id
                }
            };

            db.Tickets.AddRange(tickets);
            db.SaveChanges();
        }
    }
}
