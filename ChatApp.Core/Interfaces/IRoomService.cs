using ChatApp.Core.DTOs.Room;

namespace ChatApp.Core.Interfaces
{
    public interface IRoomService
    {
        Task<RoomResponseDto> CreateRoomAsync(CreateRoomDto dto, Guid creatorId);
        Task<List<RoomResponseDto>> GetUserRoomsAsync(Guid userId);
        Task<RoomResponseDto> GetRoomByIdAsync(Guid roomId);
        Task AddMemberAsync(Guid roomId, Guid userId);
        Task RemoveMemberAsync(Guid roomId, Guid userId);
        Task DeleteRoomAsync(Guid roomId, Guid requesterId);
    }
}
