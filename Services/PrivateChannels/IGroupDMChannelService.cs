using Server.DTOs;
using Server.Models;

namespace Server.Services.PrivateChannels;
public interface IGroupDMChannelService {
    Task<GroupDMChannel> CreateChannelAsync(PrivateChannelCreateDto dto, long userId);
    Task DeleteChannelAsync(IdDto dto, long userId);
    Task<IEnumerable<GroupDMChannel>> GetAllChannelsAsync(long userId);
}