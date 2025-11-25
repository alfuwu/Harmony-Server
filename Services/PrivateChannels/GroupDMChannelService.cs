using Server.DTOs;
using Server.Models;
using Server.Models.Enums;
using Server.Repositories;

namespace Server.Services.PrivateChannels;
public class GroupDMChannelService(IRepository<GroupDMChannel> channels) : IGroupDMChannelService {
    private readonly IRepository<GroupDMChannel> _channels = channels;

    public async Task<GroupDMChannel> CreateChannelAsync(PrivateChannelCreateDto dto, long userId) {
        if (dto.Type != ChannelType.GroupDM)
            throw new ArgumentException("Cannot create a non-DM/group DM channel outside of a server");

        var c = new GroupDMChannel {
            OwnerId = userId,
            Members = [.. dto.Others.Append(userId).Distinct()],
            Type = dto.Type,
            CreatedAt = DateTime.UtcNow
        };
        return await _channels.AddAsync(c);
    }

    public async Task DeleteChannelAsync(IdDto dto, long userId) {
        var c = await _channels.GetAsync(dto.Id) ?? throw new KeyNotFoundException("Channel not found");
        if (userId != c.OwnerId)
            throw new UnauthorizedAccessException("You cannot delete this channel");

        await _channels.DeleteAsync(c);
    }

    public async Task<IEnumerable<GroupDMChannel>> GetAllChannelsAsync(long userId) =>
        await _channels.GetForIdAsync(userId);
}