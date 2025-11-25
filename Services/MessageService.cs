using Server.DTOs;
using Server.Models;
using Server.Repositories;

namespace Server.Services;
public class MessageService(IRepository<Message> messages) : IMessageService {
    private readonly IRepository<Message> _messages = messages;

    public async Task<Message> SendMessageAsync(MessageCreateDto dto) {
        var m = new Message {
            //Id = Random.Shared.NextInt64(),
            ChannelId = dto.ChannelId,
            AuthorId = dto.AuthorId,
            Content = dto.Content,
            Timestamp = DateTime.UtcNow
        };
        await _messages.AddAsync(m);
        //dto.Id = m.Id;
        //dto.Timestamp = m.Timestamp;
        return m;
    }

    public async Task DeleteMessageAsync(MessageIdDto dto, long userId) {
        var m = await _messages.GetAsync(dto.Id) ?? throw new KeyNotFoundException("Message not found");
        if (m.AuthorId != userId) // check for message deletion perms here too
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