using Server.DTOs;
using Server.Models;
using Server.Repositories;

namespace Server.Services;
public class ChannelService(IRepository<Channel> channels) : IChannelService {
    private readonly IRepository<Channel> _channels = channels;

    public async Task<Channel> CreateChannelAsync(ChannelCreateDto dto) {
        var c = new Channel {
            //Id = Random.Shared.NextInt64(),
            ServerId = dto.ServerId,
            Name = dto.Name,
            Description = dto.Description
        };
        return await _channels.AddAsync(c);
    }

    public async Task DeleteChannelAsync(IdDto dto, long userId) {
        var c = await _channels.GetAsync(dto.Id) ?? throw new KeyNotFoundException("Channel not found");
        if (c.ServerId != userId) // check for channel management perms here
            throw new UnauthorizedAccessException("You cannot delete this channel");

        await _channels.DeleteAsync(c);
    }

    public async Task<IEnumerable<Channel>> GetAllChannelsAsync(long serverId) {
        return await _channels.GetAllAsync();
    }
}