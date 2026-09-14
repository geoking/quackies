# Quackies handoff

## Current task: M3 complete; awaiting user review

14 September 2026: the user explicitly closed M2 and approved the remaining
rules/defaults and local autosave/Continue scope, with two corrections:

- **Days 1–9:** independent visible Draw/Settle actions; reacting to other ducks'
  completed progress is intended. **Day 10 only:** hidden simultaneous choices
  and atomic reveal after all active ducks commit. Previews remain private.
- **Final rank:** total Twigs including Dream Twigs, then frozen retained Night 10
  Sleep before conversion, then a draw if still equal. Include eligible bonuses
  and worn-out halving; do not add a safe-only filter or distance tiebreak.

The [plan](duck-migration/PLAN.md), [implementation plan](duck-migration/IMPLEMENTATION_PLAN.md),
[recap](duck-migration/RULES_AT_A_GLANCE.md) and [timing](duck-migration/ENCOUNTER_RULES.md)
now form an approved contract. Starting Feathers are a shared 0–3 setting,
default 0. First draw is required; empty-bag/space-50 finishes resolve the full
final chip and Exhaustion. Unlimited shop stock, one purchase per token type
within 1/2/3 nest capacity, Sleep expiry, no separate recovery, and safe/worn
payout details are approved. Night 10 conversion and Most Rested are unchanged.

All [50 rewards and 11 prices](duck-migration/v1/BOARD_AND_SHOP.md), encounter
powers and [ten World Events](duck-migration/v1/WORLD_EVENTS.md) remain approved.
Havens are 7/13/21/27/32/38/44/50; endpoint is 21 Sleep/9 Twigs/2 Feathers.
Dawn gifts cap at 3; default start bounds to 46, or 49 with starting setting 3.
Numeric data, event effects and the bounded math audit have not changed.

There is no persistent save implementation yet. M4 must preserve authoritative
bag/deck/random continuation, exact previews, final-Day commitment state,
action freshness, frozen rewards and purchase counts across local restore.
Keep Core free of Unity/filesystem dependencies and retain the original engine
profile as a regression reference while substantially refactoring duck rules.

**M3 is complete. M4 waits for the user's command.** Open **Quackies → Build
and Play Duck Layout Proof** for the separate fixed-data scene. It has the
approved board, 50 stable spaces, eight haven links, all 16 token variants,
duck/zzz/Feather samples, enlarged inspections and all 11 Dream offers. Native
painted havens at 21/38/50 remain visible; the oasis endpoint has two Feathers
and 21 Sleep / 9 Twigs beneath it. Moon/Sleep replaces the coin treatment.

[M3 evidence](duck-migration/m3/README.md) records 1133 × 744 Adventure,
occupied, Dream and inspection captures. Two rebuilds share a hierarchy digest
and identical metadata for all 14 textures. Compilation, layout/text checks,
pointer navigation and final zero-error Console query pass. Play mode is stopped.
There is no live Core binding or duck gameplay. M4 is Core/CLI, M5 connects the
Unity game, and M6 covers balance/export; stop for review between milestones.

Approved art remains unchanged. The board is native 1536 × 1024; the requested
3072 master remains production work after review of the measured M3 fit. Preserve the
unrelated ProjectSettings preload removal and other pre-existing changes.

No Core/CLI changes, runtime tests, Unity calls/imports, scene building or art
generation occurred in the M2 closure. Root owns the regular Git checkpoint/push;
no merge, release or device install is authorized.

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
