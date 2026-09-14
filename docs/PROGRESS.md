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

### 17. M1 V3 — exact board labels and eight rests (11 September 2026)

- The user approved the duck tiles and three-biome art style, requested one
  strongly coloured seed shape and explicit placement/reward labels, and chose
  eight resting spots. Data and revised brief are published in `63b4ceb`.
- Generated one orange rounded triangular seed tile and clean illustrated
  board scenery; native outputs and inspection records are in `8990e3e`.
  The fully generated labelled-board attempt had numbering/reward errors and
  was excluded. The user explicitly approved precise static typesetting.
- Published the final board, renderer, layout and QA in `47434dc`. The image
  contains start 0, spaces 1–53, exact existing Pond penny/Twig rewards, and
  visible rests at 5, 13, 20, 28, 34, 40, 46 and 52. Space 53 remains the final
  scoring space; the current game scores the next empty space.
- Source data was independently compared with both Core reward arrays. Checks
  passed for 53 unique numbered spaces, 53 exact reward pairs, eight selected
  original ruby indices, bounds and non-overlap. The final 3072 × 2048 image was
  visually inspected for reward-strip fit and visible rest medallions.
- The published review image uses quality-98 JPEG with 4:4:4 chroma sampling;
  the lossless PNG exceeded the publisher's transport capacity. The renderer
  can reproduce either format. Prompts, hashes, data and reproduction notes are
  in [the V3 review folder](duck-migration/concepts/2026-09-11-v3/README.md).
- No Unity imports, Editor calls or gameplay changes occurred. Core still has
  15 ruby flags; the eight rests are an image-only design pending review.
  The proposed biome reward curve remains deferred. Source, tests, Unity
  assets/packages and build scene list match checkpoint `4866fb2`; the
  pre-existing ProjectSettings change retains its M0 hash and is excluded.
- Stopped for the user's reaction. Static-image checks are not a new Unity,
  iOS export or device test; no M2 work has begun.

### 18. M1 V4 — nest, icon rewards and shelter trails (11 September 2026)

- The user accepted the tokens and requested another board-only review.
  Published the revised brief and unchanged data in `d5aebab`: incomplete
  starting nest, no index/start labels, coin/twig reward icons, zero-Twig
  omission, consistent geometry at 11/44/53 and a visible shelter at every rest.
- Three built-in image edits produced the selected nest/shelter background.
  An alignment attempt displaced a meadow shelter; a focused edit restored
  the distinct fern and stone shelters before selection. Native artwork,
  exact prompts and provenance are published in `d4523f5`.
- Published the final static renderer, layout, JPEG and QA in `a66c8d3`.
  All 53 internal route anchors and existing reward pairs are retained; five
  zero-Twig rows show a centered coin pair. Eight green rest pads have feather
  markers and duck-footprint entry trails to recognizable shelters. Bridge
  arrows clarify direction after removing visible indices.
- Static checks passed for 53 reward rows, five zero-Twig omissions, eight
  rests/entry trails, uniform 11/44/53 geometry, bounds and non-overlap. Lead
  visually inspected the final 3072 × 2048 JPEG for the nest, icon rows, shelter
  association, restrained footprints and unobstructed placement wells.
- Reward data is identical to the verified V3 audit. Source, tests, Unity
  assets/packages and shipping scene list match starting checkpoint `49ab002`.
  The pre-existing ProjectSettings preload removal retains its M0 hash and was
  excluded from every checkpoint. Approved token images remain unchanged.
- [V4 review notes](duck-migration/concepts/2026-09-11-v4/README.md) record the
  method, prompts, files and validation. No Unity calls/imports, game-rule
  changes, iOS export or device test occurred. Stopped for user review; M2 and
  gameplay experiments have not begun.

### 19. M1 V5 — painted spaces with precise typography (11 September 2026)

- The user preferred the first nest/shelter background (exec-56f99a5a), with
  the oasis at the far upper right, and the bubbly stone spaces and full legend
  in the earlier generated labelled-board attempt (exec-88b1f5df). V4's flat
  repeated well treatment was superseded. Saved both references and the revised
  brief in `f05432f`; the earlier image remains a style reference, not rule data.
- Kept the selected background pixels unchanged. Generated a nine-component
  painted kit: three ordinary stone variants, three leafy white-feather rest
  variants and three illustrated resource/legend icons. The native output and
  a background-extraction retry were opaque; deterministic matte extraction
  prepared the components for the agreed static compositing workflow.
- Published native and RGBA component atlases, extraction script and inspection
  evidence in `cd32631`. All RGB channels remain unchanged; opaque stone and
  feather samples and nine complete sprite bounds were verified. A solid-dark
  preview confirmed clean silhouettes and complete foliage.
- Published the final bitmap composition, layout, atlas metadata, JPEG and QA
  in `db76c1c`. Painted wells replace the flat SVG shapes; the larger footer
  restores matching icons, separators, playful Marker Felt lettering and a
  dashed scoring arrow. Internal indices, start labels and SCORE badges remain
  absent. All 53 existing reward rows and eight rest positions are retained.
- Measured actual numeral glyph bounds and bearings for centered reward groups,
  with a 6px horizontal / 3px vertical inset inside their painted capsules.
  All 53 measured groups pass containment checks; zero-Twig rows 1–5 show only
  centered coins. Source row comparison, atlas alpha/crops, pad bounds and
  non-overlap checks pass. Lead visually inspected the final full board and
  close-ups of the longest reward rows and coin-only start rows.
- [V5 review notes](duck-migration/concepts/2026-09-11-v5/README.md) preserve
  prompts, method and hashes. Source, tests, Unity assets/packages and shipping
  scene list match starting checkpoint `fcbedf6`; the pre-existing settings
  hash is preserved and excluded from commits. Approved token images are
  unchanged. No Unity calls/imports, new game rules or device checks occurred.
- Stopped for review of the finished labelled image. No M2 or implementation
  work has begun, and user style acceptance is pending.

### 20. M1 V6 — base-board route revision (11 September 2026)

- The user likes V5’s painted style but rejected the alignment of painted spaces
  with the illustrated paths. The current bounded pass edits only the base board:
  connect the upper shelter in the middle meadow island to the route on the left,
  use softer pond grass/riverside-verge and meadow moss/clover routes while
  retaining desert sand, and leave room for future token spaces.
- Preserve the selected far-upper-right oasis, nest, eight shelters in the 2/3/3
  biome distribution and both bridges. The base image has no tiles, tokens,
  numbers, reward text or legend. V5 alignment QA remains historical and is not
  carried forward as proof of this revision.
- Root saved the generated image and inspection notes under
  `duck-migration/concepts/2026-09-11-v6/`; lead visually inspected it and
  marked it ready for user review. V6 has no overlay, so 53-space fit, reward
  placement and exact indexed alignment remain unresolved. No Unity positioning
  or import was authorized, and no Core, source/settings, gameplay, M2 or rules
  changes occurred. The full existing rules baseline and review gate remain in
  force.

### 21. M1 V7 — biome distinction revision (11 September 2026)

- V6 is retained as historical evidence at checkpoint `94fd3bb`. The user wants
  the middle grasslands distinguished from wetlands: cooler teal/sage damp banks
  and reeds versus warm sunny open grasslands with broad short-grass route,
  taller soft grasses, flowers and leafy cover around the three shelters.
- V7 preserves the desert composition, far-upper-right oasis, nest,
  eight shelters in the 2/3/3 distribution, bridges, left access to the upper
  meadow shelter and broad future tile corridors. It contains no tiles, text or
  legend. The generated image is under
  `duck-migration/concepts/2026-09-11-v7/`; lead visually inspected it and
  marked it ready for user review. V7 has no overlay, so 53-space fit, reward
  placement and exact indexed alignment remain unresolved.
- No overlay, Unity positioning/import, Core, source/settings, gameplay, M2 or
  rules changes are authorized. The full rules baseline and review gate remain.

### 22. M1 V8 — shortcut-blocking revision (11 September 2026)

- V7 is retained as historical evidence at checkpoint `9ee73e0`. The user wants
  the middle island’s apparent shortcuts blocked with dense tall meadow grasses,
  small leafy shrubs and trees, while keeping the main U route and all shelter
  entry pockets open. A reed/willow thicket should also block the direct nest to
  first-bridge shortcut while preserving the downward route and returning lane.
- Preserve the cool wetland, warm grassland and desert distinctions, eight
  shelters in the 2/3/3 distribution, nest, bridges, oasis, left access to the
  upper meadow shelter, broad corridors, blank footer and no tiles/text/legend.
  V8 is saved under `duck-migration/concepts/2026-09-11-v8/`; lead visually
  inspected the corrected image, including reopened shelter access pockets, and
  marked it ready for user review. No exact 53-space fit or Unity verification is
  claimed.

### 23. M1 V9 — bridge/style review candidate (11 September 2026)

- V8 is retained as historical evidence at checkpoint `93f7a47`. The user wants
  bridges integrated into the scene and a clean high-resolution sweep to reduce
  iterative grain. V9 preserves the wetland/grassland/desert distinctions,
  shortcut barriers, LEFT/RIGHT/DOWN meadow entries, eight shelters, nest,
  oasis, broad routes and blank footer.
- Bridge treatment is specified as grounded earth/stone abutments, open walk-on
  approaches and contact shadows, with wet moss on the wetland bridge and drier
  stone/wood on the desert bridge. V9 is under
  `duck-migration/concepts/2026-09-11-v9/`; selected native candidate measures
  1536 × 1024 and was visually inspected as a reviewable bridge/style checkpoint.
  The requested 3072 × 2048 master remains unresolved after two built-in
  attempts. No resampling was applied; a CLI/API fallback requires explicit user
  authorization and a local `OPENAI_API_KEY`, which is not configured. Do not
  claim 53-space fit or Unity/runtime evidence.

### 24. M1 V10 — route-width and wasteland bridge review candidate (12 September 2026)

- V9 is retained as historical evidence at checkpoint `72ffdf3`. The user wants
  broadly equal usable widths across all three islands. The original target was
  about 120px; the later generation target was 140–150px. General corridor
  compaction and one lower-grotto compaction were applied. Bounded manual samples
  measured meadow 110–140px, wetland 100–145px, desert-left 120–160px,
  below-oasis 130–170px and lower-grotto sides 110–130px/right and 115–135px/left,
  with about ±10px uncertainty. These samples do not certify all 53 positions.
- The wasteland crossing now spans a deeper rocky gap with a grounded
  timber/rope bridge, open deck ends and sweeping approaches from the meadow's
  upward lane to the desert's downward lane. Preserve the LEFT/RIGHT/DOWN meadow
  entries, nest, oasis, shelters, broad routes and no overlay/text/legend.
- V10 is under `duck-migration/concepts/2026-09-12-v10/` and is ready for image
  review. The dramatic bridge approaches passed visual inspection. The 3072 ×
  2048 master remains unresolved; do not claim exact 53-space fit or
  Unity/runtime evidence.

### 25. M1 art selection and consolidated plan (12 September 2026)

- The user approved **exec-9d44cb08-9cb9-4367-8a00-4a5f8c60b78b**, the first V10
  candidate. Saved its exact native PNG and provenance in
  [the approved-board folder](duck-migration/concepts/2026-09-12-approved/README.md).
  This explicit choice supersedes the later V10 candidate as the art reference;
  the later image's passing sampled audit does not transfer to the selected one.
- Consolidated [PLAN.md](duck-migration/PLAN.md), removing repeated iteration
  notes and gates. Retained approved duck/seed references, defined the layered
  board and precise overlays, and documented a reusable physical-token philosophy:
  one silhouette per category, strong colour plus recognizable imagery, common
  fit, uncluttered faces and separate strength/state overlays.
- Renamed bonus-die presentation to **Most Rested Duck reward**, a small bonus
  using current eligibility, tied leaders and outcomes. Fortune presentation
  becomes **World Events**, one situation shared by all ducks at each Day's
  start, retaining existing reveal-time and later choices/effects.
- Aligned STATUS, ASSET_BRIEF and the current HANDOFF with this decision. Marked
  V1 provenance and later V10 art/audit explicitly historical. Recorded exact
  53-space fit, selected-image clearance, production resolution and the
  eight-shelter/15-reward representation as remaining integration work.
- Validation: checked reward/event wording against Core and received a focused
  independent documentation review; checked changed-document links, obsolete
  terminology, selected-image SHA-256 and whitespace. The native 1536 × 1024
  image is byte-identical to the user-selected source. Core, tests and Unity
  files were not changed; the pre-existing ProjectSettings hash is preserved
  and excluded from the checkpoint. No runtime tests, image generation,
  resizing, Unity calls or imports were performed for this documentation change.
- Stop here at the user's request. The next milestone and Unity construction
  await the user's command; regular checkpoint publication remains authorized.

### 26. Day/dream rules discussion and presentation concepts (13 September 2026)

- Recorded the user's proposed 50-space track, resting on the final encounter,
  Sleep score, dream shopping, stronger next-Day advantages and one-to-one
  Feather upgrades in a separate discussion study. The approved baseline rules
  were not silently replaced; current plan/status/handoff link to the study.
- Reviewed the rule implications against Core with a focused Sol/high worker:
  reward-table/index changes, movement versus Exhaustion, repeated-winner
  feedback, Feather purchasing power, event timing and final-night closure.
  Proposed comparing earned Sleep for Most Rested, keeping spend remaining
  separate and testing bounded temporary advantages. No balance simulation or
  gameplay implementation occurred.
- Generated and visually inspected two presentation mockups: an integrated
  dream tray over the moonlit board, and a separate nest/dream player mat.
  Both retain the cardboard duck/seed style and show a persistent exact Twig
  score plus decorative nest growth. The recommended flow uses the integrated
  tray with an expandable nest view.
- Saved both byte-identical native 1536 × 1024 images, complete prompts,
  provenance/limitations and discussion notes under
  `duck-migration/concepts/2026-09-13-dream-study/`. Example values/prices are
  illustrative; the images do not establish 50-space placement or a legal shop.
- Validation: documentation links, native image hashes, original approved board
  hash and pre-existing ProjectSettings hash checked. No Unity calls/imports,
  code changes, runtime tests or approved asset replacements occurred.
- Stopped for discussion and visual feedback; implementation remains paused.

### 27. Accepted Day/Dream plan and encounter family (13 September 2026)

- Replaced the old presentation-only roadmap with the accepted duck-game target:
  50 playable spaces plus nest, final-occupied rest, Sleep earned/remaining,
  full-screen Dream Concept B, shared 1/2/3 nest purchase tiers, automatic
  single-purpose Feather trails and bounded Dawn Delivery catch-up. Most Rested
  compares safe ducks' earned Sleep and initially grants a temporary start step.
  Thresholds/tier timings are initial settings to test, not balance evidence.
- Updated PLAN, STATUS, ASSET_BRIEF and current HANDOFF. Aligned AGENTS and the
  architecture/baseline headers so future work does not restore superseded
  mechanics. Preserve the original game as a tested reference; the new profile
  requires an explicit 50-row table and complete encounter/event adaptation.
- A focused Sol/high source audit recommends evolving Core/MatchSession and the
  CLI rather than restarting. Saved ENGINE_EVOLUTION.md with reusable boundaries,
  concentrated refactors, incompatible event examples and planned verification.
  The short next-build sequence is rules sheet, Core/CLI, Unity layout, complete
  playable integration, then balance and finishing. No implementation began.
- Generated two token sheets: Obstacles, Tailwind, Signpost, Refreshing splash,
  Nesting reeds, Companion duck and Wildflowers, alongside a Seed style reference.
  Root visually inspected all eight illustrated chips, labels and representative
  strength badges. Each category has a distinct colour/silhouette; the Companion
  duck remains inside a round encounter disc. The original approved Seed and
  player tiles remain unchanged; new concepts await visual feedback.
- Saved native 1536 × 1024 sheets, full built-in image_gen prompts, provenance,
  hashes and inspection notes under
  `duck-migration/concepts/2026-09-13-token-family/`. These are opaque concept
  sheets, not production sprites or a verified iPad footprint.
- Validation: focused independent plan review, 57 local documentation links,
  native-image provenance, selected-board hash, preserved baseline HANDOFF tail,
  pre-existing ProjectSettings hash and whitespace checks. No runtime tests,
  Unity calls/imports, Core/CLI changes or approved artwork replacements occurred.
- Stopped after planning and token art. The next bounded milestone is the complete
  rules sheet; implementation and Unity remain subject to the next user command.

### 28. Ten-Day direction, fresh powers and obstacle study (13 September 2026)

- The user approved the token-family style, chose ten Days as standard and
  requested original encounter rules independent of the Quacks structure.
  Updated the current plan, status, art brief, engine notes, working agreement
  and handoff. Shorter-match settings remain later work; the initial third nest
  tier now extends through Day 10, with final-Night behavior still unresolved.
  The old game remains a tested reference, not a required 24-event conversion.
- Saved [the encounter/obstacle study](duck-migration/concepts/2026-09-13-obstacle-study/README.md):
  seven helpful powers, four regular nuisance variants and an optional Grumpy
  Goose. Eight whites plus five colours, five safe Exhaustion and the sixth
  worn out are working proposals. Default movement one is unprinted; Tailwind
  has explicit total movement 2/4/6. Prices, full rewards, worn-out payout and
  exact powers remain open for review; none were implemented or called balanced.
- A focused Sol/high worker enumerated every white/colour ordering for candidate
  bags and checked exact hypergeometric formulas. In 8+5, stopping at white five
  averages 7.78 placements and 2.78 coloured placements. Continuing through draw
  eight has a 24.94% chance of reaching white six. This excludes all powers,
  nuisance penalties, purchases, stopping strategy, weather and permanent starts;
  it is a bag-pressure baseline, not a measured player loss rate or full-game test.
  Saved the reproducible dependency-free script and exact JSON report.
- Independent candidate-rule review identified duplicate Log/Pebble behavior
  and shield/capped-nuisance timing. Pebbles now reduces Sleep by one only if
  its own chip is the safe resting place. Clarified shield consumption, pending
  nuisance priority, daily caps and cancellation versus resolved placement.
  Feather effect remains fixed at one permanent start step per Feather.
- Generated three native 1536 × 1024 sheets with built-in image_gen: ordinary
  encounters without generic badges; Companion/Wildflowers and Tailwinds with
  readable →2/→4/→6; five white obstacle illustrations with no printed ‘1’.
  Root visually inspected the outputs and saved byte-identical images, prompts,
  source paths, hashes and limitations. These are opaque concept sheets, not
  isolated sprites or game-size readability evidence.
- Validation: exact bag checks passed, 58 local documentation links checked,
  JSON/provenance/native-image hashes verified, historical HANDOFF baseline
  preserved, selected-board and pre-existing ProjectSettings hashes unchanged,
  and whitespace checks passed. No Core/CLI implementation, Unity calls/imports,
  runtime tests or approved asset replacements occurred.
- Stopped for the user's reaction to the rules and art. The full rules-sheet
  milestone remains incomplete; no automatic implementation or Unity work follows.

### 29. Ability quantities, conditional movement and Day 5 Goose (13 September 2026)

- The user likes the helpful colour ideas and requests Reeds x1/x2/x3 awarding
  one/two/three Twigs while each chip still has normal movement one. Saved
  [ENCOUNTER_RULES.md](duck-migration/ENCOUNTER_RULES.md) as the current discussion:
  category quantities are distinct from movement instructions and never imply
  extra placements or triggers. Other denominations and shared future rule cards
  are proposals; initial play still uses one fixed ruleset.
- Proposed conditional precision alongside Tailwind's reliable distance: Seeds
  can eat one kernel for an extra step after an Obstacle, forgoing that kernel's
  Sleep; a Companion drawn while a shield is already held moves an extra step
  instead of stacking protection. Neither proposal is implemented or priced.
- Replaced arbitrary ordinary-white payout deductions with conditional nuisances.
  Mud blocks the next colour's bonus movement while preserving intrinsic movement
  and other effects. Log halves movement; Pebbles/Brambles deduct Sleep/a newly
  earned Twig only on their own safe resting chip. Clarified nonstacking slow
  effects, one placement per chip, and shield use on actual rest deductions.
- Recorded the user's Day 5 Goose idea and a candidate lifecycle: add one per
  bag once, retain it for later Days, and lower that Day's safe maximum five to
  four only when resolved. It remains a white Obstacle; Companion cannot block
  its special Exhaustion rule. Splash checks the resulting maximum and cancels
  a fatal reveal completely before automatic safe settlement. These precise
  lifecycle/counter interpretations remain for review, not finalized balance.
- Independent Sol/high review checked candidate quantity semantics and timing.
  A separate Sol/high exact enumeration compared eight continued draws with no
  powers or purchased colours: prior 8+5 gives 24.94% worn-out, adding Goose gives
  57.58%, replacing a white gives 52.14%. The document records exact fractions
  and enumeration conditions. These are controlled risk comparisons, not actual
  Day 5 player loss rates; a replacement is a fallback if addition proves too harsh.
- Updated PLAN, STATUS, ASSET_BRIEF, ENGINE_EVOLUTION and current HANDOFF; marked
  the previous art study's powers superseded while preserving its images and
  opening-bag evidence. Checked current document links, whitespace, historical
  baseline handoff preservation and the unchanged pre-existing ProjectSettings
  hash. No art generation/editing, Core/CLI changes, Unity calls/imports or runtime
  tests occurred. Stopped for discussion; implementation remains paused.

### 30. Plain Seeds and a Companion flock proposal (13 September 2026)

- The user rejected the Seed food/movement choice and requests a cheap plain
  non-Exhaustion chip. Updated the current plan and encounter discussion to
  remove Seed Sleep, kernel denominations and ability text; ordinary movement
  remains one. Early board rewards and Dream prices must support buying without
  the former Seed Sleep subsidy.
- Interpreted the user's successive Companion values 2/3/4 as movement of the
  first/second/third-and-later placed that Day. The candidate resets counts each
  dawn, counts physical Companions rather than all chips or owned inventory,
  and replaces the former nuisance shield. Mud removes the flock movement bonus
  while retaining the placed count; Log halves the proposed movement normally.
- Proposed one Night award for the largest safe flock after everyone finishes:
  tied positive leaders earn +1 Sleep with one Companion or +2 total with two
  or more. Worn-out ducks are excluded. The award enters earned/available Sleep
  before the result freezes and Most Rested is compared. These caps, tie rules,
  interpretation and eligibility are recommendations for review, not accepted
  or tested balance. A neighbour-only multiplayer card remains a later option.
- A focused Sol/high review checked the cumulative 2/5/9/13 Companion movement,
  dominance risk relative to Tailwind, and the loop through extra buying power
  and Most Rested. Final read-only review found no retained live Seed ability or
  Companion shield rule. Clarified that owning additional copies changes draw
  likelihood; resolving a chip does not improve future odds by itself.
- Added a proposed Signpost move-two/peek-one as a simple utility mover, with a
  targeted Wildflower shelter step kept as a later alternative. Neither is an
  implemented rule. Updated PLAN, STATUS, ASSET_BRIEF and current HANDOFF while
  retaining historical studies and baseline evidence.
- Validation: checked changed-document links, whitespace, unchanged historical
  handoff tail and preserved pre-existing ProjectSettings hash. No Core/CLI,
  Unity, image or runtime-test changes occurred. Stopped for discussion; the
  complete rules sheet and balance work remain outstanding.

### 31. Agreed rule overview and complete encounter token set (13 September 2026)

- The user accepted the current rules direction and requested a simple complete
  recap plus designs for every agreed encounter. A focused Sol/high worker
  drafted [RULES_AT_A_GLANCE.md](duck-migration/RULES_AT_A_GLANCE.md); root reviewed
  it against the conversation and current plan. It covers ten Days, the 50-space
  route, daily draw/rest loop, all seven helpful families, five whites, the
  13-chip starter, Goose, Night comparisons, nest purchases and Feather/Dawn rules.
- Kept unknown values visibly separate: exact 50-row rewards, prices and stock
  policies, full World Events, worn-out payouts, route-end/no-draw/rewind behavior,
  saturated trails, final-Night handling and final ties. Corrected the recap's
  Splash check to use the resulting maximum including Goose, and distinguished
  no nightly Feather redemption cap from the unresolved physical route endpoint.
- Updated PLAN, STATUS, ENCOUNTER_RULES, ENGINE_EVOLUTION, ASSET_BRIEF and current
  HANDOFF to record current powers as accepted first-test rules. Wildflower
  movement, additional Signpost/Flower quantities and alternate rule cards remain
  future options. Acceptance is not full-game balance evidence or completion of
  the remaining rules-sheet milestone. No implementation or Unity work began.
- Generated four matching native 1536 × 1024 review sheets with built-in
  image_gen, covering all 16 encounter designs/variants: five everyday chips,
  Tailwind →2/→4/→6, Reeds bundle quantities ×1/×2/×3, and five white Obstacles.
  Signpost now shows →2; Companion remains unnumbered with no shield; Goose
  says “Joins on Day 5” and has no obsolete always-five-safe footer.
- Root visually inspected every token, label and numeric mark. Saved the
  byte-identical RGB images, complete prompts, source paths, hashes and review
  findings under [the current token set](duck-migration/concepts/2026-09-13-agreed-token-set/README.md).
  These are opaque concept sheets, not isolated sprites or an iPad-size fit test.
  Approved player ducks and board source remain unchanged.
- Validation: 65 local documentation links, four image/source hashes, dimensions
  and RGB metadata, JSON, historical baseline handoff preservation, selected-board
  and pre-existing ProjectSettings hashes, and whitespace checks passed. No
  Core/CLI changes, Unity calls/imports or runtime tests were performed.
- Stopped after the requested rule recap and visual set; await the user's next
  command before proceeding with another milestone.

### 32. V1 board, economy, events and Most Rested proposal (14 September 2026)

- Incorporated the user's new direction: Mud loses one active Companion without
  deleting owned chips or changing prior movement; Splash protects only the
  immediately next chip's nuisance, retaining movement/Exhaustion; Dawn Delivery
  is uncapped ceil(Twig deficit/4); worn-out ducks retain Twigs and half Sleep
  rounded down; safe Day 10 havens add another +2 Sleep; Night 10 converts retained
  Sleep at 4:1 to Dream Twigs and gives safe Most Rested winners +1 Dream Twig.
- Authored all 50 candidate board rows in CSV/JSON and a readable table, with
  eight havens at 7/13/21/27/32/38/44/50, 2/3/3 across biomes. Ordinary wasteland
  has lower Sleep than late meadow; its havens pay more. Non-endpoint havens
  match both neighbours' Twigs. Endpoint 21 Sleep/9 Twigs/2 Feathers is one above
  the previous printed Sleep/Twig maxima. All 11 offers have proposed prices.
- A focused Sol/high worker proposed ten shared World Events with per-duck
  triggers and exact timing. Root replaced the fewest-distance reward with
  Friendly Guide's first-Obstacle nuisance protection, verified Splash overlap,
  and retained a once-shuffled, no-replacement deck using all ten cards.
- Another bounded Sol/high audit used exact arithmetic for opening travel,
  printed reward affordability, single-purchase movement, one-Companion Mud
  attrition, controlled Goose/Splash pressure and repeated uncapped Dawn gifts.
  Saved the reproducible script and JSON with model omissions explicit. This is
  not a full-match simulation or evidence of balanced AI/human play.
- Kept interpretations visible: reduced active flock governs later movement and
  Night comparison; final Pebbles/Brambles penalties also apply when worn out;
  Reeds Twigs survive wear-out while haven Feathers, Flowers, flock and other
  safe-only bonuses do not. Proposed unlimited initial shop stock, one-per-family,
  Sleep expiry, no final shopping and housekeeping rules remain review items.
  The finite route versus uncapped permanent Feathers remains unresolved. No
  cap, discard, bank, conversion or automatic implementation workaround was added.
- Generated and visually inspected one matching lavender zzz cloud marker for
  the temporary Most Rested step beyond the updated Feather trail. Saved the
  native byte-identical 1536 × 1024 opaque PNG, prompt, hash and inspection.
  Existing board/player/encounter images were preserved; the latest 16 encounter
  designs are now recorded as user-approved. No Unity import occurred.
- Consolidated PLAN around canonical rule/data references, updated the recap,
  detailed timing, architecture recommendation, art brief, STATUS and current
  HANDOFF. Preserved the historical baseline handoff tail unchanged.
- Validation: all 50 CSV/JSON rows agree, haven counts/adjacent Twig plateaus and
  endpoint maxima pass, all 11 offers and ten unique cards are present, local
  documentation links and JSON parse, artwork/source and pre-existing Settings
  hashes are preserved, and whitespace checks pass. See v1/validation.json.
  No Core/CLI changes, Unity calls or runtime tests were performed.
- Stop at this reviewable planning/art checkpoint. M2 is not complete until the
  remaining boundaries/policies are reviewed; the next implementation milestone
  requires the user's command. Regular checkpoint commit/push remains authorized.

### 33. Approved economy, capped Dawn and shared World Event proposals (14 September 2026)

- The user approved the 50 board rewards and shop prices. Recorded them as
  accepted starting values without changing a row or offer; full-game balance
  remains unverified. Event effects and other marked policies remain proposals.
- Replaced player-facing “families” with **token types** and a concrete example:
  Seed + Tailwind + Signpost is three types; three Tailwind strengths count as
  one. Retained stable historical art paths and internal data field names.
- Adopted the user's Dawn source cap: 0 deficit gives none, 1–4 gives 1 Feather,
  5–8 gives 2, and 9+ gives 3. Each awarded Feather still advances one permanent
  step, and the score comparison repeats before each dawn's deliveries.
- A bounded Luna/medium worker updated only the audit script/results, preserving
  unrelated bag calculations. Exact cap-boundary assertions passed. With zero
  starting Feathers, even nine prior haven rewards at two and nine Dawn gifts
  at three yield at most 45 permanent steps; temporary Most Rested makes the
  latest possible Day 10 start 46. This resolves default-start saturation under
  the current sources/length, not nonzero settings or ordinary draw overshoot.
- A focused Sol/high worker revised the ten-card proposal to four helpful
  effects, three collective goals and three mild setbacks. All Tucked In pays
  +2 Sleep only if every human/AI finishes safely at a haven; Home Before Dark
  needs everyone safe; Shared Supper needs every duck to have placed a Seed,
  including worn-out ducks before Sleep halving. Conditions are checked once
  after all players finish. Equal shared Sleep chiefly helps purchasing and
  final conversion rather than changing the safe Most Rested ranking.
- Still Air halves Tailwind once even alongside Log; Thick Morning Mist removes
  Signpost previews; Restless Night deducts one haven Sleep on a safe finish.
  Weather is not an obstacle nuisance and cannot be blocked by Splash/Guide.
  These are new review proposals, not user-approved or implemented effects.
- Updated PLAN, recap, timing, engine recommendation, board/shop notes, STATUS
  and current HANDOFF. Preserved its historical baseline tail and approved art.
  No Core/CLI or Unity changes/imports/tests were performed.
- Validation: numeric rows/offers unchanged; ten distinct events and explicit
  shared/negative timing; capped Dawn boundaries/default-start bound; unchanged
  non-Dawn audit calculations; CSV/JSON consistency, local links, preserved
  board/Settings hashes and whitespace checks. Updated v1/validation.json.
- Stop for the user's review of the revised cards. Remaining policies/settings
  and event acceptance precede the next implementation milestone.

### 34. Implementation readiness and source-backed migration plan (14 September 2026)

- The user approved the revised World Event mix and asked what remains before
  building, whether Core/API/CLI should be modified or restarted, and what the
  implementation plan needs. Recorded all ten events as accepted first-test
  rules without changing their effects, board rows or shop prices.
- Read-only Sol/high Core and Terra/high Unity audits support keeping the match
  API boundary, state ownership, deterministic injection, inventory, CLI loop
  and presenter/view/editor separation, while substantially replacing old rule
  assumptions and cauldron-specific views. No source files were changed.
- Added [IMPLEMENTATION_PLAN.md](duck-migration/IMPLEMENTATION_PLAN.md) with
  concrete source evidence: colour/value conflation cannot model new token
  quantities; existing preview sampling does not reserve the next draw; round
  nine alone batches hidden decisions; ordinary action IDs are not uniformly
  revision-bound; Normal AI encodes the old game; persistent match save/load
  does not exist. These are scoped implementation requirements, not new bugs
  claimed fixed in the completed reference profile.
- Kept remaining defaults explicit for review: first draw/empty bag, endpoint
  resolution before ending, shared starting setting 0–3, stock/one-per-type and
  Sleep expiry, shared final victory, no separate recovery, payout interpretations,
  and simultaneous Draw/Settle beats. Specify a frozen participant cohort,
  private commitments, deterministic atomic reveal and fresh action issuance.
- Proposed local autosave/Continue for the tablet version. Authoritative save
  data needs bag/deck/RNG continuation, pending previews and beat identity,
  counters, frozen rewards and purchases; MatchView and scene reconstruction
  cannot substitute. Core remains filesystem/Unity independent. This added
  capability and gameplay defaults await review, not implicit implementation.
- Reordered future milestones: close M2 defaults; M3 isolated iPad layout proof;
  M4 Core/CLI; M5 connected Unity; M6 balance/export. The proof uses fixed sample
  data, exactly 50 wells, eight havens, occupied reward visibility and all 11
  Dream offers. It is deliberately not a live match or balance test. Keep
  approved composition and inspect production assets after layout is settled.
- Split Core work into small source/test checkpoints: identity/state; Adventure
  and exact previews; a full Day 1/Night 1/Day 2 slice; ten-Day progression;
  Normal AI and agreed resume. The CLI evolves with each slice. Final review
  corrected the Day-slice dependency so Dawn and zzz activation work before
  claiming a complete first-Day loop.
- Updated PLAN, engine rationale, concise rules, STATUS, current HANDOFF and
  entry-point docs. Removed stale active README/NEXT_MILESTONES descriptions of
  53 spaces/Pennies/rejected V5 alignment. Preserved completed baseline bodies,
  approved art, numerical data, previous audit evidence and unrelated Settings.
- Validation: source-backed read-only findings; changed-document links and
  source anchors; stable baseline handoff/implementation-goal bodies; unchanged
  board rows, prices and event effects; preserved asset/Settings hashes and
  whitespace checks. No new runtime tests, Core/CLI changes, Unity calls,
  scene/import operations or art generation were performed.
- Stopped at the requested readiness checkpoint for user review. M2 remains
  pending the remaining defaults/scope; no implementation milestone began.

### 35. M2 closed; M3 awaiting command (14 September 2026)

- The user explicitly approved the remaining defaults and state saving, closed
  M2, and requested that M3 wait for their command. Recorded local autosave and
  Continue as required v1 scope, still unimplemented; all source/Unity work
  remains paused.
- Applied the two user corrections throughout the current contract: Days 1–9
  resolve Draw/Settle independently with completed actions visible for opponents
  to react to; only Day 10 uses hidden simultaneous commitments and an atomic
  reveal. Normal Days have no cohort wait or forced alternating turn order.
  Private previews remain private and collective Night rewards wait for all
  ducks to finish. Restore ordinary actions versus final-Day pending beats
  according to that distinction.
- Final rank is total Twigs including Dream Twigs, then frozen retained Night 10
  Sleep before conversion, including eligible bonuses and worn-out halving.
  If both values tie, declare a draw with tied winners. No random selection,
  distance tiebreak or extra safe-player eligibility filter is added. Most
  Rested's separate safe-only award remains unchanged.
- Marked starting Feathers 0–3/default 0, first-draw/empty-bag/endpoint behavior,
  unlimited-stock/one-per-type shopping and nightly limits, Sleep expiry,
  no separate recovery, and safe/worn payout timing as approved. Renamed the
  shop data's policy key from proposed_purchase_policy to purchase_policy and
  recorded approval; policy values and all prices remain unchanged.
- A bounded Luna/medium worker updated the recap and encounter timing; root
  reviewed the changes and corrected an unintended random-selection reading
  of “draw”, removed normal-Day cohort language, and made the recap's Day split
  explicit. Updated PLAN, implementation plan, engine guidance, board/shop
  notes, STATUS, current HANDOFF, README and NEXT_MILESTONES consistently.
- Validation: current-document links/source anchors; absence of superseded
  all-Day simultaneous/shared-random-draw wording; final ranking examples;
  unchanged board/offer values, event effects and previous bag audit; identical
  policy values under the approved key; preserved historical baseline handoff,
  approved artwork and unrelated Settings; whitespace checks. M2 closure is
  specification approval, not runtime or balance evidence.
- **M2 complete. M3 has not started.** Next is the isolated iPad layout proof,
  only after the user's command. No Core/CLI edits or tests, Unity calls,
  scene/import operations, art generation, merge or release occurred.

### 36. M3 layout-proof kickoff (14 September 2026)

- The user authorized M3. The bounded target is the isolated iPad layout proof:
  exactly 50 wells, new grass and wasteland composition, a two-Feather wasteland
  example, the final space using the existing oasis with two Feathers and scores
  beneath, Moon/Sleep icon treatment, eight haven links, all 16 encounter
  variants on a common footprint, Dream/Adventure navigation and all 11 shop
  offers. The fixed-data scene has no Core binding.
- Root owns Unity Editor mutations and the later target-size verification. M3 is
  complete only after the live layout is verified, rebuilt twice, and checked for
  new Unity Console errors. This entry records kickoff only; the layout is not
  built and no future validation is claimed.
- Initial audit: branch is clean except the unrelated pre-existing
  `unity/Quackies.Unity/ProjectSettings/ProjectSettings.asset` modification.
  The Unity CLI connector reaches the intended project at Unity 6000.6.0f1 on
  `127.0.0.1:7800`; the Editor was ready, stopped, and returned zero captured
  errors. No imports, Git operations, Core/CLI changes, Unity scene changes or
  runtime tests occurred in this documentation kickoff.

### 37. M3 scene and asset checkpoint — review in progress

- Added the isolated, fixed-data `DuckLayoutProof` scene and reproducible Editor
  builder. The original playable scenes and Core DLL remain unchanged.
- Added generated grass/wasteland wells, Moon/Sleep, single/paired Feather and
  Twig icons. Native alpha was verified for all six selected new assets.
- Approved encounter, duck and zzz image bytes are preserved. Rectangles and
  source-measured sprite outlines isolate their faces in Unity metadata.
- The Editor compilation passes. Two successive builds have the same hierarchy
  digest and unchanged metadata for all 14 textures. The structural audit passes
  50 spaces, eight havens, 11 offers, 16 distinct encounter sprites, text overflow
  and cross-space bounds checks.
- Native 1133 × 744 Adventure capture is available in `duck-migration/m3/`.
  Visual polish, endpoint Feather emphasis and interaction review remain in
  progress. This is a scene checkpoint, not M3 completion or a live duck game.

### 38. M3 layout polish and verification (14 September 2026)

- Refined the normalized board anchors and retained native painted resting
  places at 21, 38 and 50. Feather badges have a readable backing; the final
  oasis has two Feathers and its 21 Sleep / 9 Twigs beneath it.
- Added enlarged encounter/offer inspections with separate Moon, Twig and
  Feather values. All 16 approved token variants fit the common footprint;
  all 11 priced offers fit the full-screen Dream view.
- Final Editor compilation passes with no errors. Two rebuilds produce the
  same hierarchy digest and identical metadata for all 14 textures. The audit
  passes 50 unique spaces, eight havens, crop bindings, reward bounds and text.
- Verified actual Game view target 1133 × 744; native empty, occupied, Dream,
  oasis and offer captures are recorded in [M3 evidence](duck-migration/m3/README.md).
  Pointer checks passed Dream, offer inspection, Close, View adventure, oasis
  inspection and Encounter fit. Final Console query reports zero errors.
- This completes the bounded fixed-data layout proof. No duck gameplay,
  Core/CLI work, production 3072 master, iOS export or device test is claimed.
  M4 waits for the user's command after review. Preserved the approved source
  artwork, original playable scenes/Core DLL and unrelated ProjectSettings.

### 39. M3 reopened for route and haven-art revision (14 September 2026)

- The user rejected the staggered start, uneven spacing and tiles covering
  resting artwork. Reopened M3 for a single centered route with larger spaces,
  seven on-path haven tiles beside their shelters, integrated Feather seals and
  the native oasis endpoint. The previous checks remain historical evidence.
- Adopted high-resolution production authoring, resolution-independent UI and
  iPad mini plus larger-screen review. The built-in image generator returned
  1536 × 1024 despite a 3072 × 2048 request. The user explicitly chose to finish
  layout now and defer the larger board master; no API fallback was used.
- Dream Concept-B styling and playful typography are recorded for the later
  connected presentation milestone. M4 remains unstarted. This checkpoint is
  the revised plan only; new art/route integration and validation are in progress.

### 40. Revised M3 route and haven artwork checkpoint (14 September 2026)

- Replaced the staggered route with 50 centers on a single painted-path guide.
  Kept larger 90 × 66 wells and tightened reward strips to clear bends.
- Added native-alpha integrated one-/two-Feather haven tiles, kept seven havens
  beside shelters, and moved the final oasis seal beside its reward row.
- Unity compilation and the scene layout audit pass; the first revised
  1133 × 744 visual review shows clear reward rows and an uncovered oasis.
- This is a compiling checkpoint. Two-size occupied/pointer verification and
  canonical reward-data synchronization remain in progress; M4 has not begun.
