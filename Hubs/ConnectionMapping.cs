using System.Collections.Concurrent;

namespace Server.Hubs;
public static class ConnectionMapping {
    private static readonly ConcurrentDictionary<long, HashSet<string>> _connections = new();

    public static void Add(long userId, string connectionId) {
        _connections.AddOrUpdate(userId,
            _ => [connectionId],
            (_, set) => { lock (set) set.Add(connectionId); return set; });
    }

    public static void Remove(long userId, string connectionId) {
        if (_connections.TryGetValue(userId, out var set)) {
            lock (set) set.Remove(connectionId);
            if (set.Count == 0)
                _connections.TryRemove(userId, out _);
        }
    }

    public static IReadOnlyCollection<string> ForUser(long userId)
        => _connections.TryGetValue(userId, out var set) ? set : Array.Empty<string>();
}
