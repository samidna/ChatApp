namespace ChatApp.Core.DTOs.User
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; } 
        public bool IsOnline { get; set; }
    }
}
