using Server.DTOs;

namespace Server.Services;
public interface IMessageService {
    Task<MessageDto> SendMessageAsync(MessageDto dto);
    Task<IEnumerable<MessageDto>> GetRecentMessagesAsync(long channelId, byte take = 50);
}
