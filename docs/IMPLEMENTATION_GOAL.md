# Initial playable Quackies

## Leading requirement: readable, adaptable architecture

Build a complete base-game, ingredient Set 1 match for one human and one AI,
presented in a single Unity scene for iPad mini. Architectural quality takes
priority over shortcuts: future ingredient sets and expansions must have clear
extension points without copying the game loop or putting rules in Unity views.

The implementation must separate:

- Immutable rules and component data from mutable match state.
- Phase orchestration from ingredient effects and fortune-card effects.
- Player commands and legal choices from presentation and AI decisions.
- Public player observations from hidden bag contents and random state.
- Unity scene construction, asset mapping, presentation, and input handling.

Prefer small cohesive types, explicit names, documented invariants, and concrete
extension examples. Avoid a universal scripting engine, unnecessary abstraction,
and one large controller containing every rule. New sets should register rules;
new expansions should compose additional rules and state through deliberate,
tested interfaces. Core remains netstandard2.1 and never references UnityEngine.

## Playable scope

All nine rounds; starting setup; Set 1 ingredients and two-player black book;
fortune cards; bag draws and choices; explosions; flask; rat tails; evaluation
and die; finite shop and purchase restrictions; rubies; droplet; unlocks; round
six and nine rules; persistent inventory; final scoring and ties; restart.
Authoritative rules and assets must resolve prototype assumptions, including
physical track indices versus printed coin values. No undocumented approximations
or claims of full compliance based only on the old prototype tests.

## Unity deliverables

Use existing Assets/Art/raw artwork without overwriting source images. Save a
complete scene with touch and mouse controls, safe-area handling, readable
landscape layout at 2266 x 1488, and a repeatable menu item:
`Quackies/Build and Play Initial Scene`. Re-running scene construction must not
duplicate objects. Keep editor tooling outside the player build.

Show a live victory-point board, reachable from a clear score button or a small
clickable board preview. Make the opponent's small pot clickable to inspect at a
readable size, with an obvious way back to the human pot. A camera must render the
background so the Game view has no "No cameras rendering" warning. Verify chip,
droplet and rat alignment on both pot sizes against the printed physical spaces.

## Latest playtest requirements

- Fix blank action panels, clipped controls and overflowing text before adding
  more visual features. All content must remain readable through suitable
  layout, wrapping, resizing or scrolling; truncation alone is not a fix.
- Populate the active fortune panel and its reference view when the complete
  deck is enabled. Keep CLI behavior in step with the same MatchSession API.
- Normal is the only current AI difficulty. It must decline any potentially
  explosive draw and use its flask only when a safe next draw is restored.
  Conserve flasks more in early rounds: the remaining-bag threshold falls from
  more than 50% in round one to more than 15% in round nine.
- Add a settings pane: Normal difficulty, a start-with-one-ruby option (off by
  default for the user's house rules), and the optional test-tube cauldron side.
  Settings apply to a new match; the Core's official default remains one ruby.
- Implement the test-tube variant's second droplet and every printed reward,
  including the choice of which droplet to advance. Do this before additional
  ingredient-book sets. Display the uncropped board when enabled.
- Add a separate scrollable AI-history pane with the whole match history,
  including draws, flask use, stops, explosions and purchases.
- Replace the live bag display with a fixed starting-game bag reference.
  Never show remaining bag composition to the human through the interface.
- Prioritize the live scoreboard and clickable opponent pot with clear return
  controls alongside the current visual bugs.

## Evidence required before completion

- Focused tests for rules, invalid actions, state ownership, and extension points.
- Deterministic complete-match simulations with legal AI decisions.
- Core/CLI build and test results; Unity compilation and runtime checks.
- Visual inspection and interaction at the iPad mini aspect ratio.
- Restart and repeatable scene construction verified.
- iOS export attempted when toolchain support is present; device validation
  reported separately and only claimed if actually performed.
- Rules sources, architecture, extension instructions, and run instructions.

## Working notes

### Usage-conscious execution

The full playable scope above is unchanged. Follow the model and delegation
policy in AGENTS.md. Implement in reviewable milestones: architecture contract;
playable round; complete rules in bounded batches; full-match Unity integration;
extensibility and correctness audit. Astra reviews milestones, Sol implements
core rules, Terra implements Unity, and Luna handles narrow support assignments.
Use at most two concurrent workers, no nested delegation, and concise handoffs.

### Visible progress on GitHub

The user authorizes regular commits and pushes to geoking/quackies on
`codex/initial-playable-scene`. Commit each coherent Core change, then its unit
tests separately, and each compiling Unity change separately. Explain what
changed and why in each commit. Track completed work, validation, and the next checkpoint in
docs/PROGRESS.md. The lead owns staging and pushes; workers own bounded code
changes. Keep main and unrelated pre-existing modifications untouched.

Initial inspection: the editor is connected to Unity 6000.6.0f1. The repository
contains a draw/stop prototype and a prebuilt Core DLL. Package manifest and lock
file changes existed before this work and must be preserved. The initial engine
has placeholder track rewards and does not yet implement a full match.
