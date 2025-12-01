namespace Server.Models.Enums.Settings;
public enum FriendRequestContext : byte {
    Everyone,
    FriendsOfFriends,
    MutualsAndFriendsOfFriends,
    Mutuals,
    NoOne
}
