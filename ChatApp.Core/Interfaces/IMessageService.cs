using ChatApp.Core.DTOs.Message;

namespace ChatApp.Core.Interfaces
{
    public interface IMessageService
    {
        Task<MessageResponseDto> SendMessageAsync(SendMessageDto dto, Guid senderId);
        Task<List<MessageResponseDto>> GetRoomMessagesAsync(Guid roomId, int page, int pageSize);
        Task MarkAsReadAsync(Guid messageId, Guid userId);
        Task DeleteMessageAsync(Guid messageId, Guid userId);
    }
}
