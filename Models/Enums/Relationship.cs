namespace Server.Models.Enums;
public enum Relationship : byte {
    Self,
    MutualAndFriend,
    Friend,
    MutualAndFriendOfFriend,
    Mutual,
    FriendOfFriend,
    Everyone
}
