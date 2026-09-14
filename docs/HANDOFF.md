# Quackies handoff

## Current task: M3 refinement open; route count unresolved

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
Havens are 3/11/19/27/29/37/44/50; endpoint is 21 Sleep/9 Twigs/2 Feathers.
Dawn gifts cap at 3; default start bounds to 46, or 49 with starting setting 3.
Numeric data, event effects and the bounded math audit have not changed.

There is no persistent save implementation yet. M4 must preserve authoritative
bag/deck/random continuation, exact previews, final-Day commitment state,
action freshness, frozen rewards and purchase counts across local restore.
Keep Core free of Unity/filesystem dependencies and retain the original engine
profile as a regression reference while substantially refactoring duck rules.

**M3 is reopened for refinement. M4 is unstarted and waits for the user's
command.** The prior fixed-data scene and previous revised-M3 captures are
historical/rejected. The current `board-layout.json` is a provisional 40-space
visual fixture, not the current rules handoff. The user is deciding between 40
larger, 45 smaller and 45 with an extended painted route; canonical v1 data
remains 50 spaces until that choice.

[Historical M3 evidence](duck-migration/m3/README.md) records 1133 × 744
Adventure, occupied, Dream and inspection captures, two rebuilds, pointer
navigation and a zero-error Console query. It remains useful history but is not
evidence that the revision is complete. The new requirements and evidence
checklist and [validation record](duck-migration/m3-revision/validation.md) are
historical for the rejected presentation. The current refinement brief and
[candidate validation](duck-migration/m3-refinement/validation.md) are in
[m3-refinement/README.md](duck-migration/m3-refinement/README.md). Candidate
compilation, Console, rebuild, texture, dual-viewport and 40/40 pointer checks
pass. The route-count decision and M3 approval remain open. There is no live
Core binding or duck gameplay. M4 is Core/CLI, M5
connects the Unity game, and M6 covers balance/export; stop for review between
milestones.

The current refinement uses a provisional 40-space fixture at the approved
board source size. The detailed 3072 × 2048 master is explicitly deferred;
retain the higher-resolution authoring plan and 4096 import cap,
resolution-independent UI and native production sprites. Candidate validation
passes actual 1133 × 744 and 2732 × 2048 audits, 40/40 center raycasts and
PointerClick inspections. Compare 108
× 79.2 wells with 90 × 66 and compare the current 40 candidate with the user-
question 45 alternatives; they are not promised generated deliverables.
Candidate centers must follow painted paths through wasteland curves, bridges
must remain tile-free, leaving approach/deck gaps between spaces 13/14 and
26/27. Future duck animation needs explicit bridge waypoints rather than
straight center interpolation. Proposed haven IDs are
3/10/15/20/24/29/33/40 with bottom entries aligned at 20/33. Canonical v1
data remains 50 spaces with endpoint 21 Sleep / 9 Twigs / 2 Feathers.
Same-biome tile colour, green leafy nest borders and integrated Feather 1/2
treatments are covered by candidate evidence. Candidate reward labels are
visual-only remappings; canonical v1 reward data is unchanged. Dream
Concept-B likeness and fun, engaging typography are minor M5 implementation
follow-ups when the Dream view is built. Root owns any Sleep/Twig/haven row
reassignment needed by geometry.

Preserve the unrelated ProjectSettings preload removal and other pre-existing
changes.

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
