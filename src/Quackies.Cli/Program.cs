using System;
using Quackies.Core.Bags;
using Quackies.Core.Cauldrons;
using Quackies.Core.Players;
using Quackies.Core.Randomness;
using Quackies.Core.Rewards;

var seed = Environment.TickCount;
var random = new SeededRandomSource(seed);
var player = new PlayerState(DefaultBagFactory.CreateStartingBag());
var track = DefaultCauldronTrackFactory.CreatePrototypeTrack();

Console.WriteLine("Quackies draw loop");
Console.WriteLine($"Seed: {seed}");
Console.WriteLine();

while (true)
{
    ShowState(player, track);

    if (player.Cauldron.HasExploded)
    {
        ResolveRound(player, track, "Round ended because the cauldron exploded.");
        break;
    }

    if (player.Bag.IsEmpty)
    {
        player.StopRound();
        ResolveRound(player, track, "Round ended because the bag is empty.");
        break;
    }

    Console.Write("Press Enter to draw, s to stop, q to quit: ");
    var input = Console.ReadLine();

    if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Quitting.");
        break;
    }

    if (string.Equals(input, "s", StringComparison.OrdinalIgnoreCase))
    {
        player.StopRound();
        ResolveRound(player, track, "Round stopped.");
        break;
    }

    if (!string.IsNullOrWhiteSpace(input))
    {
        Console.WriteLine("Unknown command.");
        Console.WriteLine();
        continue;
    }

    try
    {
        var result = player.DrawToken(random);

        Console.WriteLine();
        Console.WriteLine($"Drawn token: {result.DrawnToken}");
        Console.WriteLine($"New cauldron position: {result.NewCauldronPosition}");
        Console.WriteLine($"White chip total: {result.WhiteChipTotal}");
        Console.WriteLine($"Exploded: {FormatBool(result.HasExploded)}");
        Console.WriteLine();

        if (result.HasExploded)
        {
            ResolveRound(player, track, "Round ended because the cauldron exploded.");
            break;
        }
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine();
        Console.WriteLine(ex.Message);
        player.StopRound();
        ResolveRound(player, track, "Round ended.");
        break;
    }
}

static void ShowState(PlayerState player, CauldronTrack track)
{
    var rewardSpace = track.GetRewardSpace(player.Cauldron.CurrentPosition);

    Console.WriteLine("Current state");
    Console.WriteLine($"Cauldron position: {player.Cauldron.CurrentPosition}");
    Console.WriteLine($"White chip total: {player.Cauldron.WhiteChipTotal}");
    Console.WriteLine($"Exploded: {FormatBool(player.Cauldron.HasExploded)}");
    Console.WriteLine($"Remaining bag count: {player.Bag.Count}");
    Console.WriteLine($"Reward space: position {rewardSpace.Position}, buying {rewardSpace.BuyingPower}, VP {rewardSpace.VictoryPoints}, ruby {FormatBool(rewardSpace.HasRuby)}");
    Console.WriteLine("Placed tokens:");

    if (player.Cauldron.PlacedTokens.Count == 0)
    {
        Console.WriteLine("(none)");
    }
    else
    {
        foreach (var placedToken in player.Cauldron.PlacedTokens)
        {
            Console.WriteLine(placedToken);
        }
    }

    Console.WriteLine();
}

static void ResolveRound(PlayerState player, CauldronTrack track, string reason)
{
    var reward = CauldronRewardResolver.Resolve(player.Cauldron, track);
    var choice = reward.PlayerExploded ? PromptForExplodedChoice() : (ExplodedRewardChoice?)null;
    var appliedReward = player.ApplyEndRoundReward(reward, choice);

    Console.WriteLine();
    Console.WriteLine(reason);
    Console.WriteLine($"Drawn tokens: {player.Cauldron.PlacedTokens.Count}");
    Console.WriteLine($"Final cauldron position: {player.Cauldron.CurrentPosition}");
    Console.WriteLine($"Final white chip total: {player.Cauldron.WhiteChipTotal}");
    Console.WriteLine($"Exploded: {FormatBool(player.Cauldron.HasExploded)}");
    Console.WriteLine();
    Console.WriteLine("Reward summary");
    Console.WriteLine($"Reward position: {reward.Position}");
    Console.WriteLine($"Available buying power: {reward.BuyingPower}");
    Console.WriteLine($"Available victory points: {reward.VictoryPoints}");
    Console.WriteLine($"Ruby on space: {FormatBool(reward.HasRuby)}");
    Console.WriteLine($"Choice required: {FormatBool(reward.MustChooseVictoryPointsOrBuyingPower)}");
    Console.WriteLine($"Applied buying power: {appliedReward.AppliedBuyingPower}");
    Console.WriteLine($"Applied victory points: {appliedReward.AppliedVictoryPoints}");
    Console.WriteLine($"Applied rubies: {appliedReward.AppliedRubies}");
    Console.WriteLine($"Player total VP: {player.VictoryPoints}");
    Console.WriteLine($"Player rubies: {player.Rubies}");
    Console.WriteLine($"Buying power available this round: {player.BuyingPowerAvailableThisRound}");
}

static ExplodedRewardChoice PromptForExplodedChoice()
{
    while (true)
    {
        Console.Write("Pot exploded. Take v for victory points or b for buying power: ");
        var input = Console.ReadLine();

        if (string.Equals(input, "v", StringComparison.OrdinalIgnoreCase))
        {
            return ExplodedRewardChoice.TakeVictoryPoints;
        }

        if (string.Equals(input, "b", StringComparison.OrdinalIgnoreCase))
        {
            return ExplodedRewardChoice.TakeBuyingPower;
        }

        Console.WriteLine("Unknown choice.");
    }
}

static string FormatBool(bool value)
{
    return value ? "yes" : "no";
}
