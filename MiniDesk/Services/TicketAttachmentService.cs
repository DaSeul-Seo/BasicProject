using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MiniDesk.Data;
using MiniDesk.Models;

namespace MiniDesk.Services
{
    public class TicketAttachmentService
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _environment;
        private const long MaxFileSize = 10 * 1024 * 1024;
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".gif",
            ".pdf",
            ".txt",
            ".doc",
            ".docx",
            ".xls",
            ".xlsx",
            ".zip"
        };

        public TicketAttachmentService(AppDbContext db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }

        // 파일 업로드
        public async Task<TicketAttachment> UploadAsync(int ticketId, IFormFile file, int userId)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("업로드할 파일이 없습니다.");
            }

            if (file.Length > MaxFileSize)
            {
                throw new InvalidOperationException("파일 크기는 10MB를 초과할 수 없습니다.");
            }

            var ticket = await _db.Tickets
                .FirstOrDefaultAsync(x => x.Id == ticketId);

            if (ticket == null)
            {
                throw new KeyNotFoundException("Ticket을 찾을 수 없습니다.");
            }

            // 원본 파일명
            var originalFileName = Path.GetFileName(file.FileName);

            var extension = Path.GetExtension(originalFileName);

            if (!AllowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException("허용되지 않는 파일 형식입니다.");
            }

            // Ticket별 폴더
            var folderPath = Path.Combine(_environment.WebRootPath, "uploads", "tickets", ticketId.ToString());

            Directory.CreateDirectory(folderPath);

            // 서버 저장용 랜던 파일명
            var storedFileName = $"{Guid.NewGuid():N}" + Path.GetExtension(originalFileName);

            var physicalPath = Path.Combine(folderPath, storedFileName);

            // 실제 파일 저장
            await using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                // DB 저장
                var attachment = new TicketAttachment
                {
                    TicketId = ticketId,
                    FileName = originalFileName,
                    StoredFileName = storedFileName,
                    FilePath = Path.Combine("uploads", "tickets", ticketId.ToString(), storedFileName),
                    ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application.octet-stream" : file.ContentType,
                    FileSize = file.Length,
                    CreatedAt = DateTime.UtcNow
                };
                
                _db.TicketAttachments.Add(attachment);

                // history
                _db.TicketHistories.Add(new TicketHistory
                {
                    TicketId = ticketId,
                    UserId = userId,
                    Action = "파일 업로드",
                    AfterValue = originalFileName
                });

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return attachment;
            }
            catch
            {
                await transaction.RollbackAsync();

                // DB 저장 실패 시 실제 파일도 삭제
                if (File.Exists(physicalPath))
                {
                    File.Delete(physicalPath);
                }

                throw;
            }
        }

        // 파일 삭제
        public async Task DeleteAsync(int attchmentId, int userId)
        {
            var attachment = await _db.TicketAttachments.FirstOrDefaultAsync(x => x.Id == attchmentId);

            if (attachment == null)
            {
                throw new KeyNotFoundException("첨부파일을 찾을 수 없습니다.");
            }

            var physicalPath = Path.Combine(_environment.WebRootPath, attachment.FilePath);

            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var fileName = attachment.FileName;
                var ticketId = attachment.TicketId;

                // 실제 파일 삭제
                if (File.Exists(physicalPath))
                {
                    File.Delete(physicalPath);
                }
                
                // DB 삭제
                _db.TicketAttachments.Remove(attachment);

                // history
                _db.TicketHistories.Add(new TicketHistory
                {
                    TicketId = ticketId,
                    UserId = userId,
                    Action = "파일 삭제",
                    BeforeValue = fileName
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

        public async Task<int?> GetTicketIdAsync(int attachmentId)
        {
            return await _db.TicketAttachments
                .Where(x => x.Id == attachmentId)
                .Select(x => (int?)x.TicketId)
                .FirstOrDefaultAsync();
        }
    }
}
