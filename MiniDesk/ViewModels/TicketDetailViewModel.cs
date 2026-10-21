using MiniDesk.Models;

namespace MiniDesk.ViewModels
{
    public class TicketDetailViewModel
    {
        public Ticket Ticket { get; set; } = null!;

        public List<User> Users { get; set; } = new();

        public List<Comment> Comments { get; set; } = new();

        public List<TicketHistory> Histories { get; set; } = new();
    }
}
