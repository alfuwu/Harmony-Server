using Server.Models.Enums;

namespace Server.DTOs.Input;
public class PrivateChannelCreateDto {
    public long[] Others { get; set; } = [];
    public ChannelType Type { get; set; }
}
