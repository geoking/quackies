# Quackies handoff

## Current task: v1 rules/data proposal ready for review

14 September: the latest encounter artwork is approved. The user changed Mud
to lose one active Companion, Splash to immediate-next-chip nuisance protection,
Dawn to uncapped ceil(Twig deficit/4) Feathers, and worn-out rewards to full
Twigs plus half Sleep rounded down. Day 10 safe havens add another +2 Sleep;
Night 10 converts retained Sleep at floor(Sleep/4) Dream Twigs and grants safe
Most Rested winners one extra Dream Twig. Highest final Twigs wins.

The [current plan](duck-migration/PLAN.md), [recap](duck-migration/RULES_AT_A_GLANCE.md)
and [detailed encounter timing](duck-migration/ENCOUNTER_RULES.md) replace older
Mud/rescue/stork/three-Feather-endpoint specifications. Mud's reduced active flock
controls later Companion movement and the safe Night contest without deleting
owned chips or changing previous placements. Splash can block Goose's limit
drop but never its Exhaustion. Root interpretations are labelled for review.

The [board/shop proposal](duck-migration/v1/BOARD_AND_SHOP.md) contains all 50
rows, havens 7/13/21/27/32/38/44/50, endpoint 21 Sleep/9 Twigs/2 Feathers,
11 prices and Night examples. [Ten proposed World Events](duck-migration/v1/WORLD_EVENTS.md)
are shuffled once and revealed without replacement. Exact bag/counter
[audit evidence](duck-migration/v1/balance-audit.json) is bounded, not full-game
balance. New numerical values/events/policies remain review candidates.

A [Most Rested zzz tile](duck-migration/concepts/2026-09-14-most-rested/README.md)
was generated and inspected. It covers one temporary extra start space beyond
the updated Feather trail, distinct from permanent Feathers. Nights 1–9 pass
the award; ties preserve equal benefits. No Day 11 start is awarded.

The approved [board](duck-migration/concepts/2026-09-12-approved/board-art-approved.png),
V2 player ducks, Dream Concept B and [16 encounter designs](duck-migration/concepts/2026-09-13-agreed-token-set/README.md)
remain unchanged. The selected board is native 1536 × 1024; the requested
3072 master, exact 50-space alignment and actual-size token/readability fit are
still outstanding. No Core/CLI, Unity scene or import work occurred.

**Open before implementation:** resolve uncapped permanent Feathers at/beyond
a finite route; review new data, safe-only payout interpretations and proposed
stock/no-draw/empty-bag/overshoot/final-tie/no-flask rules. No silent cap, discard,
banking or conversion may substitute for the one-Feather/one-step rule.
Reuse the existing engine and CLI. Initial scope remains human versus Normal
AI on iPad mini, ten Days. Preserve the unrelated ProjectSettings modification.

**Stop after this planning/art checkpoint.** Await review and the next explicit
milestone command before implementation. Root owns Git; no merge/release.

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
