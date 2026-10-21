using MiniDesk.Models;

namespace MiniDesk.ViewModels
{
    public class TicketListViewModel
    {
        public List<Ticket> Tickets { get; set; } = new();
        public string? Keyword { get; set; }
        public TicketStatus? Status { get; set; }
        public TicketPriority? Priority { get; set; }
        public string? Sort { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
