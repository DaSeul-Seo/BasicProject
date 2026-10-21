namespace MiniDesk.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; } = null!;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string Action { get; set; } = string.Empty;
        public string? BeforeValue { get; set; }
        public string? AfterValue { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
