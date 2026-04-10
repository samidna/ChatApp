using ChatApp.Core.DTOs.Message;
using ChatApp.Core.Entities;
using ChatApp.Core.Interfaces;
using ChatApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Services
{
    public class MessageService : IMessageService
    {
        private readonly AppDbContext _context;

        public MessageService(AppDbContext context)
        {
            _context = context;
        }

        public async Task DeleteMessageAsync(Guid messageId, Guid userId)
        {
            var message = await _context.Messages
               .FirstOrDefaultAsync(m => m.Id == messageId)
            ?? throw new Exception("Message not found.");

            if (message.SenderId != userId)
                throw new Exception("You can delete only your messages.");

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
        }

        public async Task<List<MessageResponseDto>> GetRoomMessagesAsync(Guid roomId, int page, int pageSize)
        {
            return await _context.Messages
               .Include(m => m.Sender)
               .Where(m => m.RoomId == roomId)
               .OrderByDescending(m => m.SentAt)
               .Skip((page - 1) * pageSize)
               .Take(pageSize)
               .Select(m => MapToDto(m))
               .ToListAsync();
        }

        public async Task MarkAsReadAsync(Guid messageId, Guid userId)
        {
            var message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == messageId);

            if (message is null)
                throw new Exception("Message not found.");

            message.IsRead = true;
            await _context.SaveChangesAsync();
        }

        public async Task<MessageResponseDto> SendMessageAsync(SendMessageDto dto, Guid senderId)
        {
            var room = await _context.Rooms.AnyAsync(r => r.Id == dto.RoomId);

            if (!room)
                throw new Exception("Room not found");

            var isMember = await _context.RoomMembers
                .AnyAsync(rm => rm.RoomId == dto.RoomId && rm.UserId == senderId);

            if (!isMember)
                throw new Exception("You are not belong this room.");

            var message = new Message
            {
                Content = dto.Content,
                FileUrl = dto.FileUrl,
                RoomId = dto.RoomId,
                SenderId = senderId,
                SentAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            await _context.Entry(message)
                .Reference(m => m.Sender)
                .LoadAsync();

            return MapToDto(message);
        }

        private static MessageResponseDto MapToDto(Message message) => new()
        {
            Id = message.Id,
            Content = message.Content,
            FileUrl = message.FileUrl,
            IsRead = message.IsRead,
            SentAt = message.SentAt,
            SenderId = message.SenderId,
            SenderUsername = message.Sender.UserName!,
            RoomId = message.RoomId
        };
    }
}
