using Server.DTOs;
using Server.Models;
using Server.Repositories;

namespace Server.Services;
public class ChannelService(IRepository<Channel> channels) : IChannelService {
    private readonly IRepository<Channel> _channels = channels;

    public async Task<ChannelDto> CreateChannelAsync(ChannelDto dto) {
        var c = new Channel {
            Id = Random.Shared.NextInt64(),
            Name = dto.Name,
        };
        await _channels.AddAsync(c);
        dto.Id = c.Id;
        return dto;
    }

    public async Task<IEnumerable<ChannelDto>> GetAllChannelsAsync() {
        var all = await _channels.GetAllAsync();
        return all.Select(c => new ChannelDto {
            Id = c.Id,
            Name = c.Name
        });
    }
}