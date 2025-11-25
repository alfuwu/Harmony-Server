using Server.DTOs;
using Server.Models;

namespace Server.Services.PrivateChannels;
public interface IDMChannelService {
    Task<DMChannel> CreateChannelAsync(PrivateChannelCreateDto dto, long userId);
    Task DeleteChannelAsync(IdDto dto, long userId);
    Task<IEnumerable<DMChannel>> GetAllChannelsAsync(long userId);
}