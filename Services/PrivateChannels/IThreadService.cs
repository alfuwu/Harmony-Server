using Server.DTOs;
using Server.Models;

namespace Server.Services.PrivateChannels;
public interface IThreadService {
    Task<ThreadChannel> CreateThreadAsync(ChannelCreateDto dto, long userId, long serverId);
    Task DeleteThreadAsync(IdDto dto, long userId);
    Task<IEnumerable<ThreadChannel>> GetAllThreadsAsync(long channelId);
}