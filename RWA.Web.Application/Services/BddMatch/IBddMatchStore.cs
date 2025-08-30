using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RWA.Web.Application.Services.BddMatch
{
    public record BddMatchRow(
        int NumLigne,
        bool AddToBdd,
        string? MatchedRaf,
        string MatchBy,
        string? InventoryIdentifiantUniqueRetenu,
        string? InventoryRaf
    );
    public record BddMatchProgress(string Version, int Processed, int Total);

    public interface IBddMatchStore
    {
        void Init(string version, int total);
        void Append(string version, IEnumerable<BddMatchRow> rows, int processed);
        (IReadOnlyList<BddMatchRow> Items, int Total, int Processed) Get(string version, int skip, int take);
    }

    public class InMemoryBddMatchStore : IBddMatchStore
    {
        private class State
        {
            public int Total;
            public int Processed;
            public List<BddMatchRow> Rows = new();
        }

        private readonly ConcurrentDictionary<string, State> _states = new();

        public void Init(string version, int total)
        {
            _states[version] = new State { Total = total, Processed = 0, Rows = new List<BddMatchRow>() };
        }

        public void Append(string version, IEnumerable<BddMatchRow> rows, int processed)
        {
            if (_states.TryGetValue(version, out var s))
            {
                s.Rows.AddRange(rows);
                s.Processed = processed;
            }
        }

        public (IReadOnlyList<BddMatchRow> Items, int Total, int Processed) Get(string version, int skip, int take)
        {
            if (_states.TryGetValue(version, out var s))
            {
                var page = s.Rows.Count > skip ? s.Rows.GetRange(skip, System.Math.Min(take, s.Rows.Count - skip)) : new List<BddMatchRow>();
                return (page, s.Total, s.Processed);
            }
            return (new List<BddMatchRow>(), 0, 0);
        }
    }
}
