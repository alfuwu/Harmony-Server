using Server.DTOs;
using Server.Models;
using Server.Repositories;

namespace Server.Services;
public class ServerService(IRepository<GuildServer> servers, IRepository<Channel> channels, IRepository<Message> messages) : IServerService {
    private readonly IRepository<GuildServer> _servers = servers;
    private readonly IRepository<Channel> _channels = channels;
    private readonly IRepository<Message> _messages = messages;

    public async Task<GuildServer> CreateServerAsync(ServerCreateDto dto, long userId) {
        if (dto.Name.Length is < 2 or > 128 && userId > 0)
            throw new ArgumentException("Server name must be between 2 and 128 characters");
        var s = new GuildServer {
            OwnerId = userId,
            Name = dto.Name,
            Description = dto.Description,
            Tags = dto.Tags,
            InviteUrls = dto.InviteUrls,
            Members = [userId],
            CreatedAt = DateTime.UtcNow
        };
        return await _servers.AddAsync(s);
    }

    public async Task JoinServerAsync(IdDto dto, long userId) {
        var server = await _servers.GetAsync(dto.Id) ?? throw new KeyNotFoundException("Server not found");
        if (server.Members.Contains(userId))
            throw new InvalidOperationException("User is already a member of this server");

        server.Members.Add(userId);
        await _servers.UpdateAsync(server);
    }

    public async Task DeleteServerAsync(IdDto dto, long userId) {
        var server = await _servers.GetAsync(dto.Id) ?? throw new KeyNotFoundException("Server not found");
        if (server.OwnerId != userId && userId > 0)
            throw new UnauthorizedAccessException("You are not the owner of this server");

        await _servers.DeleteAsync(server);

        // delete channels
        await _channels.FindAsync(c => c.ServerId == server.Id).ContinueWith(async t => {
            foreach (var channel in t.Result) {
                await _channels.DeleteAsync(channel, false); // don't save so that we can batch save later

                // delete messages
                await _messages.FindAsync(m => m.ChannelId == channel.Id).ContinueWith(async mt => {
                    foreach (var message in mt.Result)
                        await _messages.DeleteAsync(message, false);
                });
            }
        });
        await _channels.SaveAsync();
    }

    public async Task<IEnumerable<GuildServer>> GetServersAsync(long userId) =>
        await _servers.GetForIdAsync(userId);

    public async Task<GuildServer?> GetServerFromInviteUrlAsync(string inviteUrl) =>
        (await _servers.FindAsync(s => s.InviteUrls != null && s.InviteUrls.Contains(inviteUrl))).First();
}