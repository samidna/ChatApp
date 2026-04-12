using ChatApp.Core.DTOs.Message;
using ChatApp.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        private Guid GetUserId() => Guid.Parse(User.Claims.First(c => c.Type == "uid").Value);

        [HttpPost]
        public async Task<IActionResult> SendMessage(SendMessageDto dto)
        {
            try
            {
                var result = await _messageService.SendMessageAsync(dto, GetUserId());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetMessages(Guid roomId, int page = 1, int pageSize = 20)
        {
            try
            {
                var result = await _messageService.GetRoomMessagesAsync(roomId, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{messageId}/read")]
        public async Task<IActionResult> MarkAsRead(Guid messageId)
        {
            try
            {
                await _messageService.MarkAsReadAsync(messageId, GetUserId());
                return Ok("Message is read.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{messageId}")]
        public async Task<IActionResult> DeleteMessage(Guid messageId)
        {
            try
            {
                await _messageService.DeleteMessageAsync(messageId, GetUserId());
                return Ok("Message deleted.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
