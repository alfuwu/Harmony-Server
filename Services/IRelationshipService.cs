using Server.Models.Enums;
using Server.Models.Enums.Settings;

namespace Server.Services;
public interface IRelationshipService {
    Task<Relationship> GetUserContext(long id, long targetId);
    Task<bool> CanSee(long id, long targetId, UserContext privacyCtx);
    bool CanSee(Relationship relationship, UserContext privacyCtx);
    Task<UserRelationshipType> GetRelationship(long id, long targetId);
    void SendFriendRequest(long id, long targetId, FriendRequestContext ctx);
    void AcceptFriendRequest(long id, long targetId);
    void BlockUser(long id, long targetId);
    Task<List<long>> GetFriends(long id);
}