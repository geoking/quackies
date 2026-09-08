using Quackies.Core.AI;
using Quackies.Core.Match;
using Quackies.Core.Randomness;

var seed = Environment.TickCount;
var session = MatchSession.Create(new SeededRandomSource(seed));
var opponent = new BalancedPolicy();

Console.WriteLine($"Quackies · nine-round Set 1 match · seed {seed}");
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
    Console.WriteLine();
}
