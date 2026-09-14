using System;
using System.Collections.Generic;
using System.Linq;
using Quackies.Core.Ducks.Definitions;

namespace Quackies.Core.Ducks.Runtime
{
    /// <summary>Collective, ordered Night scoring. No client computes or awards these values.</summary>
    internal static class DuckNightResolver
    {
        internal static IReadOnlyDictionary<string, DuckNightOutcome> Calculate(DuckMatchState state, DuckRuleDefinitions rules)
        {
            if (state.Players.Count == 0 || state.Players.Any(player => !player.HasFinishedDay || player.PlacedChips.Count == 0))
                throw new InvalidOperationException("Night scoring waits until every duck has drawn and finished.");
            if (state.Players.Any(player => player.IsSleepFrozen))
                throw new InvalidOperationException("This Night has already been resolved.");

            var worldEvent = rules.WorldEvent(state.WorldEventDeckDefinitionIds[state.CurrentEventIndex]).EventType;
            var allSafe = state.Players.All(player => !player.IsWornOut);
            var allHavens = state.Players.All(player => rules.BoardSpaceAt(player.Position).IsHaven);
            var allSeeds = state.Players.All(player => player.PlacedHelpfulTypes.Contains(DuckEncounterType.Seeds));
            var eventSleep = worldEvent == DuckWorldEventType.AllTuckedIn && allSafe && allHavens ? 2
                : worldEvent == DuckWorldEventType.HomeBeforeDark && allSafe ? 1
                : worldEvent == DuckWorldEventType.SharedSupper && allSeeds ? 1 : 0;
            var greatestSafeFlock = state.Players.Where(player => !player.IsWornOut).Select(player => player.ActiveFlock).DefaultIfEmpty(0).Max();
            var amounts = state.Players.Select(player => new NightAmounts(state.Day, player, rules, worldEvent, eventSleep, greatestSafeFlock)).ToArray();
            var bestSafeSleep = amounts.Where(amount => !amount.Player.IsWornOut).Select(amount => amount.FrozenSleep).DefaultIfEmpty(-1).Max();
            return amounts.ToDictionary(amount => amount.Player.Id, amount => amount.ToOutcome(
                !amount.Player.IsWornOut && amount.FrozenSleep == bestSafeSleep), StringComparer.Ordinal);
        }

        internal static void Resolve(DuckMatchState state, DuckRuleDefinitions rules)
        {
            // Calculate the complete cohort before awarding anything. Immediate Reeds/Pocket
            // Twigs are already banked; add only the printed amount less final Brambles here.
            var outcomes = Calculate(state, rules);
            foreach (var player in state.Players)
            {
                var outcome = outcomes[player.Id];
                player.TotalTwigs += outcome.PrintedTwigs - outcome.BramblesPenalty;
                player.FrozenSleep = outcome.FrozenSleep;
                player.RemainingSleep = outcome.FrozenSleep;
                player.IsSleepFrozen = true;
                player.PendingMostRestedStep = outcome.NextDayTemporaryStep == 1;
                player.LastNightOutcome = outcome;
                DuckFeatherAwards.Give(state, player, outcome.FeathersAwarded, "haven");
                state.History.Add(new DuckHistoryState(state.Day, player.Id,
                    $"{player.Name} rests at {player.Position}: {outcome.TotalTwigsEarned} Twigs today, {outcome.FrozenSleep} Sleep" +
                    (player.IsWornOut ? " after worn-out halving." : ".")));
                if (outcome.IsMostRested)
                {
                    state.PublicAwards.Add(new DuckPublicAwardState(state.Day, "most_rested", new[] { player.Id }, 0, 0, 0));
                    state.History.Add(new DuckHistoryState(state.Day, player.Id, $"{player.Name} is a Most Rested Duck."));
                }
            }
            state.Phase = DuckPhase.Night;
        }

        private sealed class NightAmounts
        {
            internal readonly DuckPlayerState Player;
            internal readonly int FrozenSleep;
            private readonly int _day, _printedSleep, _printedTwigs, _brambles, _flowers, _finalHaven, _restless,
                _eventSleep, _flock, _pebbles, _beforeWear, _totalTwigs, _feathers;

            internal NightAmounts(int day, DuckPlayerState player, DuckRuleDefinitions rules,
                DuckWorldEventType worldEvent, int eventSleep, int greatestSafeFlock)
            {
                Player = player;
                _day = day;
                var final = player.PlacedChips.Last();
                if (final.Position != player.Position) throw new InvalidOperationException("Rest must use the final occupied space.");
                var finalChip = player.Inventory.Single(chip => chip.PhysicalChipId == final.PhysicalChipId);
                var finalType = rules.Encounter(finalChip.DefinitionId).EncounterType;
                var space = rules.BoardSpaceAt(player.Position);
                _printedSleep = space.Sleep;
                _printedTwigs = space.Twigs;
                var grossTwigs = space.Twigs + player.DayReedsTwigs + player.DayEventTwigs;
                _brambles = finalType == DuckEncounterType.Brambles && !final.NuisanceSuppressed ? Math.Min(1, grossTwigs) : 0;
                _totalTwigs = grossTwigs - _brambles;
                var safeHaven = space.IsHaven && !player.IsWornOut;
                _flowers = safeHaven ? 2 * player.FlowersPlaced : 0;
                _finalHaven = safeHaven && day == DuckMatchSettings.StandardDays ? 2 : 0;
                var havenSleep = space.Sleep + _flowers + _finalHaven;
                _restless = safeHaven && worldEvent == DuckWorldEventType.RestlessNight ? Math.Min(1, havenSleep) : 0;
                _eventSleep = eventSleep;
                _flock = !player.IsWornOut && player.ActiveFlock > 0 && player.ActiveFlock == greatestSafeFlock
                    ? Math.Min(2, player.ActiveFlock) : 0;
                var grossSleep = havenSleep - _restless + _eventSleep + _flock;
                _pebbles = finalType == DuckEncounterType.LoosePebbles && !final.NuisanceSuppressed ? Math.Min(1, grossSleep) : 0;
                _beforeWear = grossSleep - _pebbles;
                FrozenSleep = player.IsWornOut ? _beforeWear / 2 : _beforeWear;
                _feathers = safeHaven ? space.Feathers : 0;
            }

            internal DuckNightOutcome ToOutcome(bool mostRested) => new DuckNightOutcome(
                _day, _printedSleep, _printedTwigs, Player.DayReedsTwigs, Player.DayEventTwigs,
                _brambles, _flowers, _finalHaven, _restless, _eventSleep, _flock, _pebbles,
                _beforeWear, FrozenSleep, _totalTwigs, _feathers, mostRested,
                mostRested && _day < DuckMatchSettings.StandardDays ? 1 : 0,
                _day == DuckMatchSettings.StandardDays ? FrozenSleep / 4 + (mostRested ? 1 : 0) : 0);
        }
    }

    /// <summary>One capability for every permanent Feather grant; never a spendable balance.</summary>
    internal static class DuckFeatherAwards
    {
        internal static void Give(DuckMatchState state, DuckPlayerState player, int amount, string source)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
            if (amount == 0) return;
            player.PermanentFeatherTrail += amount;
            state.PublicAwards.Add(new DuckPublicAwardState(state.Day, source, new[] { player.Id }, 0, 0, amount));
            state.History.Add(new DuckHistoryState(state.Day, player.Id, $"{player.Name} gains {amount} Feather(s) from {source}."));
        }
    }
}
