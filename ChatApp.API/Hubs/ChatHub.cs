using ChatApp.Core.DTOs.Message;
using ChatApp.Core.Entities;
using ChatApp.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatApp.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly UserManager<AppUser> _userManager;

        public ChatHub(IMessageService messageService, UserManager<AppUser> userManager)
        {
            _messageService = messageService;
            _userManager = userManager;
        }

        private Guid GetUserId() => Guid.Parse(Context.User!.Claims.First(c => c.Type == "uid").Value);

        public async Task JoinRoom(Guid roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
            await Clients.Group(roomId.ToString()).SendAsync("UserJoined", GetUserId(), roomId);
        }

        public async Task LeaveRoom(Guid roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
            await Clients.Group(roomId.ToString()).SendAsync("UserLeft", GetUserId(), roomId);
        }

        public async Task SendMessage(SendMessageDto dto)
        {
            var message = await _messageService.SendMessageAsync(dto, GetUserId());
            await Clients.Group(dto.RoomId.ToString()).SendAsync("ReceiveMessage", message);
        }

        public async Task Typing(Guid roomId)
        {
            var userId = GetUserId();
            await Clients.OthersInGroup(roomId.ToString()).SendAsync("UserTyping", userId, roomId);
        }

        public async Task MarkAsRead(Guid messageId)
        {
            await _messageService.MarkAsReadAsync(messageId, GetUserId());
            await Clients.Caller.SendAsync("MessageRead", messageId);
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                user.IsOnline = true;
                user.LastSeen = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }

            await Clients.All.SendAsync("UserOnline", userId);
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                user.IsOnline = false;
                user.LastSeen = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }

            await Clients.All.SendAsync("UserOffline", userId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
