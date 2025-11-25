using Server.DTOs;
using Server.Models;

namespace Server.Services;
public interface IMessageService {
    Task<Message> SendMessageAsync(MessageCreateDto dto);
    Task DeleteMessageAsync(MessageIdDto dto, long userId);
    Task<IEnumerable<Message>> GetRecentMessagesAsync(long channelId, long offset = 0, byte take = 50);
}
