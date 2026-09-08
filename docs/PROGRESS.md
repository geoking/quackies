# Quackies progress

Full scope and acceptance criteria: [implementation goal](IMPLEMENTATION_GOAL.md).

## Completed checkpoints

### 1. Architecture and execution agreement

- Set readable, expandable architecture as the leading requirement.
- Preserved the full nine-round base-game Set 1 human-versus-AI scope.
- Added project model defaults and Sol, Terra, and Luna worker definitions.
- Limited work to two concurrent workers without nested delegation.
- Established small commits and pushes on `codex/initial-playable-scene`.
- Validation: reviewed the brief and configuration; documentation checkpoint
  contains no runtime changes and makes no claim that the game is playable.

### 2. Compiling match foundation

- Added a Unity-independent nine-round session with separate brewing, evaluation,
  shopping and ruby-spending handlers, immutable observations and legal actions.
- Added the physical 54-space board, Set 1 ingredient handlers, finite chip stocks,
  prices and unlocks, flask, explosions, round-six white chip and round-nine
  simultaneous draw commitments. Shopping follows start-player order.
- Added an observation-only AI policy and moved the CLI onto the match API.
- Protected chained choices against stale commands and froze evaluation rewards
  before droplet bonuses can change an empty pot's position.
- Validation: solution build passed with zero warnings/errors; all 38 tests passed,
  including 128 complete deterministic matches across 64 seeds without fortunes.
- This checkpoint establishes the match foundation. The default fortune deck is
  still empty; it is **not yet the complete base game**. Fortune-card implementation,
  broader rule tests and legacy-prototype cleanup are the next Core milestone.

## In progress

- Unity compiles successfully with the new Core DLL. The scene builder has saved
  and opened `QuackiesInitialScene` in Play Mode using the supplied artwork.
- Visual inspection found layout/marker alignment issues being corrected before
  publishing the Unity milestone. Scene reload, controls and iPad layout are under
  verification; this progress entry does not claim those checks are complete.
- iOS export support is installed. The selected command-line developer tools do
  not include an iPhone SDK, so a signed device build is not currently verified.

## Next checkpoints

1. Repeatable scene builder and playable Unity round.
2. Complete fortune cards, rule tests and legacy-prototype cleanup.
3. Complete rules and deterministic full-match validation with fortune cards.
4. iPad layout, iOS export, and extensibility review.
