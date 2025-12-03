using Server.Models;
using Server.Models.Enums;
using Server.Models.Enums.Settings;

namespace Server.Services;
public interface IRelationshipService {
    Task<Relationship> GetUserContext(long id, long targetId);
    Task<bool> CanSee(long id, long targetId, UserContext privacyCtx);
    bool CanSee(Relationship relationship, UserContext privacyCtx);
    Task<UserRelationshipType> GetRelationship(long id, long targetId);
    Task SendFriendRequest(long id, long targetId, FriendRequestContext ctx);
    Task AcceptFriendRequest(long id, long targetId);
    Task Unfriend(long id, long targetId);
    Task BlockUser(long id, long targetId);
    Task<List<long>> GetFriends(long id);
    Task<List<RelationshipEntry>> GetPending(long id);
    Task<List<RelationshipEntry>> GetRelationships(long id);
}