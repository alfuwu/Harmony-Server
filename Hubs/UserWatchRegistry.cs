using System.Collections.Concurrent;

namespace Server.Hubs;
public static class UserWatchRegistry {
    private static readonly ConcurrentDictionary<long, HashSet<long>> _watchers = new();

    public static void AddWatcher(long target, long watcher) {
        _watchers.AddOrUpdate(
            target,
            _ => [watcher],
            (_, set) => { lock (set) set.Add(watcher); return set; }
        );
    }

    public static void RemoveWatcher(long target, long watcher) {
        if (_watchers.TryGetValue(target, out var set)) {
            lock (set) set.Remove(watcher);
            if (set.Count == 0)
                _watchers.TryRemove(target, out _);
        }
    }

    public static IReadOnlyCollection<long> GetWatchers(long target)
        => _watchers.TryGetValue(target, out var set) ? set : Array.Empty<long>();
}
