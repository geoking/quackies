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

### 3. Reusable artwork import

- Published the art catalogue, sprite import recipe and generated sprite metadata.
- Validation: Unity compiled and built the catalogue; all 102 original source
  image hashes are unchanged.

### 4. First fortune implementation and separate tests

- Published lifecycle hooks plus six preparation cards in a Core-only commit.
- Published seven focused fortune tests separately: gifts in the current bag,
  shared-supply exhaustion, stale choices, returned chips, tied rewards and unlocks.
- Validation: solution build has zero warnings/errors; full Core suite is 45/45.
- The partial fortune batch remains opt-in until the complete 24-card deck is ready.

### 5. Saved playable table

- Added the repeatable scene builder, clear-background camera, iPad layout,
  human/AI pots, legal-action controls, reference modal and restart.
- Corrected the human image pivot so droplet and chip markers align with the
  preserve-aspect artwork. Fresh iPad-resolution inspection verified droplet 0
  and a green chip on physical space 1. No missing scripts or camera warning.
- Verified Draw/Stop through the real scene's button callbacks. A purchase-button
  raycast and pointer-click handler bought green 1: coins 4 → 0, inventory 9 → 10,
  shared stock 13 → 12. Book open/close and Restart callbacks passed.
- Rebuilt the scene from Edit mode and verified one presenter and one camera.
- Unity compilation and the subsequent runtime console check reported no errors.
  Native mouse automation was unavailable; callback and hit-test evidence is
  distinct from a physical touch or native mouse test.

## In progress

- Improve shopping readability, add the live victory-point board and an enlarged
  opponent-pot view. Complete the other fortune cards and full-match audit.
- iOS export support is installed. The selected command-line developer tools do
  not include an iPhone SDK, so a signed device build is not currently verified.

## Next checkpoints

1. Repeatable scene builder and playable Unity round.
2. Complete fortune cards, rule tests and legacy-prototype cleanup.
3. Complete rules and deterministic full-match validation with fortune cards.
4. iPad layout, iOS export, and extensibility review.
