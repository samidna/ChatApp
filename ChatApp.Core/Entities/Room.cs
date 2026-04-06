namespace ChatApp.Core.Entities
{
    public class Room
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public bool IsPrivate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public ICollection<RoomMember> RoomMembers { get; set; } = new List<RoomMember>();
    }
}
