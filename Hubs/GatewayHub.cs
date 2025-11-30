using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Server.DTOs.Output;
using Server.Models;
using Server.Services;

namespace Server.Hubs;
[Authorize]
public class GatewayHub(IUserService userService) : Hub {
    private readonly IUserService _userService = userService;

    public override async Task OnConnectedAsync() {
        // update user's last seen
        var uid = Context.User?.FindFirst("uid")?.Value;
        if (long.TryParse(uid, out var id)) {
            // update presence table / cache? idk
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? ex) {
        // Update presence
        await base.OnDisconnectedAsync(ex);
    }

    public Task JoinServer(long serverId) =>
        Groups.AddToGroupAsync(Context.ConnectionId, $"server:{serverId}");
    public Task LeaveServer(long serverId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, $"server:{serverId}");

    public Task JoinChannel(long channelId) =>
        Groups.AddToGroupAsync(Context.ConnectionId, $"channel:{channelId}");
    public Task LeaveChannel(long channelId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, $"channel:{channelId}");

    public Task JoinDm(long dmId) =>
        Groups.AddToGroupAsync(Context.ConnectionId, $"dm:{dmId}");
    public Task LeaveDm(long dmId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, $"dm:{dmId}");

    public async Task SendMessage(Message msg) {
        //var uid = Context.User?.FindFirst("uid")?.Value;
        //if (!long.TryParse(uid, out var senderId))
        //    throw new HubException("Invalid user id");

        await Clients.Group($"channel:{msg.ChannelId}").SendAsync("RecvMsg", msg);
    }
    public async Task StartTyping(long channelId) {
        var uid = Context.User?.FindFirst("uid")?.Value;
        if (!long.TryParse(uid, out var senderId))
            throw new HubException("Invalid user id");
        if (await _userService.GetByIdAsync(senderId) == null)
            throw new HubException("User not found");

        await Clients.Group($"channel:{channelId}").SendAsync("Typing", new TypingDto {
            ChannelId = channelId,
            UserId = senderId
        });
    }
}