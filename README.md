# Quackies

A tabletop duck adventure for one human versus Normal AI. Explore by Day,
rest and shop in your dreams by Night, and finish ten Days with the most Twigs.
The current game has 43 spaces across wetlands, meadow and wasteland.

**The complete duck game is playable in the CLI.** M4 and the M4.5 AI/balance
review are complete. New games use the approved Tailwind prices of 4/8/12 Sleep
and Reeds prices of 8/14/20. Existing saves keep their original prices.
The accepted Unity board is still a visual proof; connecting it to the duck
rules is M5, which has not started.

## Play now

Install the **.NET 10 SDK**, open a terminal in this repository, and run:

```sh
dotnet run --project src/Quackies.Cli --configuration Release
```

Choose a listed action number to Explore, Settle down or buy a chip. Type `help`
for commands. `board`, `tokens`, `event`, `bag` and `shop` let you inspect the
information you need without advancing play. `q` quits; actions are autosaved.
If a save exists, the next launch offers Continue or a new game.

[How to play the CLI](docs/CLI_GUIDE.md) covers the Day/Night loop, commands,
rewards, saves and developer options. The CLI runs without Unity.

## Build and check

```sh
dotnet build Quackies.sln --configuration Release
dotnet test Quackies.sln --configuration Release
```

For a reproducible Normal-versus-Normal demonstration:

```sh
dotnet run --project src/Quackies.Cli --configuration Release -- --seed 42 --demo-game
```

Demonstrations do not autosave unless requested. Use `--help` for launch options.

## Find your way around

| Location | Responsibility |
| --- | --- |
| `src/Quackies.Core` | Unity-independent rules, state, observations, AI and save data |
| `src/Quackies.Cli` | Terminal input, presentation and local save files |
| `tests/Quackies.Core.Tests` | Duck and classic rules, AI, persistence and CLI integration tests |
| `tools/Quackies.Evaluation` | Reproducible full games driven through the real Core API |
| `unity/Quackies.Unity` | Accepted duck artwork/layout proof and retained classic playable scene |
| `docs` | Player guide, code map, specifications, plans and validation evidence |

Start with the [codebase map](docs/CODEBASE_MAP.md) for the folder structure and
how commands, rules, AI and saves fit together. The
[architecture](docs/ARCHITECTURE.md) records the boundaries and extension rules.

The [current plan](docs/duck-migration/PLAN.md),
[handoff](docs/HANDOFF.md) and [progress log](docs/PROGRESS.md) track the work.
The [rules recap](docs/duck-migration/RULES_AT_A_GLANCE.md),
[board and shop](docs/duck-migration/v1/BOARD_AND_SHOP.md) and
[World Events](docs/duck-migration/v1/WORLD_EVENTS.md) define gameplay.

## Unity and the classic reference

The target is iPad mini in landscape. The Unity project uses **6000.6.0f1**.
The accepted `DuckLayoutProof` is a fixed-data visual scene, documented in the
[M3 closeout](docs/duck-migration/m3-closeout/README.md). It is not yet bound to
the new duck Core. M5 will connect Adventure and Dream views, then test the
human-versus-AI experience.

The original nine-round Quacks-inspired game remains a tested reference. Run it
with `--profile classic`, or open Unity's **Quackies → Build and Play Initial
Scene**. Its original rules, Unity checks and iOS export are recorded in the
[completed baseline](docs/IMPLEMENTATION_GOAL.md). They do not establish a
connected duck-game build or physical-device test.

Updating the Unity Core DLL is an explicit integration step using
`tools/sync-unity-core.sh`, outside Play mode; see the architecture guide. A CLI
build does not sync or rebuild Unity. Original art under `Assets/Art/raw` is
preserved.
