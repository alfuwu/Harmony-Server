using Server.Models;

namespace Server.DTOs.Output;
// givn to users who can manage role settings
public class FullRoleDto(Role role) : RoleDto(role) {
    public long[]? LinkedRoles { get; set; } = role.LinkedRoles;
    public long[]? MutuallyExclusiveRoles { get; set; } = role.MutuallyExclusiveRoles;
    public List<RolePrerequisite>? Prerequisites { get; set; } = role.Prerequisites;
    public short Priority { get; set; } = role.Priority;

    // granular perms
    /// <summary>
    /// A given set of roles that this role can give to others, regardless of hierarchy.
    /// <br/>
    /// Operates independently of the "Permissions" field.
    /// </summary>
    public long[]? GrantableRoles { get; set; } = role.GrantableRoles;
    /// <summary>
    /// A list of role IDs that can ping this role.
    /// </summary>
    public long[]? CanPing { get; set; } = role.CanPing;
}
