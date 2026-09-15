namespace Quackies.Cli;

internal static class CliEntry
{
    private const string Usage = "Usage: Quackies.Cli [--profile ducks|classic] [profile options]";

    internal static int Run(string[] arguments)
    {
        if (arguments.Any(argument => argument is "--help" or "-h"))
        {
            ShowHelp();
            return 0;
        }

        try
        {
            var (profile, remaining) = SelectProfile(arguments);
            return profile switch
            {
                "ducks" => DuckCli.Run(remaining),
                "classic" => ClassicCli.Run(remaining),
                _ => throw new ArgumentException("--profile must be ducks or classic.")
            };
        }
        catch (ArgumentException exception)
        {
            Console.Error.WriteLine(exception.Message);
            Console.Error.WriteLine(Usage);
            return 2;
        }
    }

    private static (string Profile, string[] Remaining) SelectProfile(string[] arguments)
    {
        var profile = "ducks";
        var found = false;
        var remaining = new List<string>();
        for (var index = 0; index < arguments.Length; index++)
        {
            var argument = arguments[index];
            if (argument.StartsWith("--profile=", StringComparison.Ordinal))
            {
                if (found) throw new ArgumentException("Specify --profile only once.");
                profile = argument.Substring("--profile=".Length);
                if (profile.Length == 0) throw new ArgumentException("--profile requires ducks or classic.");
                found = true;
                continue;
            }
            if (argument == "--profile")
            {
                if (found) throw new ArgumentException("Specify --profile only once.");
                if (++index >= arguments.Length) throw new ArgumentException("--profile requires ducks or classic.");
                profile = arguments[index];
                found = true;
                continue;
            }
            remaining.Add(argument);
        }
        return (profile, remaining.ToArray());
    }

    private static void ShowHelp()
    {
        Console.WriteLine("Quackies command-line game");
        Console.WriteLine(Usage);
        Console.WriteLine();
        Console.WriteLine("The current ten-Day duck game is the default. Use --profile classic for the retained nine-round reference.");
        Console.WriteLine("Duck options: --seed N, --inspect, --demo-day, --demo-game, --save PATH, --no-save,");
        Console.WriteLine("              --continue, --new-game, --two-player");
        Console.WriteLine("Classic options: --seed N, --starting-rubies 0|1");
        Console.WriteLine("Use help during a duck game for its interactive commands.");
    }
}
