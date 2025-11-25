using Server.Models.Enums;

namespace Server.Models;
public class Role {
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public long[]? LinkedRoles { get; set; }
    public long[]? MutuallyExclusiveRoles { get; set; }
    public RolePrerequisite[]? Prerequisites { get; set; }
    public long Permissions { get; set; }
    public int Priority { get; set; }
    public int DisplayPriority { get; set; }
    public RoleDisplayType DisplayType { get; set; }

    // granular perms
    /// <summary>
    /// A list of role IDs that can see this role in the role list.
    /// <br/>
    /// If empty, it will be visible to everyone.
    /// </summary>
    public long[]? VisibleTo { get; set; }
    /// <summary>
    /// A given set of roles that this role can give to others, regardless of hierarchy.
    /// <br/>
    /// Operates independently of the "Permissions" field.
    /// </summary>
    public long[]? GrantableRoles { get; set; }
    /// <summary>
    /// A list of role IDs that can ping this role.
    /// </summary>
    public long[]? CanPing { get; set; }
}
