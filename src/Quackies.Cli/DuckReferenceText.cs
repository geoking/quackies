using Quackies.Core.Ducks.Definitions;

namespace Quackies.Cli;

internal static class DuckReferenceText
{
    internal static string Encounter(DuckEncounterDefinition encounter) => encounter.EncounterType switch
    {
        DuckEncounterType.Seeds => "Move 1. No additional ability or Exhaustion.",
        DuckEncounterType.Tailwind => $"Move {encounter.BaseMovement}. No additional ability.",
        DuckEncounterType.Signpost => "Move 2, then privately preview the next chip (up to two during Sunlit Signboards). Continue with that exact chip or settle; never select or reorder.",
        DuckEncounterType.Splash => "Move 1. The immediately next placed chip consumes protection; if it is an obstacle, suppress only its nuisance, not movement or Exhaustion.",
        DuckEncounterType.Reeds => $"Move 1 and immediately gain {encounter.TwigYield} Twig{(encounter.TwigYield == 1 ? "" : "s")}. Keep those Twigs if worn out.",
        DuckEncounterType.Companion => "Increase the active flock, then move min(active flock + 1, 4): the first moves 2, second 3, later Companions 4. Mud can reduce it. The largest safe positive flock gains +1 Sleep at one or +2 total at two or more; ties qualify.",
        DuckEncounterType.Wildflowers => "Move 1. Gain +2 Sleep at Night for each placed Wildflowers only after settling safely at a haven.",
        DuckEncounterType.FallenLog => "Move 1 and add 1 Exhaustion. Unless protected, halve the next helpful chip's movement, rounding up to at least 1; repeated Logs do not stack.",
        DuckEncounterType.MudPuddle => "Move 1 and add 1 Exhaustion. Unless protected, reduce the active Companion flock by 1, minimum 0.",
        DuckEncounterType.LoosePebbles => "Move 1 and add 1 Exhaustion. Unless protected, lose 1 Sleep if this remains the final occupied chip.",
        DuckEncounterType.Brambles => "Move 1 and add 1 Exhaustion. Unless protected, lose 1 Twig earned today if this remains the final occupied chip; banked Twigs are safe.",
        DuckEncounterType.GrumpyGoose => "Move 1 and add 1 Exhaustion. Unless protected, lower today's safe Exhaustion maximum to 4. One Goose enters each bag on Day 5.",
        _ => throw new ArgumentOutOfRangeException(nameof(encounter))
    };

    internal static string Event(DuckWorldEventType type) => type switch
    {
        DuckWorldEventType.RainSoftenedSeeds => "Every placed Seed gains +1 movement today.",
        DuckWorldEventType.SunlitSignboards => "Every placed Signpost previews up to two available chips in their fixed draw order today.",
        DuckWorldEventType.FriendlyGuide => "Each duck's first placed obstacle has its nuisance suppressed today; movement and Exhaustion still apply.",
        DuckWorldEventType.PocketOfDriftwood => "The first time a duck places three different helpful chip types today, it gains +1 Twig.",
        DuckWorldEventType.AllTuckedIn => "If both ducks finish safely at havens, each gains +2 Sleep.",
        DuckWorldEventType.HomeBeforeDark => "If both ducks finish safely, each gains +1 Sleep.",
        DuckWorldEventType.SharedSupper => "If both ducks place at least one Seed, each gains +1 Sleep; worn-out ducks include it before halving Sleep.",
        DuckWorldEventType.StillAir => "Every Tailwind moves half its normal total today, rounded up; a pending Log does not halve that Tailwind twice.",
        DuckWorldEventType.ThickMorningMist => "Signposts still move 2 but do not preview a chip today.",
        DuckWorldEventType.RestlessNight => "A duck finishing safely at a haven receives 1 less haven-related Sleep, minimum 0.",
        _ => throw new ArgumentOutOfRangeException(nameof(type))
    };
}
