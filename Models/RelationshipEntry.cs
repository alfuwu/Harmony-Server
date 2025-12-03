using Server.Models.Enums;

namespace Server.Models;
public class RelationshipEntry {
    public long UserId { get; set; }
    public UserRelationshipType Type { get; set; }
    public bool IsIncoming { get; set; }
}
