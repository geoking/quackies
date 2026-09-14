namespace Quackies.Cli;

internal sealed class DuckCliOptions
{
    internal const string Usage = "Usage: Quackies.Cli --profile ducks [--seed integer] [--inspect] [--demo-day]";
    internal int Seed { get; private set; } = 42;
    internal bool InspectOnly { get; private set; }
    internal bool DemoDay { get; private set; }

    internal static bool Requested(string[] arguments) => arguments.Any(argument =>
        argument == "--profile" || argument.StartsWith("--profile=", StringComparison.Ordinal));

    internal static DuckCliOptions Parse(string[] arguments)
    {
        var options = new DuckCliOptions();
        for (var index = 0; index < arguments.Length; index++)
        {
            var parts = arguments[index].Split('=', 2);
            var name = parts[0];
            string Value()
            {
                if (parts.Length == 2 && parts[1].Length > 0) return parts[1];
                if (parts.Length == 2 || index + 1 >= arguments.Length)
                    throw new ArgumentException($"{name} requires a value.");
                return arguments[++index];
            }

            switch (name)
            {
                case "--profile":
                    if (Value() != "ducks") throw new ArgumentException("Use --profile ducks, or omit --profile for the classic reference.");
                    break;
                case "--seed":
                    if (!int.TryParse(Value(), out var seed)) throw new ArgumentException("--seed requires an integer.");
                    options.Seed = seed;
                    break;
                case "--inspect" when parts.Length == 1: options.InspectOnly = true; break;
                case "--demo-day" when parts.Length == 1: options.DemoDay = true; break;
                default: throw new ArgumentException($"Unknown duck option: {arguments[index]}.");
            }
        }
        if (options.InspectOnly && options.DemoDay) throw new ArgumentException("Choose either --inspect or --demo-day.");
        return options;
    }
}
