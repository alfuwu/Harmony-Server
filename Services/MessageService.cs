using Server.Data;
using Server.DTOs;
using Server.Models;
using Server.Repositories;

namespace Server.Services;
public class MessageService(IRepository<Message> messages, AppDbContext db) : IMessageService {
    private readonly IRepository<Message> _messages = messages;
    private readonly AppDbContext _db = db;

    public async Task<Message> SendMessageAsync(MessageCreateDto dto, long channelId, long userId) {
        if (userId > 0)
            if (string.IsNullOrWhiteSpace(dto.Content))
                throw new ArgumentException("Message content cannot be empty");
            else if (dto.Content.Length > 5000)
                throw new ArgumentException("Message content exceeds maximum length of 5000 characters");

        if (await _db.AbstractChannels.FindAsync(channelId) == null)
            throw new KeyNotFoundException("Channel not found");

        var m = new Message {
            ChannelId = channelId,
            AuthorId = userId,
            Content = dto.Content,
            Timestamp = DateTime.UtcNow
        };
        return await _messages.AddAsync(m);
    }

    public async Task DeleteMessageAsync(MessageIdDto dto, long userId) {
        var m = await _messages.GetAsync(dto.Id);
        if (m == null || m.ChannelId != dto.ChannelId)
            throw new Exception("Message not found in the specified channel");
        if (m.AuthorId != userId && userId > 0) // check for message deletion perms here too
            throw new UnauthorizedAccessException("You cannot delete this message");

        await _messages.DeleteAsync(m);
    }

    public async Task<Message> PinMessageAsync(MessageIdDto dto) {
        var m = await _messages.GetAsync(dto.Id);
        if (m == null || m.ChannelId != dto.ChannelId)
            throw new Exception("Message not found in the specified channel");

        m.IsPinned = true;
        await _messages.UpdateAsync(m);
        return new Message {
            Id = m.Id,
            ChannelId = m.ChannelId,
            AuthorId = m.AuthorId,
            Content = m.Content,
            Timestamp = m.Timestamp
        };
    }

    public async Task<IEnumerable<Message>> GetRecentMessagesAsync(long channelId, long offset = 0, byte take = 50) {
        var msgs = (await _messages.FindAsync(m => m.ChannelId == channelId && !m.IsDeleted)).OrderByDescending(m => m.Timestamp).Take(take);
        return msgs.Reverse();
    }
}