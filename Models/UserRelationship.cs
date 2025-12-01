using Server.Models.Enums;

namespace Server.Models;
public class UserRelationship {
    public long First { get; set; }
    public long Second { get; set; }
    public UserRelationship Other { get; set; } = null!;
    public UserRelationshipType Relationship { get; set; }
    public DateTime UpdatedAt { get; set; }
}