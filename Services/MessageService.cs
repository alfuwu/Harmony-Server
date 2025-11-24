using Server.DTOs;
using Server.Models;
using Server.Repositories;

namespace Server.Services;
public class MessageService(IRepository<Message> messages) : IMessageService {
    private readonly IRepository<Message> _messages = messages;

    public async Task<MessageDto> SendMessageAsync(MessageDto dto) {
        var m = new Message {
            Id = Random.Shared.NextInt64(),
            ChannelId = dto.ChannelId,
            AuthorId = dto.AuthorId,
            Content = dto.Content,
            Timestamp = DateTime.UtcNow
        };
        await _messages.AddAsync(m);
        dto.Id = m.Id;
        dto.Timestamp = m.Timestamp;
        return dto;
    }

    public async Task<IEnumerable<MessageDto>> GetRecentMessagesAsync(long channelId, byte take = 50) {
        var msgs = (await _messages.FindAsync(m => m.ChannelId == channelId && !m.IsDeleted)).OrderByDescending(m => m.Timestamp).Take(take);
        return msgs.Select(m => new MessageDto {
            Id = m.Id,
            ChannelId = m.ChannelId,
            AuthorId = m.AuthorId,
            Content = m.Content,
            Timestamp = m.Timestamp
        }).Reverse();
    }
}