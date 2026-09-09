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

### 6. Match settings and complete history

- Published immutable zero/one starting-ruby settings, fixed nine-chip starting
  reference and full actor-attributed history. CLI uses the same setup and Normal
  AI, displays history, handles EOF and leaves round advancement to the human.
- Published tests separately: 11 focused cases and the full 71-test Release suite
  pass, including real CLI processes and retained history beyond 80 entries.

### 7. Inspectable scoreboard and opponent pot

- Published the supplied `board.png` with live scores, round marker, player counter
  art, separate tied counters and correct 50-point laps. Zero starts in the seal
  book rather than covering a numbered score space.
- Added a header Scoreboard button and a tappable rival pot / View CPU pot button.
  The full-screen pot continues updating during AI play; both screens have Back.
- Validation: rebuilt saved scene, inspected iPad mini captures, scoreboard
  raycast/pointer callback, open/back callbacks, live CPU updates and score laps.
  No missing scripts, compilation/runtime errors, or text overflow in the
  inspected CPU view. Original image bytes remain unchanged. These are Editor
  checks; physical touch/device validation is separate.

### 8. Stable controls and current match settings

- User checkpoint783cad3 preserved fixed Draw/Stop/Flask positions, flask artwork,
  fixed starting-bag reference and nine rat-fortune tests. Editor pointer-handler
  checks verified draws, flask return and stopping without moving controls.
- Settings paneb9967eb shows Normal AI and current versus next starting-ruby setup.
  House default is off; toggling preserves the current game until Apply & restart.
  Applying starts both players with the selected ruby count. Unity compiled cleanly.
- The user explicitly deferred test tubes and the separate AI-history pane.

### 9. Preview fortunes and observable dice

- Rat cards7c59849 and interactive cards02cd162 bring implementation coverage to
  21 of 24 cards. The latter adds Less Is More, An Opportunistic Moment and
  Schadenfreude through non-destructive previews and atomic supply exchanges.
- Typed die observations6a4294c expose face, recipient, reason, applied outcome and
  immutable history. Tests556a3a9 cover all six faces, tied eligibility, explosions,
  unavailable rewards, previews and stock changes: 18 focused cases, 98 total pass.
- Unity checkpoint2b093f3 displays actual Core dice results with supplied art,
  recipient/reason, round labels, Continue and current-round review. Both-exploded
  rounds have an explicit no-bonus explanation. Includes Core DLL6a4294c.
- Validation: Unity compiled without errors, scene rebuilt, and an Editor bonus
  result showed AI Orange 1 with the correct pumpkin face. Continue only closes
  the overlay. Source dice image bytes are unchanged. Physical-device evidence
  remains separate and has not been obtained.

## In progress and next checkpoints

1. Complete A Second Chance, Well Stirred and Strong Ingredient with focused
   regressions. The default fortune deck remains empty until all 24 are ready.
2. Enable the full deck and extend complete-match Normal AI coverage beyond the
   existing 112 matches across 14 cards. Keep CLI behavior aligned with Core.
3. Sync the published Core into Unity; verify fortune artwork/choices, dice,
   settings, all nine rounds, restart/rebuild and readable iPad mini layout.
4. Review architecture/legacy prototype and attempt supported iOS export. Unity
   iOS support is installed; the selected developer tools previously lacked an
   iPhone SDK. Editor success does not establish a signed build or device test.

Usage reached 95% during the September 9 review. Source, tests and Unity changes
above were separately committed and pushed; see HANDOFF.md before resuming.
The milestone remains incomplete. Main has not been merged.
