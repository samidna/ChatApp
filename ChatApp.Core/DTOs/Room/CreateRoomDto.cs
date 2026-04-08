namespace ChatApp.Core.DTOs.Room
{
    public class CreateRoomDto
    {
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
        public List<Guid>? MemberIds { get; set; } = new();
    }
}
