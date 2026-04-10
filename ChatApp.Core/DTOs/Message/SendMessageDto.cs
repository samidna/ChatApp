namespace ChatApp.Core.DTOs.Message
{
    public class SendMessageDto
    {
        public Guid RoomId { get; set; }
        public string Content { get; set; } 
        public string? FileUrl { get; set; }
    }
}
