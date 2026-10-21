namespace MiniDesk.Models
{
    public class TicketAttachment
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        /// <summary>
        /// 사용자가 업로드한 원본 파일명
        /// </summary>
        public string FileName { get; set; } = string.Empty;
        /// <summary>
        /// 서버에 실제 저장된 파일명
        /// </summary>
        public string StoredFileName { get; set; } = string.Empty;
        /// <summary>
        /// 서버 내부 파일 경로
        /// </summary>
        public string FilePath { get; set; } = string.Empty;
        /// <summary>
        /// MIME Type
        /// </summary>
        public string ContentType { get; set; } = string.Empty;
        /// <summary>
        /// 파일 크기(byte)
        /// </summary>
        public long FileSize { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        /// <summary>
        /// Navigation Property
        /// </summary>
        public Ticket Ticket { get; set; } = null!;
    }
}
