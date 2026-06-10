using System;
using Quackies.Core.Game;
using Quackies.Core.Randomness;
using Quackies.Core.Rewards;
using Quackies.Core.Rounds;

var seed = Environment.TickCount;
var random = new SeededRandomSource(seed);
var game = QuackiesGame.CreateSinglePlayer(random);

Console.WriteLine("Quackies draw loop");
Console.WriteLine($"Seed: {seed}");
Console.WriteLine();

while (true)
{
    ShowState(game.GetSnapshot());

    Console.Write("Press Enter to draw, s to stop, q to quit: ");
    var input = Console.ReadLine();

    if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Quitting.");
        break;
    }

    if (string.Equals(input, "s", StringComparison.OrdinalIgnoreCase))
    {
        PrintSummary(game.StopRound());
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
        var drawResult = game.DrawToken();
        PrintDrawResult(drawResult);

        var snapshot = game.GetSnapshot();

        if (snapshot.Phase == GamePhase.AwaitingExplosionRewardChoice)
        {
            snapshot = game.ApplyExplodedRewardChoice(PromptForExplodedChoice());
            PrintSummary(snapshot);
            break;
        }

        if (snapshot.Phase == GamePhase.RoundEnded)
        {
            PrintSummary(snapshot);
            break;
        }
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine();
        Console.WriteLine(ex.Message);
        break;
    }
}

static void ShowState(GameSnapshot snapshot)
{
    var player = snapshot.CurrentPlayer;
    var cauldron = player.Cauldron;
    var rewardSpace = snapshot.CurrentRewardSpace;

    Console.WriteLine("Current state");
    Console.WriteLine($"Cauldron position: {cauldron.CurrentPosition}");
    Console.WriteLine($"White chip total: {cauldron.WhiteChipTotal}");
    Console.WriteLine($"Exploded: {FormatBool(cauldron.HasExploded)}");
    Console.WriteLine($"Remaining bag count: {player.RemainingBagCount}");
    Console.WriteLine($"Reward space: position {rewardSpace.Position}, buying {rewardSpace.BuyingPower}, VP {rewardSpace.VictoryPoints}, ruby {FormatBool(rewardSpace.HasRuby)}");
    Console.WriteLine("Placed tokens:");

    if (cauldron.PlacedTokens.Count == 0)
    {
        Console.WriteLine("(none)");
    }
    else
    {
        foreach (var placedToken in cauldron.PlacedTokens)
        {
            Console.WriteLine($"#{placedToken.DrawIndex}: {placedToken.TokenText} at {placedToken.Position}");
        }
    }

    Console.WriteLine();
}

static void PrintDrawResult(DrawResult result)
{
    Console.WriteLine();
    Console.WriteLine($"Drawn token: {result.DrawnToken}");
    Console.WriteLine($"New cauldron position: {result.NewCauldronPosition}");
    Console.WriteLine($"White chip total: {result.WhiteChipTotal}");
    Console.WriteLine($"Exploded: {FormatBool(result.HasExploded)}");
    Console.WriteLine();
}

static void PrintSummary(GameSnapshot snapshot)
{
    var player = snapshot.CurrentPlayer;
    var cauldron = player.Cauldron;
    var reward = snapshot.PendingReward;
    var appliedReward = snapshot.AppliedReward;

    Console.WriteLine();
    Console.WriteLine(snapshot.RoundEndReason ?? "Round ended.");
    Console.WriteLine($"Drawn tokens: {cauldron.PlacedTokens.Count}");
    Console.WriteLine($"Final cauldron position: {cauldron.CurrentPosition}");
    Console.WriteLine($"Final white chip total: {cauldron.WhiteChipTotal}");
    Console.WriteLine($"Exploded: {FormatBool(cauldron.HasExploded)}");
    Console.WriteLine();
    Console.WriteLine("Reward summary");

    if (reward == null)
    {
        Console.WriteLine("No reward has been resolved.");
        return;
    }

    Console.WriteLine($"Reward position: {reward.Position}");
    Console.WriteLine($"Available buying power: {reward.BuyingPower}");
    Console.WriteLine($"Available victory points: {reward.VictoryPoints}");
    Console.WriteLine($"Ruby on space: {FormatBool(reward.HasRuby)}");
    Console.WriteLine($"Choice required: {FormatBool(reward.MustChooseVictoryPointsOrBuyingPower)}");

    if (appliedReward != null)
    {
        Console.WriteLine($"Applied buying power: {appliedReward.AppliedBuyingPower}");
        Console.WriteLine($"Applied victory points: {appliedReward.AppliedVictoryPoints}");
        Console.WriteLine($"Applied rubies: {appliedReward.AppliedRubies}");
    }

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
