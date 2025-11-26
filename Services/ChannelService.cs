using Server.DTOs.Input;
using Server.Models;
using Server.Models.Enums;
using Server.Repositories;

namespace Server.Services;
public class ChannelService(IRepository<Channel> channels, IRepository<GuildServer> servers) : IChannelService {
    private readonly IRepository<Channel> _channels = channels;
    private readonly IRepository<GuildServer> _servers = servers;
    public static readonly ChannelType[] UnrestrictedNameChannelTypes = [
        ChannelType.Category,
        ChannelType.Voice,
        ChannelType.Thread
    ];

    public async Task<Channel> CreateChannelAsync(ChannelCreateDto dto, long serverId, long userId) {
        if (dto.Name.Length is < 2 or > 128 && userId > 0)
            throw new ArgumentException("Channel name must be between 2 and 128 characters");
        if (dto.Description != null && dto.Description.Length > 2048 && userId > 0)
            throw new ArgumentException("Channel description cannot exceed 2048 characters");
        if (dto.Type is < 0 or >= ChannelType.Unknown)
            throw new ArgumentException("Invalid channel type");

        if (dto.Type == ChannelType.DM && dto.Type == ChannelType.GroupDM && userId > 0)
            throw new ArgumentException("Cannot assign a server to a DM channel or a group DM channel");
        else if (dto.Type == ChannelType.Thread)
            throw new ArgumentException("Use the thread creation endpoint to create threads");

        var server = await _servers.GetAsync(serverId) ?? throw new KeyNotFoundException("Server not found");
        if (server.OwnerId != userId && userId > 0) // check for channel management perms here
            throw new UnauthorizedAccessException("You cannot create a channel in this server");

        if (dto.ParentId.HasValue) {
            var parentChannel = await _channels.GetAsync(dto.ParentId.Value);
            if (parentChannel == null || parentChannel.ServerId != serverId)
                throw new KeyNotFoundException("Parent channel not found in this server");
            else if (parentChannel.Type != ChannelType.Category)
                throw new InvalidOperationException("Parent channel must be a category");
        }

        var c = new Channel {
            ParentId = dto.ParentId,
            ServerId = serverId,
            Name = UnrestrictedNameChannelTypes.Contains(dto.Type) ?
                dto.Name :
                dto.Name.ToLower().Replace(' ', '-'),
            Description = dto.Description,
            Type = dto.Type,
            CreatedAt = DateTime.UtcNow
        };

        return await _channels.AddAsync(c);
    }

    public async Task DeleteChannelAsync(long id, long userId) {
        var c = await _channels.GetAsync(id) ?? throw new KeyNotFoundException("Channel not found");
        var server = await _servers.GetAsync(c.ServerId) ?? throw new KeyNotFoundException("Server not found (???)");
        if (server.OwnerId != userId && userId > 0) // check for channel management perms here
            throw new UnauthorizedAccessException("You cannot delete this channel");

        await _channels.DeleteAsync(c);
    }

    public async Task<IEnumerable<Channel>> GetAllChannelsAsync(long serverId) =>
        await _channels.GetForIdAsync(serverId);
}