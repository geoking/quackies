using System;
using Quackies.Core.Bags;
using Quackies.Core.Players;
using Quackies.Core.Randomness;

var seed = Environment.TickCount;
var random = new SeededRandomSource(seed);
var player = new PlayerState(DefaultBagFactory.CreateStartingBag());

Console.WriteLine("Quackies draw loop");
Console.WriteLine($"Seed: {seed}");
Console.WriteLine();

while (true)
{
    ShowState(player);

    if (player.HasExploded)
    {
        PrintSummary(player, "Round ended because the cauldron exploded.");
        break;
    }

    if (player.Bag.IsEmpty)
    {
        PrintSummary(player, "Round ended because the bag is empty.");
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
        PrintSummary(player, "Round stopped.");
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
            PrintSummary(player, "Round ended because the cauldron exploded.");
            break;
        }
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine();
        Console.WriteLine(ex.Message);
        PrintSummary(player, "Round ended.");
        break;
    }
}

static void ShowState(PlayerState player)
{
    Console.WriteLine("Current state");
    Console.WriteLine($"Cauldron position: {player.CurrentCauldronPosition}");
    Console.WriteLine($"White chip total: {player.WhiteChipTotal}");
    Console.WriteLine($"Exploded: {FormatBool(player.HasExploded)}");
    Console.WriteLine($"Remaining bag count: {player.Bag.Count}");
    Console.WriteLine();
}

static void PrintSummary(PlayerState player, string reason)
{
    Console.WriteLine();
    Console.WriteLine(reason);
    Console.WriteLine($"Drawn tokens: {player.DrawnTokens.Count}");
    Console.WriteLine($"Final cauldron position: {player.CurrentCauldronPosition}");
    Console.WriteLine($"Final white chip total: {player.WhiteChipTotal}");
    Console.WriteLine($"Exploded: {FormatBool(player.HasExploded)}");
}

static string FormatBool(bool value)
{
    return value ? "yes" : "no";
}
