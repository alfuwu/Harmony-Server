using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using Server.Models.Enums;
using Server.Models.Enums.Settings;

namespace Server.Services;
public class RelationshipService(IServerService serverService, AppDbContext db) : IRelationshipService {
    private readonly IServerService _serverService = serverService;
    private readonly AppDbContext _db = db;

    private static (long, long) GetKey(long userId1, long userId2) => userId1 < userId2 ? (userId1, userId2) : (userId2, userId1);

    public async Task<Relationship> GetUserContext(long id, long targetId) {
        if (id == targetId)
            return Relationship.Self;

        var requesterFriends = await GetFriends(id);
        var targetFriends = await GetFriends(targetId);

        if (requesterFriends.Contains(targetId))
            return Relationship.Friend;

        var isFriendOfFriend = requesterFriends.Intersect(targetFriends).Any();
        var sharedServers = (await _serverService.GetServersAsync(id))
                            .Intersect(await _serverService.GetServersAsync(targetId));
        var mutual = sharedServers.Any();

        if (mutual && isFriendOfFriend)
            return Relationship.MutualAndFriendOfFriend;

        if (mutual)
            return Relationship.Mutual;

        if (isFriendOfFriend)
            return Relationship.FriendOfFriend;

        return Relationship.Everyone;
    }

    public async Task<bool> CanSee(long id, long targetId, UserContext privacyCtx) {
        var relationship = await GetUserContext(id, targetId);

        return CanSee(relationship, privacyCtx);
    }

    public bool CanSee(Relationship relationship, UserContext privacyCtx) {
        return privacyCtx switch {
            UserContext.Everyone => true,
            UserContext.Friends => relationship == Relationship.Friend,
            UserContext.Mutuals => relationship == Relationship.Mutual || relationship == Relationship.MutualAndFriend,
            UserContext.FriendsOfFriends => relationship == Relationship.MutualAndFriendOfFriend ||
                                            relationship == Relationship.FriendOfFriend ||
                                            relationship == Relationship.Friend,
            UserContext.MutualsAndFriends => relationship == Relationship.MutualAndFriendOfFriend ||
                                             relationship == Relationship.MutualAndFriend ||
                                             relationship == Relationship.Mutual ||
                                             relationship == Relationship.Friend,
            UserContext.MutualsAndFriendsOfFriends => relationship == Relationship.MutualAndFriendOfFriend ||
                                                      relationship == Relationship.MutualAndFriend ||
                                                      relationship == Relationship.Mutual ||
                                                      relationship == Relationship.Friend ||
                                                      relationship == Relationship.FriendOfFriend,
            UserContext.NoOne => relationship == Relationship.Self,
            _ => false
        };
    }

    private async Task<bool> CanSendFriendRequest(long id, long targetId, FriendRequestContext ctx) {
        var relationship = await GetUserContext(id, targetId);

        return ctx switch {
            FriendRequestContext.Everyone => true,
            FriendRequestContext.Mutuals => relationship == Relationship.Mutual,
            FriendRequestContext.FriendsOfFriends => relationship == Relationship.MutualAndFriendOfFriend ||
                                            relationship == Relationship.FriendOfFriend,
            FriendRequestContext.MutualsAndFriendsOfFriends => relationship == Relationship.MutualAndFriendOfFriend ||
                                                      relationship == Relationship.Mutual ||
                                                      relationship == Relationship.FriendOfFriend,
            FriendRequestContext.NoOne => false,
            _ => false
        };
    }

    public async Task<UserRelationshipType> GetRelationship(long id, long targetId) {
        if (id == targetId)
            return UserRelationshipType.Self;

        var rel = await _db.Relationships
                     .FirstOrDefaultAsync(r => r.First == id && r.Second == targetId);

        return rel?.Relationship ?? UserRelationshipType.Regular;
    }

    public async void SendFriendRequest(long id, long targetId, FriendRequestContext ctx) {
        var rel = await _db.Relationships
                     .FirstOrDefaultAsync(r => r.First == targetId && r.Second == id);

        if (rel != null || rel?.Relationship != UserRelationshipType.Regular || !await CanSendFriendRequest(id, targetId, ctx))
            throw new UnauthorizedAccessException("You cannot send a friend request to this user");

        if (rel != null) {
            rel.Relationship = UserRelationshipType.Pending;
            _db.Update(rel);
        } else {
            (long first, long second) = GetKey(id, targetId);
            await _db.Relationships.AddAsync(new UserRelationship {
                First = first,
                Second = second,
                Relationship = UserRelationshipType.Pending
            });
        }
        await _db.SaveChangesAsync();
    }

    public async void AcceptFriendRequest(long id, long targetId) {
        var rel = await _db.Relationships
                     .FirstOrDefaultAsync(r => r.First == targetId && r.Second == id);

        if (rel != null && rel.Relationship == UserRelationshipType.Pending) {
            rel.Relationship = UserRelationshipType.Friend;
            _db.Relationships.Update(rel);
            await _db.SaveChangesAsync();
        } else {
            throw new UnauthorizedAccessException("You have no friend request to accept");
        }
    }

    public async void BlockUser(long id, long targetId) {
        var rel = await _db.Relationships
                     .FirstOrDefaultAsync(r => r.First == targetId && r.Second == id);

        if (rel != null) {
            rel.Relationship = UserRelationshipType.Blocked;
            _db.Update(rel);
        } else {
            (long first, long second) = GetKey(id, targetId);
            await _db.Relationships.AddAsync(new UserRelationship {
                First = first,
                Second = second,
                Relationship = UserRelationshipType.Blocked
            });
        }
        await _db.SaveChangesAsync();
    }

    public async Task<List<long>> GetFriends(long userId) {
        var friends = await _db.Relationships
            .Where(r => r.Relationship == UserRelationshipType.Friend &&
                        (r.First == userId || r.Second == userId))
            .Select(r => r.First == userId ? r.Second : r.First)
            .ToListAsync();

        return friends;
    }
}
