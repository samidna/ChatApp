using ChatApp.Core.DTOs.Room;
using ChatApp.Core.Entities;
using ChatApp.Core.Interfaces;
using ChatApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Services
{
    public class RoomService : IRoomService
    {
        private readonly AppDbContext _context;

        public RoomService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddMemberAsync(Guid roomId, Guid userId)
        {
            var exists = await _context.RoomMembers
            .AnyAsync(rm => rm.RoomId == roomId && rm.UserId == userId);

            if (exists)
                throw new Exception("User is already in room.");

            _context.RoomMembers.Add(new RoomMember
            {
                RoomId = roomId,
                UserId = userId,
                IsAdmin = false,
                JoinedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }

        public async Task<RoomResponseDto> CreateRoomAsync(CreateRoomDto dto, Guid creatorId)
        {
            var existRoom = await _context.Rooms.AnyAsync(r => r.Name == dto.Name);
            if (existRoom)
                throw new Exception("This room name is already exist");

            var room = new Room
            {
                Name = dto.Name,
                IsPrivate = dto.IsPrivate,
                CreatedAt = DateTime.UtcNow
            };

            _context.Rooms.Add(room);

            _context.RoomMembers.Add(new RoomMember
            {
                RoomId = room.Id,
                UserId = creatorId,
                IsAdmin = true,
                JoinedAt = DateTime.UtcNow
            });

            foreach (var memberId in (dto.MemberIds ?? new List<Guid>()).Where(id => id != creatorId))
            {
                _context.RoomMembers.Add(new RoomMember
                {
                    RoomId = room.Id,
                    UserId = memberId,
                    IsAdmin = false,
                    JoinedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return await GetRoomByIdAsync(room.Id);
        }

        public async Task DeleteRoomAsync(Guid roomId, Guid requesterId)
        {
            var room = await _context.Rooms
                .Include(r => r.RoomMembers)
                .FirstOrDefaultAsync(r => r.Id == roomId)
                ?? throw new Exception("Room not found.");

            var isAdmin = room.RoomMembers
                .Any(rm => rm.UserId == requesterId && rm.IsAdmin);

            if (!isAdmin)
                throw new Exception("Only admin can delete any room.");

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }

        public async Task<RoomResponseDto> GetRoomByIdAsync(Guid roomId)
        {
            var room = await _context.Rooms
                .Include(r => r.RoomMembers)
                .ThenInclude(rm => rm.User)
                .FirstOrDefaultAsync(r => r.Id == roomId)
                 ?? throw new Exception("Room not found");

            return MapToDto(room);
        }

        public async Task<List<RoomResponseDto>> GetUserRoomsAsync(Guid userId)
        {
            var rooms = await _context.Rooms
                .Include(r => r.RoomMembers)
                .ThenInclude(rm => rm.User)
                .Where(r => r.RoomMembers.Any(rm => rm.UserId == userId))
                .ToListAsync();

            return rooms.Select(MapToDto).ToList();
        }

        public async Task RemoveMemberAsync(Guid roomId, Guid userId)
        {
            var member = await _context.RoomMembers
                .FirstOrDefaultAsync(rm => rm.RoomId == roomId && rm.UserId == userId)
                ?? throw new Exception("Member not found.");

            _context.RoomMembers.Remove(member);
            await _context.SaveChangesAsync();
        }

        private static RoomResponseDto MapToDto(Room room) => new()
        {
            Id = room.Id,
            Name = room.Name,
            IsPrivate = room.IsPrivate,
            CreatedDate = room.CreatedAt,
            Members = room.RoomMembers.Select(rm => new RoomMemberDto
            {
                UserId = rm.UserId,
                Username = rm.User.UserName!,
                IsAdmin = rm.IsAdmin
            }).ToList()
        };
    }
}
