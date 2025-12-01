namespace Server.Models.Enums;
public enum UserRelationshipType : byte {
    Self,
    Friend,
    Pending,
    Regular,
    Blocked
}
