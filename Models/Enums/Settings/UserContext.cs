namespace Server.Models.Enums.Settings;
public enum UserContext : byte {
    Everyone,
    FriendsOfFriends, // friends & friends of friends
    Friends,
    MutualsAndFriendsOfFriends,
    MutualsAndFriends,
    Mutuals, // people who share a server with you
    NoOne
}
