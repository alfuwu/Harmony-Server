using Server.DTOs;

namespace Server.Services;
public interface IChannelService {
    Task<ChannelDto> CreateChannelAsync(ChannelDto dto);
    Task<IEnumerable<ChannelDto>> GetAllChannelsAsync();
}