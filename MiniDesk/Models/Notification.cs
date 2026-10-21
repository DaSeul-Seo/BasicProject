namespace MiniDesk.Models
{
    public class Notification
    {
        public int Id { get; set; }
        /// <summary>
        /// 알림을 받는 사용자
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 관련 Ticket
        /// </summary>
        public int? TicketId { get; set; }
        /// <summary>
        /// 알림 제목
        /// </summary>
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// 알림 내용
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 읽음 여부
        /// </summary>
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; } = null;
        public Ticket? Ticket { get; set; }
    }
}
