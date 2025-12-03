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

    private Task<UserRelationship?> GetDirectional(long sender, long receiver) =>
        _db.Relationships.FirstOrDefaultAsync(r => r.First == sender && r.Second == receiver &&
                                                   (r.Relationship == UserRelationshipType.Pending ||
                                                    r.Relationship == UserRelationshipType.Blocked));

    private Task<UserRelationship?> GetSymmetric(long a, long b) {
        var (first, second) = GetKey(a, b);
        return _db.Relationships.FirstOrDefaultAsync(r => r.First == first &&
                                                          r.Second == second &&
                                                          r.Relationship == UserRelationshipType.Friend);
    }

    private async Task RemoveDirectional(long a, long b) {
        var relAB = await GetDirectional(a, b);
        var relBA = await GetDirectional(b, a);

        if (relAB != null)
            _db.Relationships.Remove(relAB);
        if (relBA != null)
            _db.Relationships.Remove(relBA);
    }

    private async Task RemoveFriendship(long a, long b) {
        var rel = await GetSymmetric(a, b);
        if (rel != null)
            _db.Relationships.Remove(rel);
    }

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

    private async Task ConvertPendingToFriend(long a, long b) {
        // remove any pending or blocked directional relationships both ways
        await RemoveDirectional(a, b);
        await RemoveDirectional(b, a);

        // create symmetric friendship
        var (first, second) = GetKey(a, b);

        await _db.Relationships.AddAsync(new UserRelationship {
            First = first,
            Second = second,
            Relationship = UserRelationshipType.Friend
        });

        await _db.SaveChangesAsync();
    }

    public async Task SendFriendRequest(long id, long targetId, FriendRequestContext ctx) {
        if (id == targetId)
            throw new UnauthorizedAccessException("You cannot friend yourself");

        if (await _db.Users.FindAsync(targetId) == null)
            throw new UnauthorizedAccessException("User not found");

        if (!await CanSendFriendRequest(id, targetId, ctx))
            throw new UnauthorizedAccessException("You cannot send a friend request to this user");

        // if target has already sent id a request, accept it immediately
        var oppositePending = await GetDirectional(targetId, id);
        if (oppositePending?.Relationship == UserRelationshipType.Pending) {
            await ConvertPendingToFriend(id, targetId);
            return;
        }

        // if id already sent a request, forbid
        var existingPending = await GetDirectional(id, targetId);
        if (existingPending?.Relationship == UserRelationshipType.Pending)
            throw new UnauthorizedAccessException("You have already sent a friend request to this user. Be patient.");

        // if already friends, nothing to do
        var existingFriend = await GetSymmetric(id, targetId);
        if (existingFriend != null)
            return;

        // if id is blocked by target or vice versa, forbid
        if (await GetDirectional(targetId, id) is { Relationship: UserRelationshipType.Blocked })
            throw new UnauthorizedAccessException("You are blocked by this user");
        if (await GetDirectional(id, targetId) is { Relationship: UserRelationshipType.Blocked })
            throw new UnauthorizedAccessException("You have blocked this user");

        var pending = new UserRelationship {
            First = id,
            Second = targetId,
            Relationship = UserRelationshipType.Pending
        };

        await _db.Relationships.AddAsync(pending);
        await _db.SaveChangesAsync();
    }

    public async Task AcceptFriendRequest(long id, long targetId) {
        // targetId to id must be a Pending request
        var pending = await GetDirectional(targetId, id);

        if (pending == null || pending.Relationship != UserRelationshipType.Pending)
            throw new UnauthorizedAccessException("You have no friend request from this user that you can accept");

        await ConvertPendingToFriend(id, targetId);
    }

    public async Task Unfriend(long id, long targetId) {
        var friendship = await GetSymmetric(id, targetId);

        if (friendship == null || friendship.Relationship != UserRelationshipType.Friend)
            throw new UnauthorizedAccessException("You are not friends with this user");

        await RemoveFriendship(id, targetId);
    }

    public async Task BlockUser(long id, long targetId) {
        if (id == targetId)
            throw new UnauthorizedAccessException("You cannot block yourself");

        // remove any friendship or pending requests
        await RemoveFriendship(id, targetId);
        await RemoveDirectional(id, targetId);
        await RemoveDirectional(targetId, id);

        // create directional block entry
        await _db.Relationships.AddAsync(new UserRelationship {
            First = id,
            Second = targetId,
            Relationship = UserRelationshipType.Blocked
        });

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

    public async Task<List<RelationshipEntry>> GetPending(long userId) {
        var pending = await _db.Relationships
            .Where(r => r.Relationship == UserRelationshipType.Pending &&
                        (r.First == userId || r.Second == userId))
            .Select(r => new RelationshipEntry {
                UserId = r.First == userId ? r.Second : r.First,
                Type = UserRelationshipType.Pending,
                IsIncoming = r.Second == userId
            })
            .ToListAsync();

        return pending;
    }

    public async Task<List<long>> GetBlockedUsers(long userId) {
        var definitelyFriends = await _db.Relationships
            .Where(r => r.Relationship == UserRelationshipType.Blocked && r.First == userId)
            .Select(r => r.Second)
            .ToListAsync();

        return definitelyFriends;
    }

    public async Task<List<RelationshipEntry>> GetRelationships(long userId) {
        var results = new List<RelationshipEntry>();

        var friends = await _db.Relationships
            .Where(r => r.Relationship == UserRelationshipType.Friend &&
                        (r.First == userId || r.Second == userId))
            .ToListAsync();

        foreach (var rel in friends) {
            var other = rel.First == userId ? rel.Second : rel.First;
            results.Add(new RelationshipEntry {
                UserId = other,
                Type = UserRelationshipType.Friend,
                IsIncoming = false
            });
        }

        var pending = await _db.Relationships
            .Where(r => r.Relationship == UserRelationshipType.Pending &&
                        (r.First == userId || r.Second == userId))
            .ToListAsync();

        foreach (var rel in pending) {
            var other = rel.First == userId ? rel.Second : rel.First;

            results.Add(new RelationshipEntry {
                UserId = other,
                Type = UserRelationshipType.Pending,
                IsIncoming = rel.Second == userId
            });
        }

        return results;
    }
}
