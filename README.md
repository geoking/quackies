# Quackies

Initial repository scaffold for Quackies.

## Projects

- `src/Quackies.Core`: platform-independent core library targeting `netstandard2.1`
- `src/Quackies.Cli`: command-line debug/test front end targeting `net10.0`
- `tests/Quackies.Core.Tests`: xUnit tests for the core library targeting `net10.0`
- `unity/Quackies.Unity`: placeholder for the future Unity project

## Commands

Build the solution:

```sh
dotnet build Quackies.sln
```

Run tests:

```sh
dotnet test Quackies.sln
```

Run the CLI:

```sh
dotnet run --project src/Quackies.Cli/Quackies.Cli.csproj
```
