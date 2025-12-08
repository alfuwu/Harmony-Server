using Server.DTOs.Input;
using Server.Models;
using Server.Models.Enums;
using Server.Repositories;

namespace Server.Services.PrivateChannels;
public class ThreadService(IRepository<Channel> channels, IRepository<ThreadChannel> threads, IRepository<GuildServer> servers) : IThreadService {
    private readonly IRepository<Channel> _channels = channels;
    private readonly IRepository<ThreadChannel> _threads = threads;
    private readonly IRepository<GuildServer> _servers = servers;

    public async Task<ThreadChannel> CreateThreadAsync(ChannelCreateDto dto, long userId, long serverId) {
        if (dto.Name.Length is < 2 or > 128 && userId > 0)
            throw new ArgumentException("Channel name must be between 2 and 128 characters");
        if (dto.Description != null && dto.Description.Length > 2048 && userId > 0)
            throw new ArgumentException("Channel description cannot exceed 2048 characters");
        if (dto.Type is < 0 or >= ChannelType.Unknown)
            throw new ArgumentException("Invalid channel type");

        if (dto.Type == ChannelType.DM && dto.Type == ChannelType.GroupDM && userId > 0)
            throw new ArgumentException("Cannot assign a server to a DM channel or a group DM channel");
        else if (dto.Type != ChannelType.Thread)
            throw new ArgumentException("Use the channel creation endpoint to create channels");

        if (!dto.ParentId.HasValue)
            throw new ArgumentException("Thread channels must have a parent channel");
        var parentChannel = await _channels.GetAsync(dto.ParentId.Value) ?? throw new KeyNotFoundException("Parent channel not found");

        if (!parentChannel.Type.CanHaveThread())
            throw new InvalidOperationException("Parent channel must be one of: TEXT (1), ANNOUNCEMENT (3), RULES (4), FORUM (6), DM (9), GROUP_DM (10)");

        var t = new ThreadChannel {
            OwnerId = userId,
            ParentId = dto.ParentId.Value,
            ServerId = serverId,
            Name = dto.Type.CanBeNamedWhatever() ?
                dto.Name :
                dto.Name.ToLower().Replace(' ', '-'),
            Description = dto.Description,
            Members = [userId],
            Type = dto.Type,
            CreatedAt = DateTime.UtcNow
        };

        return await _threads.AddAsync(t);
    }

    public async Task DeleteThreadAsync(IdDto dto, long userId) {
        var t = await _threads.GetAsync(dto.Id) ?? throw new KeyNotFoundException("Thread not found");
        var server = await _servers.GetAsync(t.ServerId) ?? throw new KeyNotFoundException("Server not found (???)");
        if (t.OwnerId != userId && server.OwnerId != userId && userId > 0) // check for channel management perms here
            throw new UnauthorizedAccessException("You cannot delete this channel");

        await _threads.DeleteAsync(t);
    }

    public async Task<IEnumerable<ThreadChannel>> GetAllThreadsAsync(long channelId) =>
        await _threads.GetForIdAsync(channelId);
}