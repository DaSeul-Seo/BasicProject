using Microsoft.EntityFrameworkCore;
using MiniDesk.Models;

namespace MiniDesk.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<TicketHistory> TicketHistories => Set<TicketHistory>();
        public DbSet<TicketAttachment> TicketAttachments => Set<TicketAttachment>();
        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User 1 - N Ticket Requester
            modelBuilder.Entity<Ticket>()
                .HasOne(x => x.Requester)
                .WithMany(x => x.RequestedTickets)
                .HasForeignKey(x => x.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // User 1 - N Ticket Assignee
            modelBuilder.Entity<Ticket>()
                .HasOne(x => x.Assignee)
                .WithMany(x => x.AssignedTickets)
                .HasForeignKey(x => x.AssigneeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
