using Microsoft.EntityFrameworkCore;
using MiniDesk.Data;
using MiniDesk.Models;
using System.Runtime.CompilerServices;

namespace MiniDesk.Services
{
    public class TicketService
    {
        private readonly AppDbContext _db;

        public TicketService(AppDbContext db)
        {
            _db = db;
        }
        
        // 목록
        public async Task<(List<Ticket> Tickets, int TotalCount)> GetTicketsAsync(
            string? keyword, 
            TicketStatus? status, 
            TicketPriority? priority,
            string? sort,
            int page,
            int pageSize)
        {
            var query = _db.Tickets
                .Include(x => x.Requester)
                .Include(x => x.Assignee)
                .AsQueryable();

            // 검색
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x => x.Title.Contains(keyword) || x.Content.Contains(keyword));
            }

            // 상태
            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }
            
            // 우선순위
            if (priority.HasValue)
            {
                query = query.Where(x => x.Priority == priority.Value);
            }

            // 전체 개수
            var totalCount = await query.CountAsync();

            // 정렬
            query = sort switch
            {
                "created_asc" => query.OrderBy(x => x.CreatedAt),
                "priority_desc" => query.OrderByDescending(x => x.Priority),
                "priority_asc" => query.OrderBy(x => x.Priority),
                "title_asc" => query.OrderBy(x => x.Title),
                _ => query.OrderByDescending(x => x.CreatedAt)
            };

            // 페이징
            var tickets = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (tickets, totalCount);
        }

        // Detail 조회
        public async Task<Ticket?> GetTicketAsync(int id)
        {
            return await _db.Tickets
                .Include(x => x.Requester)
                .Include(x => x.Assignee)
                .Include(x => x.Comments)
                    .ThenInclude(x => x.User)
                .Include(x => x.Histories)
                    .ThenInclude(x => x.User)
                .Include(x => x.Attachments)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        // 생성
        public async Task<Ticket> CreateAsync(string title, string content, string category, TicketPriority priority, int requesterId)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                // Ticket 생성
                var ticket = new Ticket
                {
                    Title = title,
                    Content = content,
                    Category = category,
                    Priority = priority,
                    Status = TicketStatus.Pending,
                    RequesterId = requesterId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _db.Tickets.Add(ticket);

                // 먼저 Ticket ID 생성
                await _db.SaveChangesAsync();

                // history
                _db.TicketHistories.Add(new TicketHistory
                {
                    TicketId = ticket.Id,
                    UserId = requesterId,
                    Action = "요청 등록",
                    AfterValue = "Pending"
                });

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                return ticket;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // 담담자 변경
        public async Task ChangeAssigneeAsync(int ticketId, int? assigneeId, int changedBy)
        {
            // Ticket 업데이트는 성공했는데 History는 실패하는 경우 데이터가 꼬여서
            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var ticket = await _db.Tickets.FirstOrDefaultAsync(x => x.Id == ticketId);

                if (ticket == null)
                {
                    throw new KeyNotFoundException("Ticket을 찾을 수 없습니다.");
                }

                var beforeUser = ticket.AssigneeId.HasValue ? await _db.Users.FindAsync(ticket.AssigneeId.Value) : null;

                User? afterUser = null;

                if (assigneeId.HasValue)
                {
                    afterUser = await _db.Users.FindAsync(assigneeId.Value);
                    
                    if (afterUser == null)
                    {
                        throw new KeyNotFoundException("담당자를 찾을 수 없습니다.");
                    }
                }

                ticket.AssigneeId = assigneeId;
                ticket.UpdatedAt = DateTime.Now;

                _db.TicketHistories.Add(new TicketHistory
                {
                    TicketId = ticketId,
                    UserId = changedBy,
                    Action = "담당자 변경",
                    BeforeValue = beforeUser?.Name ?? "미지정",
                    AfterValue = afterUser?.Name ?? "미지정"
                });

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // 상태 변경
        public async Task ChangeStatusAsync(int ticketId, TicketStatus status, int changedBy)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var ticket = await _db.Tickets.FirstOrDefaultAsync(x => x.Id == ticketId);

                if (ticket == null)
                {
                    throw new KeyNotFoundException("Ticket을 찾을 수 없습니다.");
                }

                var before = ticket.Status.ToString();

                // 상태 변경
                ticket.Status = status;
                ticket.UpdatedAt = DateTime.Now;

                // History 기록
                _db.TicketHistories.Add(new TicketHistory
                {
                    TicketId = ticketId,
                    UserId = changedBy,
                    Action = "상태 변경",
                    BeforeValue = before,
                    AfterValue = status.ToString()
                });

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // 댓글 등록
        public async Task<Comment> AddCommentAsync(int ticketId, string content, int userId)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var ticket = await _db.Tickets.FirstOrDefaultAsync(x => x.Id == ticketId);

                if (ticket == null)
                {
                    throw new KeyNotFoundException("Ticket을 찾을 수 없습니다.");
                }

                // 댓글 등록
                var comment = new Comment
                {
                    TicketId = ticketId,
                    UserId = userId,
                    Content = content,
                    CreatedAt = DateTime.Now,
                };

                _db.Comments.Add(comment);

                // Audit 기록
                _db.TicketHistories.Add(new TicketHistory
                {
                    TicketId = ticketId,
                    UserId = userId,
                    Action = "댓글 등록"
                });

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return comment;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
