using Server.Models;
using Server.Models.Enums;

namespace Server.DTOs.Output;
// given to users who can't manage role settings
public class RoleDto(Role role) {
    public long Id { get; set; } = role.Id;
    public long ServerId { get; set; } = role.ServerId;
    public string Name { get; set; } = role.Name;
    public string? Description { get; set; } = role.Description;
    public string? Icon { get; set; } = role.Icon;
    public ulong Permissions { get; set; } = role.Permissions;
    public ushort Position { get; set; } = role.Position;
    public ulong Flags { get; set; } = role.Flags;
    public int? Color { get; set; } = role.Color;
    public List<int>? Colors { get; set; } = role.Colors;
    public RoleDisplayType DisplayType { get; set; } = role.DisplayType;
    public long[]? VisibleTo { get; set; } = role.VisibleTo;
}
