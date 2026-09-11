# Quackies

The duck-theme migration is on `codex/duck-game-milestone-0`.
See its [plan](docs/duck-migration/PLAN.md) and
[milestone status](docs/duck-migration/STATUS.md). The first M1 Unity style
preview passed technical checks but was rejected visually on 11 September 2026.
The user approved the duck and seed tokens and the three-biome style. The V4
board review adds an incomplete starting nest, unnumbered wells, coin/twig icon
rewards and eight visibly linked shelters using illustrated scenery and precise
static typesetting. The [board and review notes](docs/duck-migration/concepts/2026-09-11-v4/README.md)
remain outside Unity. The original playable game remains below; no M2 or
game-code change is authorized by the image review.

A Unity tabletop game for one human and one AI, targeting iPad mini landscape.
The complete base-game Set 1 fortune deck, Normal AI, board inspection, match
settings and dice results are implemented. The initial playable milestone is
complete, with a full nine-round Unity interaction check, native iPad-aspect
review and successful iOS export. See
[progress](docs/PROGRESS.md) and [next milestones](docs/NEXT_MILESTONES.md).
For the latest completed work, work in progress and known gaps, read the
[handoff](docs/HANDOFF.md).

## Open the playable scene

Open `unity/Quackies.Unity` with Unity **6000.6.0f1**. From Edit mode, invoke:

**Quackies → Build and Play Initial Scene**

That menu rebuilds and saves `Assets/Scenes/QuackiesInitialScene.unity`, then
enters Play mode. Stop Play mode before rebuilding. Draw or stop using the bottom
buttons; use the available actions to resolve choices, buy chips and continue.
Ingredient buttons open the supplied book artwork; the active fortune opens its
full card. Scoreboard and the rival pot open larger inspection views. Dice reopens
the current round’s results. Settings shows Normal AI and the starting-ruby option
(off by default in Unity); Apply & restart begins a match with that setup.
Restart begins a fresh match. Final-round coins convert at five per victory point.

## Projects

- `src/Quackies.Core`: platform-independent core library targeting `netstandard2.1`
- `src/Quackies.Cli`: command-line debug/test front end targeting `net10.0`
- `tests/Quackies.Core.Tests`: xUnit tests for the core library targeting `net10.0`
- `unity/Quackies.Unity`: saved scene, art catalogue, Unity views and scene builder

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

Add `-- --starting-rubies 0` to use the no-starting-ruby house rule. The CLI
defaults to the official one ruby, uses Normal AI, shows recent action history
and waits for the human to advance each round. Add `--seed 0` for a repeatable
fortune sequence. All 24 fortune cards are enabled in standard Core and CLI games.

After changing Core code, update the DLL used by Unity:

```sh
./tools/sync-unity-core.sh
```

Let Unity finish importing before using Play mode. Original artwork under
`Assets/Art/raw` is preserved; sprite cropping is stored in import metadata.

The project targets iPad in landscape. The live GameView render target and
Canvas were verified at 1133×744 (the same aspect as 2266×1488); see the
[recorded evidence](tools/validation/evidence/2026-09-10/README.md). The iOS export
succeeded with 0 errors and 5 warnings. Xcode compilation, signing, installation
and physical-device testing remain separate work.
