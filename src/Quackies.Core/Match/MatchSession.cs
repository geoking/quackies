using System;
using System.Collections.Generic;
using System.Linq;
using Quackies.Core.Randomness;
using Quackies.Core.Rules;

namespace Quackies.Core.Match
{
    /// <summary>
    /// Shared authoritative command boundary. Profiles own their state and phase
    /// rules; clients receive only detached observations and issued commands.
    /// </summary>
    public sealed class MatchSession<TView>
    {
        private readonly IMatchRuntime<TView> _runtime;
        private readonly object _commandScope = new object();
        private readonly Dictionary<string, long> _revisions = new Dictionary<string, long>(StringComparer.Ordinal);

        internal MatchSession(IMatchRuntime<TView> runtime)
        {
            _runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
        }

        public TView GetSnapshot(string playerId) => _runtime.GetSnapshot(playerId);

        public IReadOnlyList<GameAction> GetLegalActions(string playerId)
        {
            var actions = _runtime.GetLegalActions(playerId);
            var revision = Revision(playerId);
            return MatchView.Freeze(actions.Select(action =>
                action.Issue(_commandScope, playerId, revision, _runtime.ActionWindow)));
        }

        public TView Execute(string playerId, GameAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (!action.WasIssued(_commandScope, playerId, Revision(playerId), _runtime.ActionWindow))
                throw new InvalidOperationException("This action is stale or belongs to another player or match. Request current legal actions.");
            var legal = _runtime.GetLegalActions(playerId).SingleOrDefault(candidate => candidate.Id == action.Id);
            if (legal == null) throw new InvalidOperationException($"Action '{action.Id}' is no longer legal for {playerId}.");
            var view = _runtime.Execute(playerId, legal);
            _revisions[playerId] = Revision(playerId) + 1;
            return view;
        }

        private long Revision(string playerId)
        {
            if (playerId == null) throw new ArgumentNullException(nameof(playerId));
            return _revisions.TryGetValue(playerId, out var revision) ? revision : 0;
        }
    }

    /// <summary>Compatibility facade for the completed classic profile and profile factories.</summary>
    public sealed partial class MatchSession
    {
        private readonly ClassicMatchRuntime _classic;
        private readonly MatchSession<MatchView> _session;

        private MatchSession(ClassicMatchRuntime classic)
        {
            _classic = classic;
            _session = new MatchSession<MatchView>(classic);
        }

        public static MatchSession Create(IRandomSource random, RuleSet? rules = null) =>
            new MatchSession(ClassicMatchRuntime.Create(random, rules));

        public static MatchSession Create(IRandomSource random, MatchSettings settings, RuleSet? rules = null) =>
            new MatchSession(ClassicMatchRuntime.Create(random, settings, rules));

        public int Round => _classic.Round;
        public MatchPhase Phase => _classic.Phase;
        public MatchSettings Settings => _classic.Settings;
        public MatchView GetSnapshot(string playerId) => _session.GetSnapshot(playerId);
        public IReadOnlyList<GameAction> GetLegalActions(string playerId) => _session.GetLegalActions(playerId);
        public MatchView Execute(string playerId, GameAction action) => _session.Execute(playerId, action);
    }
}
