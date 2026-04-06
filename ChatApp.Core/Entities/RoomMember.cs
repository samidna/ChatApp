namespace ChatApp.Core.Entities;

public class RoomMember
{
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;

    public Guid RoomId { get; set; }
    public Room Room { get; set; } = null!;

    public bool IsAdmin { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
