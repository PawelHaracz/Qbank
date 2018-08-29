using System.Collections.Concurrent;

namespace Qbank.Core
{
    public class Snapshot<TState>
    {
        private static ConcurrentDictionary<string, Snapshot<TState>> _snapshotDictionary = new ConcurrentDictionary<string, Snapshot<TState>>();
        public Snapshot(TState state, int version)
        {
            State = state;
            Version = version;
        }

        public TState State { get; }
        public int Version { get; }

        public static bool TryGetSnapshot(string key, out Snapshot<TState> snapshot) => _snapshotDictionary.TryGetValue(key, out snapshot);

        public static void AddSnapshot(string key, Snapshot<TState> snapshot) => _snapshotDictionary.AddOrUpdate(key, snapshot, (_, __) => snapshot);
    }
}