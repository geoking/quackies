# Quackies handoff

## Current task: fresh encounters and obstacle art ready for review

13 September: the user approved the token-family style, chose ten Days and
requested original encounter powers rather than Quacks mappings. The earlier
accepted direction remains: 50 playable spaces plus
the starting nest; final occupied-space rest and score; frozen earned Sleep
versus remaining nightly allowance; full-screen Dream Concept B; shared nest
levels initially adjusted to Days 1–3, 4–6 and 7–10; persistent Twigs; automatic permanent
Feather advances; Dawn Delivery stork criteria; and Most Rested based on earned
Sleep among eligible non-worn ducks. See [status](duck-migration/STATUS.md) and
the authoritative [plan](duck-migration/PLAN.md) for the bounded specification.

The selected base board remains **exec-9d44cb08-9cb9-4367-8a00-4a5f8c60b78b**
at [the approved image](duck-migration/concepts/2026-09-12-approved/board-art-approved.png).
Three biomes and eight shelters remain, remapped 2/3/3 across 50 spaces; the
wasteland direction is two Feathers at each earlier haven and three at the
endpoint haven. The selected
image is 1536 × 1024; the requested 3072 × 2048 master, exact 50-space fit and
final table remain open. The token-family style is approved, with default ‘1’
badges removed in the [new study](duck-migration/concepts/2026-09-13-obstacle-study/README.md).
It contains revised ordinary tokens, Tailwind →2/→4/→6 and five white obstacles.
These are concept sheets, not imported or production-ready sprites.

The study proposes seven helpful powers and four regular mild nuisances, with
an optional Grumpy Goose. Eight whites plus five colours and five safe Exhaustion
(sixth worn out) are working proposals, not finalized rules. Exact effect-free
bag enumeration gives 7.78 placements/2.78 colours on average when stopping at
white five; it is not full-game balance evidence. Log affects next-colour movement;
Pebbles makes only its own safe resting space one Sleep less comfortable.

The tested base game remains a reference. New encounter/event rules need no
one-to-one conversion of its eight categories or 24 fortunes. Reuse Core/CLI
architecture. No Core, Unity, scene, import or gameplay implementation has started
for this direction. Short-match settings remain deferred.

Next bounded work is rules specification, Core plus CLI, Unity fit, a playable
full-Day/Dream loop, ten-Day AI and balance, then full verification. Preserve
unrelated work and the pre-existing ProjectSettings preload removal. Root owns
Git and the milestone checkpoints.

**Stop after this planning and token-art checkpoint.** Wait for the user's next
milestone command before implementation or Unity work.

## Completed playable baseline

Updated: 10 September 2026, after the full Unity match and iOS export.
The agreed initial playable milestone is complete.
Read IMPLEMENTATION_GOAL.md for scope, RULES_REFERENCE.md for primary rules, and
PROGRESS.md for validation. Test tubes and separate AI-history UI are deferred.

## Baseline run and workflow

The completed baseline was developed on codex/initial-playable-scene;
current migration work uses codex/duck-game-milestone-0. GitHub: geoking/quackies.
Open unity/Quackies.Unity in Unity6000.6.0f1. From Edit mode choose
**Quackies → Build and Play Initial Scene**. Run Core checks with
`dotnet test Quackies.sln`; CLI supports `--starting-rubies 0`.
Root owns Git: small Core source, separate tests, separate compiling Unity
checkpoints; push each, preserve unrelated files, no merge/force push.

## Baseline published state — 10 September 2026

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

Unity MCP was unavailable at the baseline handoff. Inspect live connector and
agent state when Unity work is next authorized; prefer the Unity connector
before UI automation. The reusable validation harness is tracked at
`tools/validation/QuackiesUnityFullMatchProbe.cs`; local run output is under
`Temp/QuackiesValidation/`, with canonical evidence under
`tools/validation/evidence/2026-09-10/`. Do not describe the recorded GameView
captures as physical-device evidence.

Shell push authentication fails; GitHub connector publishing works. The lead
publishes committed checkpoints without force-pushing and verifies that local
and remote trees match. Temporary publishing helpers are session conveniences;
do not rely on ignored Temp files surviving a reboot or cleanup.
