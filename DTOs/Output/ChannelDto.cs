using Server.Models;
using Server.Models.Enums;

namespace Server.DTOs.Output;
public class ChannelDto(Channel channel) {
    public long Id { get; set; } = channel.Id;
    public long ServerId { get; set; } = channel.ServerId;
    public long? ParentId { get; set; } = channel.ParentId;
    public string Name { get; set; } = channel.Name;
    public string? Description { get; set; } = channel.Description;
    public string? Icon { get; set; } = channel.Icon;
    public ushort Position { get; set; } = channel.Position;
    public ulong Slowmode { get; set; } = channel.Slowmode;
    public ChannelType Type { get; set; } = channel.Type;
    public long? LastMessage { get; set; } = channel.LastMessage;
    public DateTime CreatedAt { get; set; } = channel.CreatedAt;
}
