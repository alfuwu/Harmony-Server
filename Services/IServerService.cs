using Server.DTOs.Input;
using Server.Models;

namespace Server.Services;
public interface IServerService {
    Task<GuildServer> CreateServerAsync(ServerCreateDto dto, long userId);
    Task JoinServerAsync(long serverId, long userId);
    Task DeleteServerAsync(long serverId, long userId);
    Task<IEnumerable<GuildServer>> GetServersAsync(long userId);
    Task<GuildServer?> GetServerFromInviteUrlAsync(string inviteUrl);
    Task<IEnumerable<Member>> GetMembersAsync(long serverId, long userId, int page, int pageSize);
}