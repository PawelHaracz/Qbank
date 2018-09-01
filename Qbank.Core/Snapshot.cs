using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Qbank.Core
{
    public class Snapshot<TState>
    {
        private static ConcurrentDictionary<string, Snapshot<TState>> _snapshotDictionary = new ConcurrentDictionary<string, Snapshot<TState>>();
        public Snapshot(TState state, long version)
        {
            State = state;
            Version = version;
        }

        public TState State { get; }
        public long Version { get; }

        public static bool TryGetSnapshot(string key, out Snapshot<TState> snapshot) => _snapshotDictionary.TryGetValue(key, out snapshot);

        public static void AddSnapshot(string key, Snapshot<TState> snapshot) => _snapshotDictionary.AddOrUpdate(key, snapshot, (_, __) => snapshot);
    }

    public class SnapshotProjection<TState>
    {
        private static ConcurrentDictionary<string, SnapshotProjection<Dictionary<string, TState>>> _snapshotDictionary = new ConcurrentDictionary<string, SnapshotProjection<Dictionary<string, TState>>>();

        public SnapshotProjection(TState state, long version)
        {
            State = state;
            Version = version;
        }

        public TState State { get; }
        public long Version { get; }

        public static bool TryGetSnapshot(string key, out SnapshotProjection<Dictionary<string, TState>> snapshot) => _snapshotDictionary.TryGetValue(key, out snapshot);

        public static void AddSnapshot(string key, SnapshotProjection<Dictionary<string, TState>> snapshot) => _snapshotDictionary.AddOrUpdate(key, snapshot, (_, __) => snapshot);
    }
}