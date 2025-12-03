using Server.Data;
using Server.DTOs.Input;
using Server.Models;
using Server.Repositories;

namespace Server.Services;
public class ServerService(IRepository<GuildServer> servers, IRepository<Channel> channels, IRepository<Message> messages, AppDbContext db) : IServerService {
    private readonly IRepository<GuildServer> _servers = servers;
    private readonly IRepository<Channel> _channels = channels;
    private readonly IRepository<Message> _messages = messages;
    private readonly AppDbContext _db = db;

    public async Task<GuildServer> CreateServerAsync(ServerCreateDto dto, long userId) {
        if (dto.Name.Length is < 2 or > 128 && userId > 0)
            throw new ArgumentException("Server name must be between 2 and 128 characters");
        var m = new Member {
            UserId = userId,
            JoinedAt = DateTime.UtcNow,
        };
        var s = new GuildServer {
            OwnerId = userId,
            Name = dto.Name,
            Description = dto.Description,
            Tags = dto.Tags,
            InviteUrls = dto.InviteUrls,
            Members = [m],
            CreatedAt = DateTime.UtcNow
        };
        await _servers.AddAsync(s, false);
        m.ServerId = s.Id;
        await _servers.SaveAsync();
        return s;
    }

    public async Task JoinServerAsync(long serverId, long userId) {
        var server = await _servers.GetAsync(serverId) ?? throw new KeyNotFoundException("Server not found");
        if (server.Members.Select(m => m.UserId).Contains(userId))
            throw new InvalidOperationException("User is already a member of this server");

        server.Members.Add(new Member {
            UserId = userId,
            ServerId = serverId,
            JoinedAt = DateTime.UtcNow,
        });
        await _servers.UpdateAsync(server);
    }

    public async Task DeleteServerAsync(long serverId, long userId) {
        var server = await _servers.GetAsync(serverId) ?? throw new KeyNotFoundException("Server not found");
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

    public async Task<IEnumerable<Member>> GetMembersAsync(long serverId, long userId, int page, int pageSize) {
        var server = await _servers.GetAsync(serverId) ?? throw new KeyNotFoundException("Server not found");
        if (!server.Members.Select(m => m.UserId).Contains(userId))
            throw new UnauthorizedAccessException("You are not a member of this server");
        return server.Members
            .Skip(page * pageSize)
            .Take(pageSize);
    }

    public async Task<Member?> GetMemberAsync(long serverId, long userId) => await _db.Members.FindAsync(userId, serverId);

    public async Task UpdateMemberAsync(Member member) {
        _db.Members.Update(member);
        await _db.SaveChangesAsync();
    }
}