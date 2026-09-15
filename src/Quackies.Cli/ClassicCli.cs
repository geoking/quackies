using Quackies.Core.AI;
using Quackies.Core.Match;
using Quackies.Core.Randomness;

namespace Quackies.Cli;

internal static class ClassicCli
{
    private const string Usage = "Usage: Quackies.Cli --profile classic [--starting-rubies 0|1] [--seed integer]";

    internal static int Run(string[] arguments)
    {
        try
        {
            var seed = Environment.TickCount;
            var startingRubies = MatchSettings.Standard.StartingRubies;
            for (var index = 0; index < arguments.Length; index++)
            {
                var parts = arguments[index].Split('=', 2);
                string Value()
                {
                    if (parts.Length == 2 && parts[1].Length > 0) return parts[1];
                    if (parts.Length == 2 || ++index >= arguments.Length)
                        throw new ArgumentException(parts[0] + " requires a value.");
                    return arguments[index];
                }

                switch (parts[0])
                {
                    case "--seed":
                        if (!int.TryParse(Value(), out seed)) throw new ArgumentException("--seed must be an integer.");
                        break;
                    case "--starting-rubies":
                        var value = Value();
                        if (value is not ("0" or "1")) throw new ArgumentException("--starting-rubies must be 0 or 1.");
                        startingRubies = int.Parse(value);
                        break;
                    default: throw new ArgumentException("Unknown classic option: " + arguments[index] + ".");
                }
            }

            var session = MatchSession.Create(new SeededRandomSource(seed), new MatchSettings(startingRubies));
            var opponent = new NormalPolicy();
            Console.WriteLine($"Quackies classic reference · nine-round Set 1 match · seed {seed} · starting rubies {startingRubies}");
            Console.WriteLine("Choose an action number, or q to quit.\n");

            while (session.GetSnapshot("human").Phase != MatchPhase.Finished)
            {
                AdvanceOpponent(session, opponent);
                var view = session.GetSnapshot("human");
                Show(view);
                var actions = session.GetLegalActions("human");
                if (actions.Count == 0)
                {
                    Console.WriteLine("Waiting for the opponent.\n");
                    continue;
                }
                for (var index = 0; index < actions.Count; index++)
                    Console.WriteLine($"{index + 1}. {actions[index].Label}");
                Console.Write("> ");
                var input = Console.ReadLine();
                if (input == null) { Console.WriteLine("Input closed. Quitting."); return 0; }
                if (input.Equals("q", StringComparison.OrdinalIgnoreCase)) return 0;
                if (!int.TryParse(input, out var selected) || selected < 1 || selected > actions.Count)
                {
                    Console.WriteLine("Choose one of the listed numbers.\n");
                    continue;
                }
                session.Execute("human", actions[selected - 1]);
            }

            var final = session.GetSnapshot("human");
            Show(final);
            Console.WriteLine("Winner: " + string.Join(" & ", final.WinnerIds));
            return 0;
        }
        catch (ArgumentException exception)
        {
            Console.Error.WriteLine(exception.Message);
            Console.Error.WriteLine(Usage);
            return 2;
        }
    }

    private static void AdvanceOpponent(MatchSession session, IPlayerPolicy policy)
    {
        for (var step = 0; step < 64; step++)
        {
            var actions = session.GetLegalActions("ai");
            if (actions.Count == 0 || actions.All(action => action.Kind == GameActionKind.NextRound)) return;
            session.Execute("ai", policy.Choose(session.GetSnapshot("ai"), actions));
        }
        throw new InvalidOperationException("Opponent exceeded the action safety limit.");
    }

    private static void Show(MatchView view)
    {
        Console.WriteLine($"Round {view.Round}/9 · {view.Phase}");
        if (!string.IsNullOrEmpty(view.EventTitle)) Console.WriteLine($"Fortune: {view.EventTitle}");
        foreach (var player in view.Players)
            Console.WriteLine($"{player.Name}: {player.VictoryPoints} VP, {player.Rubies} rubies, pot {player.Position}, white {player.WhiteTotal}, bag {player.BagCount}" +
                (player.Exploded ? " · EXPLODED" : player.Stopped ? " · stopped" : string.Empty));
        foreach (var entry in view.History.TakeLast(5))
            Console.WriteLine($"  R{entry.Round} {(string.IsNullOrEmpty(entry.ActorId) ? "match" : entry.ActorId)}: {entry.Message}");
        Console.WriteLine();
    }
}
