using Server.DTOs.Input;
using Server.Models;
using Server.Models.Enums;
using Server.Repositories;

namespace Server.Services.PrivateChannels;
public class GroupDmChannelService(IRepository<GroupDmChannel> channels, IRepository<Message> messages) : IGroupDmChannelService {
    private readonly IRepository<GroupDmChannel> _channels = channels;
    private readonly IRepository<Message> _messages = messages;

    public async Task<GroupDmChannel> CreateChannelAsync(PrivateChannelCreateDto dto, long userId) {
        if (dto.Type != ChannelType.GroupDM)
            throw new ArgumentException("Cannot create a non-DM/group DM channel outside of a server");

        var c = new GroupDmChannel {
            OwnerId = userId,
            Members = [.. dto.Others.Append(userId).Distinct()],
            Type = dto.Type,
            CreatedAt = DateTime.UtcNow
        };
        return await _channels.AddAsync(c);
    }

    public async Task DeleteChannelAsync(long channelId, long userId) {
        var c = await _channels.GetAsync(channelId) ?? throw new KeyNotFoundException("Channel not found");
        if (userId != c.OwnerId)
            throw new UnauthorizedAccessException("You cannot delete this channel");

        await _channels.DeleteAsync(c);

        // delete messages
        await _messages.FindAsync(m => m.ChannelId == channelId).ContinueWith(async t => {
            foreach (var message in t.Result)
                await _messages.DeleteAsync(message, false);
        });
        await _channels.SaveAsync();
    }

    public async Task<IEnumerable<GroupDmChannel>> GetAllChannelsAsync(long userId) =>
        await _channels.GetForIdAsync(userId);
}