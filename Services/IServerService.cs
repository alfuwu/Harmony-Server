using Server.DTOs;
using Server.Models;

namespace Server.Services;
public interface IServerService {
    Task<GuildServer> CreateServerAsync(ServerCreateDto dto, long userId);
    Task JoinServerAsync(IdDto dto, long userId);
    Task DeleteServerAsync(IdDto dto, long userId);
    Task<IEnumerable<GuildServer>> GetServersAsync(long userId);
    Task<GuildServer?> GetServerFromInviteUrlAsync(string inviteUrl);
}