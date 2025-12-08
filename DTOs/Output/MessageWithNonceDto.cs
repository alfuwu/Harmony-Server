using Server.Models;

namespace Server.DTOs.Output;
public class MessageWithNonceDto(Message message, long nonce) : MessageDto(message) {
    public long Nonce { get; set; } = nonce;
}