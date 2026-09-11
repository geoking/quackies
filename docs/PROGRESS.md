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

### 10. Final-round scoring and Well Stirred

- Published final-round source14cecc3 and testsfe02c97. Safe pots already converted
  coins automatically and skipped shopping. Exploded final pots now automatically
  receive the better of printed VP or floor(coins / 5), preserving the rule that
  explosion earns only one reward. Ten focused tests verify rounding, no shopping,
  no double conversion and unchanged earlier-round choices.
- Published Well Stirred7993bca and tests5da103a. The first placed white, including
  one selected through blue, can return without spending the flask; redrawing does
  not repeat the ability. Three focused tests pass. Twenty-two cards are implemented.
- Normal AI coverage now includes every available fortune batch plus 32 complete
  mixed-deck matches. All seven policy tests pass; further final cards remain
  subject to the same simulations when added.
- A bounded publisher-rule audit found an additional full-pot blue-selection edge:
  a blue chip reaching physical space52 must not queue another placement. This
  fix and its regression tests are assigned with the remaining two fortune cards.
- Reconfirmed Unity connection to Quackies.Unity6000.6.0f1, stopped/ready, no current
  console errors. Current settings/dice views pass text overflow checks. These
  checks precede the final fortune DLL integration.

### 11. Strong Ingredient, full-pot rules and fortune readability

- Stronge3c9bba/testseafab38 adds protected final selection in start-player order,
  with full/empty/exploded exclusions. Ten final-fortune tests passed;27 combined
  fortune/scoring/AI checks passed. Audit identified a follow-up: its own deferred
  ingredient bonus must also be suppressed under the publisher wording.
- Full-pot fix45991dd/testsa210bca prevents direct or nested blue selections after
  reaching52. Two targeted regressions passed and retain spoon scoring.
- Captiona41f413 separates the active fortune title from the supplied image.
  Unity compiled, rebuilt and the full-reference pointer callback passed.
- Verified composited die capture shows round1 AI Orange1 reward with matching
  pumpkin face and Continue. Root spotted a wrapped SHOPPING header behind it;
  a bounded header fix is pending review. No physical-device claims.
- A Second Chance, full deck activation and final Unity integration remain open.

### 12. Complete fortune deck and Unity integration

- Strong deferred-effect correction dd0c274 / tests 67ae4ba suppresses the
  selected chip’s own green/purple/black action; fourteen card regressions pass.
- A Second Chance 5a89d48 / tests 5efc452 adds protected opening placements and
  a one-time restart after ingredient choices finish, with a dedicated snapshot.
- Standard deck 81e280d enables all 24 cards. CLI 3e0f638 supports reproducible
  seeds; tests 0bcb742 cover uniqueness, atlas order and nine distinct reveals.
  The full solution passes 129 tests with zero build warnings/errors. Normal AI
  coverage includes 192 single-card matches and 32 mixed-deck matches.
- Published CLI smoke with seed 0 reveals Rat Infestation, runs legal Normal AI
  decisions, offers human Draw/Stop and exits cleanly on q.
- Header 61c9f33 keeps Shopping/Round Complete on one line in verified iPad-aspect
  captures. Final Core DLL is synced into Unity; recompile and scene rebuild pass.
- At this checkpoint, the full nine-round Unity pointer interaction probe and
  iOS export remained pending; those items were completed in the 10 September
  validation recorded above. No physical-device validation or main merge is
  claimed.

### 13. Initial playable milestone validation (10 September 2026)

- The complete Set 1 deck is enabled by default. The Core suite last confirmed
  129 tests passing on 10 September, and final-round scoring remains automatic.
  The Unity continuation fix after the 48-action cap is covered by the full-match
  validation, not counted as a Core test. Test-tube rules and the separate
  AI-history pane remain deferred to a future goal.
- The reusable `tools/validation/QuackiesUnityFullMatchProbe.cs` harness
  completed a nine-round live Unity match on the scene published in b2861ad using
  pointer callbacks and the Core Normal driver. Human scored 43 VP and AI
  scored 42 VP; all nine fortunes were distinct.
- The durable report records successful settings restart, dice results, fortune
  references, readable choice labels, scoreboard/opponent-pot navigation and
  final-score presentation. Runtime console evidence is scoped to 0 new errors
  since cursor 4; it is not a general clean-console claim.
- Canonical evidence is tracked under `tools/validation/evidence/2026-09-10/`:
  the full report, native and resized screenshots, and the extracted iOS build
  summary. Ignored local Temp reports are supplementary, not the durable record.
- The saved-scene and continuation-fix validation is published in b2861ad
  (with d40d4e4 as its source scene checkpoint), and the export serialization is
  published in a48e0c8. The iOS export succeeded to
  `unity/Quackies.Unity/Builds/iOS` for the initial
  scene, with 0 errors and 5 Unity warnings. This is export evidence only; no
  signed build, installation or physical-device test is claimed.
- The native GameView capture target and Canvas were verified at 1133×744,
  the same aspect ratio as 2266×1488. All primary-button corners fit inside the
  viewport, and the native capture was visually reviewed. The connector's
  Screen.width/height reflected the Editor GUI surface, explaining the earlier
  dimension discrepancy; `viewport-check.json` records the authoritative target.
- Unity recompile status reports completed, failed=false, no errors. The final
  acceptance review passed for the agreed initial playable scope. Test tubes,
  AI-history UI and device validation remain future work; main is not merged.

### 14. Duck migration M0 — baseline and plan (10 September 2026)

- The initial playable branch was subsequently merged into main as `74e40cf`.
  Created `codex/duck-game-milestone-0` from that verified current baseline,
  preserving the pre-existing Unity ProjectSettings preload removal.
- Published the duck migration plan and three-asset M1 brief in `bc55369`.
  The plan incorporates the design chat's Revision 4 vocabulary and game identity,
  a winding trail, duck as permanent start, temporary lily-pad assistance and
  the distinction between a daily resting spot and persistent scored nest.
- Current baseline: `dotnet build Quackies.sln` passed with 0 warnings/errors;
  `dotnet test Quackies.sln --no-build` passed all 129 cases. The CLI smoke with
  seed 0 and starting rubies 0 displayed legal actions and exited successfully.
- Unity read-only inspection verified 6000.6.0f1, the correct shared checkout,
  saved initial scene, one MatchPresenter, iOS target and no captured console
  errors. No fresh Unity compile, full match or export was needed for M0.
- The audit records Core-originated text, enum/action identity and fortune-title
  artwork coupling. There is no playable save/replay format. The recommended
  shared vocabulary preserves identifiers and rules while serving both clients.
- M0 is complete and paused for the user's reaction. No duck art, gameplay or
  later milestone has been implemented. Current evidence and next task are in
  [duck-migration/STATUS.md](duck-migration/STATUS.md).

### 15. Duck migration M1 — original art and Unity style test (10 September 2026)

- After the user accepted M0, generated exactly three original assets: calm
  wetland playmat, happy duck and seed encounter. Native outputs were copied
  unchanged; prompts, dimensions, alpha checks and stable Unity metadata are
  published with the art in `ff2c41b`.
- Published the separate DuckStyleTestScene and builder in `42e51b3`. It shows
  all 54 indexed spaces on a six-row winding trail with five rounded bends,
  fixed seed examples, a permanent starting duck, separate resting preview and
  placeholder nest/resources. Clean code-native ellipses and 14pt indices support
  readability at the 1133 × 744 iPad mini aspect.
- Unity compilation completed without errors; the final captured console had
  no messages. Repeat construction retained 54 unique cells without duplicates.
  All cell bounds fit the native viewport and TMP reported zero text overflows.
- Real EventSystem pointer callbacks exercised Explore and Reset. Three
  seeds/rest space 4 changed to five/rest space 6, then reset. Duck space 0 and
  Twigs 12 stayed fixed; next-space rewards come from BoardTrack.Standard().
- The final native screenshot, QA and checkpoint hashes are tracked in
  [duck-migration/evidence/m1](duck-migration/evidence/m1/README.md). The preview
  was left in Play mode at reset for the user's review.
- Core, CLI, tests, original playable scene, raw art, Core DLL and build scene
  list match the M0 baseline. The pre-existing ProjectSettings modification is
  preserved and excluded from commits. No new full-match run or iOS/device test
  is claimed for this visual-only milestone; the 129-test M0 result is unchanged
  historical evidence.
- M1 is complete and paused for the user's style reaction. M2 terminology,
  additional artwork and the later shelter experiment have not started.

### 16. M1 art redirection — image review (11 September 2026)

- The user rejected the V1 visual direction and requested tangible tabletop
  duck/encounter tiles and an illustrated board across three biomes. Revised
  the plan, art brief and current handoff in `d0ac497`; V1 technical evidence
  remains historical and is not recorded as style acceptance.
- Generated and inspected three selected concept sheets: four distinct player
  duck tiles, three seed-category tile silhouettes, and a board travelling from
  a pleasant pond through lush meadow to wasteland with rare cosy refuges.
- Used three built-in image-generation calls and three targeted edits. Cleaned
  the seed background and clarified the board path and pond starting point.
  Native selected PNGs, exact prompts and inspection hashes are saved in
  [concepts/2026-09-11](duck-migration/concepts/2026-09-11/README.md).
- Captured the qualitative biome reward idea without inventing values or
  changing rules. Exact mapping and balance need later specification and review.
- No Unity import or Editor call occurred. Source, Unity Assets, packages and
  shipping build list match the start-of-turn checkpoint `567bbd5`; the existing
  ProjectSettings change retains its M0 hash and is excluded from commits.
  No code tests were warranted for this documentation and concept-image pass.
- Stopped after image inspection for the user's review. No M2 or implementation
  of the new board has begun.
