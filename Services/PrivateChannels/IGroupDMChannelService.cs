using Server.DTOs.Input;
using Server.Models;

namespace Server.Services.PrivateChannels;
public interface IGroupDmChannelService {
    Task<GroupDmChannel> CreateChannelAsync(PrivateChannelCreateDto dto, long userId);
    Task DeleteChannelAsync(long channelId, long userId);
    Task<IEnumerable<GroupDmChannel>> GetAllChannelsAsync(long userId);
}