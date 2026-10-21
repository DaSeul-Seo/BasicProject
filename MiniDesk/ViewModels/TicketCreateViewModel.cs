using MiniDesk.Models;
using System.ComponentModel.DataAnnotations;

namespace MiniDesk.ViewModels
{
    public class TicketCreateViewModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;

        public TicketPriority Priority { get; set; }

    }
}
