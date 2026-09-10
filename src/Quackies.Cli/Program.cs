using Quackies.Core.AI;
using Quackies.Core.Match;
using Quackies.Core.Randomness;

var seed = Environment.TickCount;
int startingRubies;
try
{
    startingRubies = ReadStartingRubies(args);
    seed = ReadSeed(args, seed);
}
catch (ArgumentException exception)
{
    Console.Error.WriteLine(exception.Message);
    Console.Error.WriteLine("Usage: Quackies.Cli [--starting-rubies 0|1] [--seed integer]");
    Environment.ExitCode = 2;
    return;
}
var session = MatchSession.Create(new SeededRandomSource(seed), new MatchSettings(startingRubies));
var opponent = new NormalPolicy();

Console.WriteLine($"Quackies · nine-round Set 1 match · seed {seed} · starting rubies {startingRubies}");
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
    if (input == null)
    {
        Console.WriteLine("Input closed. Quitting.");
        return;
    }
    if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase)) return;
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

static void AdvanceOpponent(MatchSession session, IPlayerPolicy policy)
{
    for (var step = 0; step < 64; step++)
    {
        var actions = session.GetLegalActions("ai");
        if (actions.Count == 0) return;
        if (actions.All(action => action.Kind == GameActionKind.NextRound)) return;
        session.Execute("ai", policy.Choose(session.GetSnapshot("ai"), actions));
    }
    throw new InvalidOperationException("Opponent exceeded the action safety limit.");
}

static void Show(MatchView view)
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

static int ReadStartingRubies(string[] arguments)
{
    for (var index = 0; index < arguments.Length; index++)
    {
        const string prefix = "--starting-rubies=";
        if (arguments[index].StartsWith(prefix, StringComparison.Ordinal))
            return ParseStartingRubies(arguments[index].Substring(prefix.Length));
        if (arguments[index] == "--starting-rubies")
        {
            if (index + 1 >= arguments.Length)
                throw new ArgumentException("--starting-rubies requires a value of 0 or 1.");
            return ParseStartingRubies(arguments[index + 1]);
        }
    }
    return MatchSettings.Standard.StartingRubies;
}

static int ParseStartingRubies(string value)
{
    if (value == "0" || value == "1") return int.Parse(value);
    throw new ArgumentException("--starting-rubies must be 0 or 1.");
}

static int ReadSeed(string[] arguments, int fallback)
{
    for (var index = 0; index < arguments.Length; index++)
    {
        const string prefix = "--seed=";
        if (arguments[index].StartsWith(prefix, StringComparison.Ordinal))
            return ParseSeed(arguments[index].Substring(prefix.Length));
        if (arguments[index] == "--seed")
        {
            if (index + 1 >= arguments.Length) throw new ArgumentException("--seed requires an integer value.");
            return ParseSeed(arguments[index + 1]);
        }
    }
    return fallback;
}

static int ParseSeed(string value)
{
    if (int.TryParse(value, out var seed)) return seed;
    throw new ArgumentException("--seed must be an integer.");
}
