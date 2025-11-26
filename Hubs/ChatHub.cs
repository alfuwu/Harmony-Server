using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Server.DTOs;
using Server.Services;

namespace Server.Hubs;
[Authorize]
public class ChatHub(IMessageService messageService, IUserService userService) : Hub {
    private readonly IMessageService _messageService = messageService;
    private readonly IUserService _userService = userService;

    public override async Task OnConnectedAsync() {
        // update user's last seen
        var uid = Context.User?.FindFirst("uid")?.Value;
        if (long.TryParse(uid, out var id)) {
            // update presence table / cache? idk
        }
        await base.OnConnectedAsync();
    }

    public async Task JoinChannel(string channelId) {
        await Groups.AddToGroupAsync(Context.ConnectionId, channelId);
    }

    public async Task LeaveChannel(string channelId) {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelId);
    }

    public async Task SendMessage(long channelId, string content) {
        var uid = Context.User?.FindFirst("uid")?.Value;
        if (!long.TryParse(uid, out var senderId))
            throw new HubException("Invalid user id");

        var dto = new MessageCreateDto {
            Content = content
        };
        var saved = await _messageService.SendMessageAsync(dto, channelId, senderId);
        await Clients.Group(channelId.ToString()).SendAsync("ReceiveMessage", saved);
    }
}