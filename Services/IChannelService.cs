using Server.DTOs;
using Server.Models;

namespace Server.Services;
public interface IChannelService {
    Task<Channel> CreateChannelAsync(ChannelCreateDto dto);
    Task DeleteChannelAsync(IdDto dto, long userId);
    Task<IEnumerable<Channel>> GetAllChannelsAsync(long serverId);
}