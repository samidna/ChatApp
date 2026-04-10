using ChatApp.Core.DTOs.Room;
using ChatApp.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody]CreateRoomDto dto)
        {
            try
            {
                var result = await _roomService.CreateRoomAsync(dto, GetUserId());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMyRooms()
        {
            try
            {
                var result = await _roomService.GetUserRoomsAsync(GetUserId());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetRoom(Guid roomId)
        {
            try
            {
                var result = await _roomService.GetRoomByIdAsync(roomId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{roomId}/members/{userId}")]
        public async Task<IActionResult> AddMember(Guid roomId, Guid userId)
        {
            try
            {
                await _roomService.AddMemberAsync(roomId, userId,GetUserId());
                return Ok("Member added.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{roomId}/members/{userId}")]
        public async Task<IActionResult> RemoveMember(Guid roomId, Guid userId)
        {
            try
            {
                await _roomService.RemoveMemberAsync(roomId, userId);
                return Ok("Member deleted.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{roomId}")]
        public async Task<IActionResult> DeleteRoom(Guid roomId)
        {
            try
            {
                await _roomService.DeleteRoomAsync(roomId, GetUserId());
                return Ok("Room deleted.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
