using Microsoft.Extensions.Primitives;

namespace MiniDesk.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public TicketStatus Status { get; set; }
        public TicketPriority Priority { get; set; }
        public string Category { get; set; } = string.Empty;
        public int RequesterId { get; set; }
        public User Requester { get; set; } = null!;
        // 처음 요처어 등록시 담당자가 없을 수 있으니까 nullable
        public int? AssigneeId { get; set; }
        public User? Assignee { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<TicketHistory> Histories { get; set; } = new List<TicketHistory>();
        public ICollection<TicketAttachment> Attachments { get; set; } = new List<TicketAttachment>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
