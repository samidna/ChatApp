namespace ChatApp.Core.DTOs.Room
{
    public class RoomMemberDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public bool IsAdmin { get; set; }
    }
}
