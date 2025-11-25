using Server.Models.Enums;

namespace Server.DTOs;
public class ChannelCreateDto {
    public long? ParentId { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public ChannelType Type { get; set; }
}
