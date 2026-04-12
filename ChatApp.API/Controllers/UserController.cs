using ChatApp.Core.DTOs.User;
using ChatApp.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        public UserController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string username)
        {
            var users = await _userManager.Users
                .Where(u => u.UserName!.Contains(username))
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Username = u.UserName!,
                    Email = u.Email!,
                    IsOnline = u.IsOnline
                })
                .ToListAsync();

            return Ok(users);
        }
    }
}
