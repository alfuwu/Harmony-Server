using Server.DTOs.Input;
using Server.Models;

namespace Server.Services;
public interface IChannelService {
    Task<Channel> CreateChannelAsync(ChannelCreateDto dto, long serverId, long userId);
    Task DeleteChannelAsync(long id, long userId);
    Task<IEnumerable<Channel>> GetAllChannelsAsync(long serverId);
}