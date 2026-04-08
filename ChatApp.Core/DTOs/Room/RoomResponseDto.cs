namespace ChatApp.Core.DTOs.Room
{
    public class RoomResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<RoomMemberDto> Members { get; set; }
    }
}
