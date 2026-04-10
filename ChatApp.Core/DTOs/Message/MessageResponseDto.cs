namespace ChatApp.Core.DTOs.Message
{
    public class MessageResponseDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string? FileUrl { get; set; }
        public bool IsRead { get; set; }
        public DateTime SentAt { get; set; }
        public Guid SenderId { get; set; }
        public string SenderUsername { get; set; }
        public Guid RoomId { get; set; }
    }
}
