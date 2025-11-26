namespace Server.Models.Enums.Settings;
public enum UserContext {
    Everyone,
    FriendsOfFriends, // friends & friends of friends
    Friends,
    Mutuals, // people who share a server with you
    NoOne
}
