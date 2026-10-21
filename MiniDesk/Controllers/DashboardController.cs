using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniDesk.Data;
using MiniDesk.Models;
using MiniDesk.ViewModels;
using System.Numerics;

namespace MiniDesk.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _db;

        public DashboardController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel
            {
                Total = await _db.Tickets.CountAsync(),
                Pending = await _db.Tickets.CountAsync(x => x.Status == TicketStatus.Pending),
                InProgress = await _db.Tickets.CountAsync(x => x.Status == TicketStatus.InProgress),
                Completed = await _db.Tickets.CountAsync(x => x.Status == TicketStatus.Completed),
                Critical = await _db.Tickets.CountAsync(x => x.Priority == TicketPriority.Critical),
                RecentTickets = await _db.Tickets
                    .Include(x => x.Requester)
                    .Include(x => x.Assignee)
                    .OrderByDescending(x => x.CreatedAt)
                    .Take(5)
                    .ToListAsync()
            };

            return View(model);
        }
    }
}
