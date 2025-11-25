using Server.DTOs;
using Server.Models;
using Server.Models.Enums;
using Server.Repositories;

namespace Server.Services.PrivateChannels;
public class DMChannelService(IRepository<DMChannel> channels) : IDMChannelService {
    private readonly IRepository<DMChannel> _channels = channels;

    public async Task<DMChannel> CreateChannelAsync(PrivateChannelCreateDto dto, long userId) {
        if (dto.Type != ChannelType.DM)
            throw new ArgumentException("Cannot create a non-DM/group DM channel outside of a server");

        long otherId = dto.Others.FirstOrDefault(id => id != userId);
        List<long> members = [.. dto.Others.Append(userId).Distinct()];
        if (members.Count != 2)
            throw new ArgumentException("DM channels can only have one other member");
        else if ((await GetAllChannelsAsync(userId)).Where(chan => chan.Members.Count == 2 && chan.Members.Contains(otherId)).Any())
            throw new ArgumentException("A DM channel with this user already exists");

        var c = new DMChannel {
            Members = members,
            Type = dto.Type
        };
        return await _channels.AddAsync(c);
    }

    public async Task DeleteChannelAsync(IdDto dto, long userId) {
        var c = await _channels.GetAsync(dto.Id) ?? throw new KeyNotFoundException("Channel not found");
        await _channels.DeleteAsync(c);
    }

    public async Task<IEnumerable<DMChannel>> GetAllChannelsAsync(long userId) =>
        await _channels.GetForIdAsync(userId);
}