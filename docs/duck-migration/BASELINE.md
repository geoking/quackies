# M0 repository baseline — 10 September 2026

## Branch and preserved work

The verified starting commit is `74e40cfe04995d813428c3ff8390461235928a44`, the
merge of the initial playable scene into main. Local HEAD and fetched origin/main
matched before creating `codex/duck-game-milestone-0`. The previous feature branch
is no longer the working baseline. The new branch is published to geoking/quackies.

One tracked modification predated M0:
`unity/Quackies.Unity/ProjectSettings/ProjectSettings.asset` removes the generated
InputSystem_Actions preloaded-assets entry, leaving an empty list. It remains
uncommitted and untouched. Its hash and exact patch are in
[workspace evidence](evidence/m0-workspace.json) and
[the recorded patch](evidence/pre-existing-settings.patch).

M0 changes documentation and branch-specific working instructions only. No Core,
CLI, Unity scripts, source images, scene files or serialized identities change.

## Current checks

| Check | Current evidence | What it establishes |
| --- | --- | --- |
| `dotnet build Quackies.sln` | Exit 0, 0 warnings, 0 errors; [output](evidence/m0-core-build.txt) | Current Core, CLI and test projects compile |
| `dotnet test Quackies.sln --no-build` | 129 passed, 0 failed, 0 skipped; [output](evidence/m0-core-tests.txt) | Existing automated suite passes on this baseline |
| CLI seed/settings smoke | Seed 0, starting rubies 0, `q` exits successfully; [output](evidence/m0-cli-smoke.txt) | Current executable accepts the documented settings and offers legal opening actions |
| Unity Editor status | Ready, stopped, not compiling; [result](evidence/m0-unity-editor_status.json) | Connector reaches the intended Editor/project |
| Active scene inspection | Saved initial scene, not dirty, one MatchPresenter; [result](evidence/m0-unity-eval.json) | Current scene and controller boundary are present |
| Captured console errors | 0 returned; [result](evidence/m0-unity-get_console_logs.json) | No errors in the connector's current captured buffer |
| Recompile status | Idle; [result](evidence/m0-unity-recompile_status.json) | No current compile operation; not a fresh compile test |

No pre-existing failure was found in these checks. A full Unity match, fresh
Editor recompile, iOS export and physical-device run were not repeated for this
documentation-only milestone. The earlier integration evidence under
`tools/validation/evidence/2026-09-10/` is historical, not a new M0 runtime run.

## Core, CLI, AI and persistence map

| Repository path | Actual role |
| --- | --- |
| `src/Quackies.Core/Quackies.Core.csproj` | Unity-independent netstandard2.1 rules assembly |
| `src/Quackies.Core/Match/MatchSession.cs` | Creates a match from random source, settings and optional RuleSet; exposes `GetSnapshot`, `GetLegalActions`, `Execute` |
| `src/Quackies.Core/Match/MatchView.cs` | Detached player/track/shop/encounter observations, own bag, starting bag, history, current event and winner data |
| `src/Quackies.Core/Match/GameAction.cs` | Core-issued Id, Kind, Label, ChoiceTitle, Color, Value and Cost |
| `src/Quackies.Core/Match/*PhaseHandler.cs` | Brewing, evaluation, shopping and ruby-spending rules and many display messages |
| `src/Quackies.Core/Rules/RuleSet.cs` | Immutable track, ingredient registry, finite shop definitions and fortune deck |
| `src/Quackies.Core/Rules/Ingredients/SetOneIngredients.cs` | Active Set 1 ingredient handlers |
| `src/Quackies.Core/Rules/Fortunes/` | All 24 fortunes, their stable IDs, titles, explanations and effects |
| `src/Quackies.Core/Rules/BoardTrack.cs` | Physical positions 0–53, last chip position 52, following-space reward lookup |
| `src/Quackies.Core/AI/NormalPolicy.cs` | Chooses from legal actions using only the observing player's information |
| `src/Quackies.Core/AI/BalancedPolicy.cs` | Compatibility wrapper around NormalPolicy |
| `src/Quackies.Cli/Program.cs` | net10 CLI using the same session and Normal AI |
| `tests/Quackies.Core.Tests/` | 19 test classes; 113 Fact/Theory methods expand to the 129 baseline cases |
| `src/Quackies.Core/Game/QuackiesGame.cs` | Retained prototype compatibility surface, not the current game engine |

`MatchSettings` currently contains only `StartingRubies`; its standard default is
1. The Unity next-match default is 0. The CLI accepts split or equals forms of
`--starting-rubies 0|1` and `--seed integer`; `q` quits. There is no help switch or
theme option yet. The CLI prints Core event/action/history strings alongside its
own headings and stat labels; its AI loop has a 64-action cap.

Active ingredients preserve the existing mechanics: white threshold chips,
orange plain movement, blue selection, red orange-dependent movement, yellow
last-white return, green final-two reward, purple count rewards and the
two-player black comparison. Their handler classes still use the classic names.
The shared duck vocabulary changes their explanation, not their effect registry.

Normal AI draws only when every remaining chip is safe. Flask use requires a
white last chip, a restored guaranteed-safe draw, and a remaining-bag fraction
above the round-dependent threshold (>50% on day 1, falling to >15% on day 9).
Shopping uses semantic colour/value data. Some choice decisions inspect stable
ID suffixes such as `:take-points`, `:take-coins` and `:remove-white-1`.

There is **no playable save/load or action-replay format**. History and RecentLog
are detached in-memory observations; they do not restore a session. Unity's
next-match setting also stays in process memory. Validation report files and
`resume-*` markers coordinate the UI probe, not a saved game. Scene/catalog YAML
stores authored presentation data and must retain its serialized bindings.

## Terminology coupling and M2 recommendation

Core supplies action labels/choice titles, event titles/descriptions, shop effect
text, history messages and die descriptions. Those strings originate in the
session, phase handlers, ingredient/fortune code and evaluation. `Token.ToString`
uses enum name/value. CLI, `MatchPresenter`, presentation views and the scene
builder add further labels. Updating Unity text alone would leave inconsistent
choices and history from Core.

Use one small immutable vocabulary/formatter keyed by existing semantic types
and IDs, applying it when messages are created. Keep the DTOs' existing display
fields consumable by both clients. Do not translate history by blind string
replacement or maintain independent client dictionaries. Preserve a compatible
classic default while adding duck presentation selection separately from rules.

Specific compatibility risks:

- `TokenColor` names and ordinal values participate in catalog serialization,
  asset paths and tests. Never globally rename or reorder them.
- Legal action IDs and monotonic choice IDs are opaque API identities. Preserve
  them, their option suffixes and player IDs `human` / `ai`.
- `QuackiesArtCatalog.GetFortuneCardSprite` currently looks up normalized titles,
  and the importer duplicates all 24 classic titles. Introduce a stable EventId
  mapping or preserve classic lookup separately before changing event titles.
- Existing tests assert some exact message strings (history, CLI/settings,
  Second Chance and other fortunes). Change display assertions deliberately;
  preserve semantic assertions and deterministic outcome comparisons.
- Keep MatchPresenter field names, scene references, catalog field names, .meta
  identities and validation object names stable or migrate them explicitly.
- A visual duck's result pose must not overwrite Core's permanent start or the
  next day's temporary catch-up position. The scored nest must not reset with
  the route. Use the detached observations as the source of those values.

## Unity checkout and boundaries

The connector reaches `127.0.0.1:7800`, Unity **6000.6.0f1**, at
`/Users/george/Repos/quackies/unity/Quackies.Unity`. It shares this branch's checkout;
do not point another worker at a different project and assume it controls this
Editor. No Editor mutations were needed for M0.

The active scene is `Assets/Scenes/QuackiesInitialScene.unity`, with roots
`Event System`, `Tabletop Background Camera` and `Quackies Table`. The target is
iOS. `EditorBuildSettings.asset` currently enables the initial scene and
SampleScene; future exports must choose their intended scenes explicitly.

| Path relative to the Unity project | Responsibility and migration seam |
| --- | --- |
| `Assets/Scripts/MatchPresenter.cs` | Owns the session, submits legal actions, paces Normal AI and renders snapshots |
| `Assets/Scripts/Presentation/PotView.cs` | Renders current pot and summary strings; duck trail needs a separate indexed layout |
| `Assets/Scripts/Presentation/ActionPanelView.cs` | Shows Core-issued actions; preserve IDs while changing displayed language |
| `Assets/Scripts/Presentation/FittedViewport.cs`, `SafeArea.cs`, `TableUi.cs` | Reusable 1133 × 744 layout and safe-area helpers |
| `Assets/Scripts/Presentation/ScoreboardView.cs`, `DiceResultsView.cs`, `MatchSettingsView.cs` | Existing score, resolved die and settings surfaces to adapt later |
| `Assets/Scripts/Art/QuackiesArtCatalog.cs` | Existing sprite/coordinate catalog; preserve classic mappings and .meta identity |
| `Assets/Editor/InitialSceneBuilder.cs` | Rebuilds the current scene, serializes bindings and updates build settings |
| `Assets/Editor/QuackiesArtImporter.cs` | Imports existing art and builds the classic catalog |
| `Assets/Plugins/Quackies.Core.dll` | Compiled Core boundary; update with `tools/sync-unity-core.sh` only when Core changes |

Existing Editor commands are **Quackies → Build Initial Scene** and
**Quackies → Build and Play Initial Scene**. They were discovered in source,
not invoked during M0. A separate M1 builder avoids changing the playable scene
or accidentally adding a style test to the shipping build list.

## Available art and agent tools

`image_gen.imagegen` is advertised and callable in this session, and the local
imagegen skill exists. No image was generated and no generation quota/output was
tested. M1 must read the skill and use the available generator for three assets;
no separately billed fallback was invoked. See [the asset brief](ASSET_BRIEF.md).

The existing `.codex/config.toml` and role files define Sol/high for Core,
Terra/high for Unity, and Luna/medium for support. They were inspected and kept.
M0 used one bounded Sol/high audit worker while the lead owned Git, Unity
read-only inspection and the plan. Future briefs stay small and disjoint, with
at most two workers and one Editor mutation owner.
