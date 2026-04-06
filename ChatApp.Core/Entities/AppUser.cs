using Microsoft.AspNetCore.Identity;

namespace ChatApp.Core.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string? AvatarUrl { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastSeen { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public ICollection<RoomMember> RoomMembers { get; set; } = new List<RoomMember>();
    }
}
