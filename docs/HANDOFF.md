# Quackies handoff

## Current task: duck migration M1 art redirection

The playable baseline below has been merged into main as `74e40cf`. New work is
on `codex/duck-game-milestone-0`. Read [the migration plan](duck-migration/PLAN.md),
[status](duck-migration/STATUS.md) and [baseline](duck-migration/BASELINE.md) for
the current scope and evidence. The user accepted M0 and authorized M1, then
rejected the V1 visual style on 11 September 2026. V1 remains historical
evidence: compile, native 1133 × 744 layout, repeat construction and
Explore/Reset checks passed, but the style is superseded. The earlier bounded
art task is complete: three new concept sheets were generated, inspected and
saved under
[duck-migration/concepts/2026-09-11](duck-migration/concepts/2026-09-11/README.md):
four distinct player duck tiles, a seed encounter tile study, and an illustrated
three-biome board. The user then approved the ducks/biome style and requested
a V3 clarity pass: one strongly coloured seed shape, explicit tile wells, exact
current reward values and eight assigned rests. V3 was completed for
review: [images and review notes](duck-migration/concepts/2026-09-11-v3/README.md).
Precise static typesetting over generated scenery is explicitly approved; the
first fully generated label attempt was inaccurate. Final board, renderer and
QA are published in `47434dc`. All 53 numbered pads and reward pairs and eight
rest markers passed static checks and the final image was visually inspected.
The user accepted the tokens and requested a V4 board-only refinement: incomplete
starting nest, no visible position/start labels, icon rewards with zero Twigs
omitted, corrected geometry at 11/44/53 and an obvious shelter beside every rest.
V4 is finished and inspected: [board and review notes](duck-migration/concepts/2026-09-11-v4/README.md).
The final renderer/image/QA checkpoint is `a66c8d3`. All 53 reward rows are
preserved, five zero-Twig rows show only coins, and eight distinct shelters
connect to their green pads with duck-footprint trails. Stopped for the user's
review. The user then preferred the untouched upper-right-oasis background
(exec-56f99a5a) and the earlier bubbly spaces/full illustrated legend
(exec-88b1f5df), finding the typeset V4 treatment less attractive. V5 is finished
with painted component sprites plus exact text: [finished board and notes](duck-migration/concepts/2026-09-11-v5/README.md).
The image/renderer/QA checkpoint is `db76c1c`. All 53 reward groups fit their
painted capsules using measured Marker Felt glyph bounds; lead checked the full
board and label close-ups. The user likes the V5 style but rejected its painted
spaces’ alignment with the paths. A V6 base-board image-only revision is ready:
it connects the upper middle-meadow shelter to the route on the left and uses
softer pond grass/riverside-verge and meadow moss/clover routes while retaining
desert sand, preserve the far-upper-right oasis, nest, eight shelters and
bridges, and leave room for future token spaces. No tiles, tokens, numbers,
reward text or legend belong in this base image. Lead visually inspected the
generated PNG and it is ready for user review. It has no overlay, so 53-space
fit and exact indexed alignment remain unresolved. Do not import into Unity,
change source/settings, implement biome
rewards, or start M2. Unity positioning was discussed only, not authorized.
Preserve the original playable game and pre-existing ProjectSettings preload
removal.

## Completed playable baseline

Updated: 10 September 2026, after the full Unity match and iOS export.
The agreed initial playable milestone is complete.
Read IMPLEMENTATION_GOAL.md for scope, RULES_REFERENCE.md for primary rules, and
PROGRESS.md for validation. Test tubes and separate AI-history UI are deferred.

## Run and workflow

Branch codex/initial-playable-scene; GitHub geoking/quackies. Open
unity/Quackies.Unity in Unity6000.6.0f1. From Edit mode choose
**Quackies → Build and Play Initial Scene**. Run Core checks with
`dotnet test Quackies.sln`; CLI supports `--starting-rubies 0`.
Root owns Git: small Core source, separate tests, separate compiling Unity
checkpoints; push each, preserve unrelated files, no merge/force push.

## Current published state

- All 24 Set 1 fortunes are implemented and enabled by default (81e280d).
  Explicit empty decks remain available for isolated fixtures. Sessions reveal
  nine distinct cards. CLI seed support is 3e0f638; default-deck tests 0bcb742.
- Final-round scoring 14cecc3 / tests fe02c97 automatically resolves the best
  single exploded reward; safe pots receive printed VP plus floor(coins / 5).
- Well Stirred includes blue-selected whites. Strong Ingredient suppresses its
  own immediate/deferred action (dd0c274 / 67ae4ba). Second Chance protects its
  opening five placements and offers one restart (5a89d48 / 5efc452).
- Blue full-pot boundary 45991dd / a210bca prevents extra clamped placements.
- Full Core suite: 129 tests pass; build has zero warnings/errors. Normal AI:
  192 single-card complete matches and 32 mixed-deck matches. Published CLI smoke
  with seed 0 reveals Rat Infestation, offers legal actions and exits cleanly.
- Unity contains the new full-deck Core DLL. Recompile and scene rebuild pass.
  Settings, dice, scoreboard, CPU pot, primary controls, fixed bag reference and
  readable fortune caption are published. Header phase wrapping fixed in 61c9f33.
- The saved-scene and continuation-fix validation is published in b2861ad (with
  d40d4e4 as its source scene checkpoint). The validation report at
  `tools/validation/evidence/2026-09-10/full-match-report.txt` records a full nine-round
  pointer-callback-driven match: Human 43 VP, AI 42 VP, nine distinct fortunes,
  settings restart, dice, references, readable choices and scoreboard/opponent-pot
  navigation all passed. Core last confirmed 129 tests passing on 10 September;
  the 48-action continuation fix is Unity validation evidence, separate from the
  Core test count. Final-round scoring is automatic; all 24 fortunes remain
  enabled by default.
- Canonical evidence is tracked under `tools/validation/evidence/2026-09-10/`.
  It includes the full report, native/resized screenshots and extracted iOS build
  summary. Runtime console evidence is 0 new errors since cursor 4. The completed
  Unity recompile status also reports no compilation errors.
- Export serialization is published in a48e0c8. iOS export succeeded to
  `Builds/iOS` for the initial scene with 0 errors and 5 Unity warnings. No
  signed build, installation or physical-device test is claimed.

## Completion and future work

The final native GameView target, Canvas pixel rect and fitted tabletop were
verified at 1133×744, the iPad mini aspect. Draw, Stop and Flask corners are all
inside the viewport. See `tools/validation/evidence/2026-09-10/viewport-check.json`
and `initial-scene-native.png`. Screen.width/height in the connector's Editor GUI
context described a different surface; the capture target resolves that mismatch.

The full acceptance review is recorded in the evidence README. Test-tube rules,
the separate AI-history pane and physical-device validation remain future work.
No Xcode signing, installation, main merge or public release was performed.

The rules sweep is recorded in RULES_AUDIT.md, including corrected boundaries and
explicit Second Chance timing interpretations. Core must remain independent of
Unity. Pre-existing prototype compatibility types are retained; current gameplay
uses MatchSession exclusively.

## Ownership and recovery

Root owns Git and coordinates the Unity validation work. Check live agent and
Git status before resuming; preserve unrelated changes and do not merge main.
Test-tube rules and the separate AI-history pane remain deferred to a future goal.

Unity MCP is currently unavailable. Use the CLI connector and inspect live agent
state before resuming. The reusable validation harness is tracked at
`tools/validation/QuackiesUnityFullMatchProbe.cs`; local run output is under
`Temp/QuackiesValidation/`, with canonical evidence under
`tools/validation/evidence/2026-09-10/`. Do not describe the recorded GameView
captures as physical-device evidence.

Shell push authentication fails; GitHub connector publishing works. The lead
publishes committed checkpoints without force-pushing and verifies that local
and remote trees match. Temporary publishing helpers are session conveniences;
do not rely on ignored Temp files surviving a reboot or cleanup.
