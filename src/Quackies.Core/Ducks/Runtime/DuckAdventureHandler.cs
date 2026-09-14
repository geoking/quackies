using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Quackies.Core.Ducks.Definitions;
using Quackies.Core.Match;

namespace Quackies.Core.Ducks.Runtime
{
    /// <summary>Authoritative Explore/Settle flow and encounter timing for the Duck profile.</summary>
    internal static class DuckAdventureHandler
    {
        private const string ExploreActionId = "duck.explore";
        private const string SettleActionId = "duck.settle";

        internal static IReadOnlyList<GameAction> GetLegalActions(DuckMatchRuntime runtime, DuckPlayerState player)
        {
            if (runtime == null) throw new ArgumentNullException(nameof(runtime));
            if (player == null) throw new ArgumentNullException(nameof(player));
            if (runtime.State.Phase != DuckPhase.Adventure || player.HasFinishedDay || player.IsWornOut)
                return EmptyActions();
            if (runtime.State.Day == DuckMatchSettings.StandardDays && runtime.State.FinalDayCommits.Any(
                commit => commit.Beat == runtime.State.FinalDayDecisionBeat && commit.PlayerId == player.Id))
                return EmptyActions();

            var actions = new List<GameAction>();
            if (player.BagPhysicalChipIds.Count > 0)
                actions.Add(new GameAction(ExploreActionId, GameActionKind.Explore, "Explore"));
            if (player.PlacedChips.Count > 0)
                actions.Add(new GameAction(SettleActionId, GameActionKind.Settle, "Settle down"));
            return new ReadOnlyCollection<GameAction>(actions);
        }

        internal static void Execute(DuckMatchRuntime runtime, DuckPlayerState player, GameAction action)
        {
            if (runtime == null) throw new ArgumentNullException(nameof(runtime));
            if (player == null) throw new ArgumentNullException(nameof(player));
            if (action == null) throw new ArgumentNullException(nameof(action));

            var legal = GetLegalActions(runtime, player).SingleOrDefault(candidate =>
                string.Equals(candidate.Id, action.Id, StringComparison.Ordinal));
            if (legal == null)
                throw new InvalidOperationException("Action '" + action.Id + "' is not legal for " + player.Id + " in Duck Adventure.");

            if (runtime.State.Day == DuckMatchSettings.StandardDays)
            {
                CommitFinalDayAction(runtime, player, legal.Kind);
                return;
            }

            ResolveAction(runtime, player, legal.Kind);
            ResolveNightWhenEveryoneFinished(runtime);
        }

        private static void CommitFinalDayAction(DuckMatchRuntime runtime, DuckPlayerState player, GameActionKind actionKind)
        {
            var beat = runtime.State.FinalDayDecisionBeat;
            if (beat < 1) throw new InvalidOperationException("Final-Day Adventure requires a positive decision beat.");
            runtime.State.FinalDayCommits.Add(new DuckFinalDayCommitState(beat, player.Id, actionKind));

            var activeCohort = runtime.State.Players.Where(candidate => !candidate.HasFinishedDay).ToArray();
            if (activeCohort.Any(candidate => !runtime.State.FinalDayCommits.Any(
                commit => commit.Beat == beat && commit.PlayerId == candidate.Id)))
                return;

            var commits = runtime.State.FinalDayCommits
                .Where(commit => commit.Beat == beat)
                .ToDictionary(commit => commit.PlayerId, StringComparer.Ordinal);
            foreach (var candidate in activeCohort)
                ResolveAction(runtime, candidate, commits[candidate.Id].ActionKind);

            runtime.State.FinalDayCommits.Clear();
            runtime.State.FinalDayDecisionBeat++;
            ResolveNightWhenEveryoneFinished(runtime);
        }

        private static void ResolveAction(DuckMatchRuntime runtime, DuckPlayerState player, GameActionKind actionKind)
        {
            switch (actionKind)
            {
                case GameActionKind.Explore:
                    Explore(runtime, player);
                    return;
                case GameActionKind.Settle:
                    FinishAdventure(runtime, player, wornOut: false, "settled down");
                    return;
                default:
                    throw new InvalidOperationException("Unsupported Duck Adventure action: " + actionKind);
            }
        }

        private static void Explore(DuckMatchRuntime runtime, DuckPlayerState player)
        {
            if (player.BagPhysicalChipIds.Count == 0)
                throw new InvalidOperationException("There is no chip left to explore.");
            if (player.KnownNextPhysicalChipIds.Count > 0
                && player.KnownNextPhysicalChipIds[0] != player.BagPhysicalChipIds[0])
                throw new InvalidOperationException("The saved Signpost preview does not match the next bag chip.");

            var physicalChipId = player.BagPhysicalChipIds[0];
            player.BagPhysicalChipIds.RemoveAt(0);
            if (player.KnownNextPhysicalChipIds.Count > 0)
                player.KnownNextPhysicalChipIds.RemoveAt(0);

            var physicalChip = player.Inventory.Single(chip => chip.PhysicalChipId == physicalChipId);
            var definition = runtime.Rules.Encounter(physicalChip.DefinitionId);
            var type = definition.EncounterType;
            var wasSplashArmed = player.SplashProtectionArmed;
            player.SplashProtectionArmed = false;
            var splashSuppression = wasSplashArmed && definition.IsObstacle;
            var guideSuppression = definition.IsObstacle && player.GuideProtectionAvailable;
            if (definition.IsObstacle && player.GuideProtectionAvailable)
                player.GuideProtectionAvailable = false;
            var nuisanceSuppressed = splashSuppression || guideSuppression;

            var movement = DetermineMovement(runtime, player, definition);
            player.Position = Math.Min(43, player.Position + movement);
            player.PlacedChips.Add(new DuckPlacedChipState(physicalChipId, player.Position, nuisanceSuppressed));

            if (definition.IsHelpful)
            {
                player.PlacedHelpfulTypes.Add(type);
                ResolveHelpfulAbility(runtime, player, definition);
            }
            else
            {
                ResolveObstacle(player, type, nuisanceSuppressed);
            }

            AwardPocketDriftwood(runtime, player);
            runtime.AddHistory(player.Id, player.Name + " placed " + definition.Name + " at space " + player.Position + ".");

            if (player.Exhaustion > player.SafeExhaustionMaximum)
            {
                FinishAdventure(runtime, player, wornOut: true, "became worn out");
                return;
            }
            if (player.Position == 43)
            {
                FinishAdventure(runtime, player, wornOut: false, "reached the oasis");
                return;
            }
            if (player.BagPhysicalChipIds.Count == 0)
                FinishAdventure(runtime, player, wornOut: false, "emptied the bag");
        }

        private static int DetermineMovement(
            DuckMatchRuntime runtime,
            DuckPlayerState player,
            DuckEncounterDefinition definition)
        {
            var type = definition.EncounterType;
            int movement;
            if (type == DuckEncounterType.Companion)
            {
                player.ActiveFlock++;
                movement = Math.Min(player.ActiveFlock + 1, 4);
            }
            else
            {
                movement = definition.BaseMovement!.Value;
            }

            var eventType = runtime.CurrentEvent.EventType;
            if (eventType == DuckWorldEventType.RainSoftenedSeeds && type == DuckEncounterType.Seeds)
                movement++;

            var alreadyHalved = false;
            if (eventType == DuckWorldEventType.StillAir && type == DuckEncounterType.Tailwind)
            {
                movement = HalveMovement(movement);
                alreadyHalved = true;
            }

            if (definition.IsHelpful && player.LogSlowdownPending)
            {
                if (!alreadyHalved) movement = HalveMovement(movement);
                player.LogSlowdownPending = false;
            }

            return Math.Max(1, movement);
        }

        private static int HalveMovement(int movement)
        {
            return Math.Max(1, (movement + 1) / 2);
        }

        private static void ResolveHelpfulAbility(
            DuckMatchRuntime runtime,
            DuckPlayerState player,
            DuckEncounterDefinition definition)
        {
            switch (definition.EncounterType)
            {
                case DuckEncounterType.Reeds:
                    player.DayReedsTwigs += definition.TwigYield;
                    player.TotalTwigs += definition.TwigYield;
                    break;
                case DuckEncounterType.Signpost:
                    RefreshSignpostPreview(runtime, player);
                    break;
                case DuckEncounterType.Splash:
                    player.SplashProtectionArmed = true;
                    break;
                case DuckEncounterType.Wildflowers:
                    player.FlowersPlaced++;
                    break;
            }
        }

        private static void RefreshSignpostPreview(DuckMatchRuntime runtime, DuckPlayerState player)
        {
            var count = runtime.CurrentEvent.EventType switch
            {
                DuckWorldEventType.SunlitSignboards => 2,
                DuckWorldEventType.ThickMorningMist => 0,
                _ => 1
            };
            player.KnownNextPhysicalChipIds.Clear();
            player.KnownNextPhysicalChipIds.AddRange(player.BagPhysicalChipIds.Take(count));
        }

        private static void ResolveObstacle(
            DuckPlayerState player,
            DuckEncounterType type,
            bool nuisanceSuppressed)
        {
            player.Exhaustion++;
            if (nuisanceSuppressed) return;

            switch (type)
            {
                case DuckEncounterType.FallenLog:
                    player.LogSlowdownPending = true;
                    break;
                case DuckEncounterType.MudPuddle:
                    player.ActiveFlock = Math.Max(0, player.ActiveFlock - 1);
                    break;
                case DuckEncounterType.GrumpyGoose:
                    player.SafeExhaustionMaximum = 4;
                    break;
            }
        }

        private static void AwardPocketDriftwood(DuckMatchRuntime runtime, DuckPlayerState player)
        {
            if (runtime.CurrentEvent.EventType != DuckWorldEventType.PocketOfDriftwood
                || player.PocketDriftwoodAwarded
                || player.PlacedHelpfulTypes.Count < 3)
                return;

            player.PocketDriftwoodAwarded = true;
            player.DayEventTwigs++;
            player.TotalTwigs++;
            runtime.AddHistory(player.Id, player.Name + " gained 1 Twig from A Pocket of Driftwood.");
        }

        private static void FinishAdventure(
            DuckMatchRuntime runtime,
            DuckPlayerState player,
            bool wornOut,
            string reason)
        {
            player.HasFinishedDay = true;
            player.IsWornOut = wornOut;
            player.KnownNextPhysicalChipIds.Clear();
            player.SplashProtectionArmed = false;
            player.LogSlowdownPending = false;
            player.GuideProtectionAvailable = false;
            runtime.AddHistory(player.Id, player.Name + " " + reason + " at space " + player.Position + ".");
        }

        private static void ResolveNightWhenEveryoneFinished(DuckMatchRuntime runtime)
        {
            if (runtime.State.Phase == DuckPhase.Adventure && runtime.State.Players.All(player => player.HasFinishedDay))
                DuckNightResolver.Resolve(runtime.State, runtime.Rules);
        }

        private static IReadOnlyList<GameAction> EmptyActions()
        {
            return new ReadOnlyCollection<GameAction>(Array.Empty<GameAction>());
        }
    }
}
