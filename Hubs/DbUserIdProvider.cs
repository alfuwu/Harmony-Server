using Microsoft.AspNetCore.SignalR;

namespace Server.Hubs;
public class DbUserIdProvider : IUserIdProvider {
    public string? GetUserId(HubConnectionContext connection) =>
        connection.User?.FindFirst("uid")?.Value;
}
