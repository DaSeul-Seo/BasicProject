using MiniDesk.Models;

namespace MiniDesk.ViewModels
{
    public class DashboardViewModel
    {
        public int Total { get; set; }
        public int Pending { get; set; }
        public int InProgress { get; set; }
        public int Completed { get; set; }
        public int Critical { get; set; }
        public List<Ticket> RecentTickets { get; set; } = new();
    }
}
