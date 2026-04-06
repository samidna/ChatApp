namespace ChatApp.Core.Entities;

public class Message
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Content { get; set; } = string.Empty;
    public string? FileUrl { get; set; }
    public bool IsRead { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public Guid SenderId { get; set; }
    public AppUser Sender { get; set; } = null!;

    public Guid RoomId { get; set; }
    public Room Room { get; set; } = null!;
}
