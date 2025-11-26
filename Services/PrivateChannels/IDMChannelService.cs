using Server.DTOs.Input;
using Server.Models;

namespace Server.Services.PrivateChannels;
public interface IDmChannelService {
    Task<DmChannel> CreateChannelAsync(PrivateChannelCreateDto dto, long userId);
    Task DeleteChannelAsync(long channelId, long userId);
    Task<IEnumerable<DmChannel>> GetAllChannelsAsync(long userId);
}