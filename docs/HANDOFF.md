# Quackies handoff

## Current task: M4 through C3

14 September 2026: the user approved the final visual result and explicitly
closed M3. M2 remains complete. **The user subsequently authorized M4 C1–C3,
then a progress report. Do not proceed into C4/C5 or Unity integration.** See [PLAN.md](duck-migration/PLAN.md) and the detailed
[C1 first checkpoint](duck-migration/IMPLEMENTATION_PLAN.md#c1--the-first-work-after-m4-approval).

The accepted proof has 43 spaces split 14/14/15, with havens at 4, 10, 16, 21,
26, 32, 36 and 43. The full first-haven payload moved from 3 to 4; the canonical
JSON/CSV now match all Unity reward rows. Endpoint 43 gives 21 Sleep / 9 Twigs /
2 Feathers. Shop prices, event catalogue and encounter powers remain unchanged.

Visual checkpoints: `3bd9ce1` route/outlined typography; `d068229` scattered
twigs, central chips and clearer Feathers; `2c7cd6a` stronger reward numerals.
The [M3 closeout](duck-migration/m3-closeout/README.md) records approval, accepted
assets, data agreement and final checks. The fixed-data `DuckLayoutProof` scene
rebuilds with **Quackies → Build Duck Layout Proof**; it has no duck Core binding.
The earlier **Build and Play Initial Scene** command belongs to the classic
reference game described below.

Preserve the accepted path centres, 108 × 84 tiles, 64-pixel chip frame, native
1536 × 1024 board and existing art/metadata. The detailed 3072 × 2048 painting is
explicitly deferred. M5 will connect Core and finish runtime Dream/nest/event
presentation using the accepted visual direction.

C1 resolved the starting contract: every duck starts at nest 0 with zero
Feathers. Safe haven rewards and Dawn thresholds (0–2 → 0, 3–6 → 1, 7–10 → 2,
11+ → 3) are the permanent sources; Most Rested is a temporary +1. The
pre-Day-10 bound is at most 42, so no effective-start cap is needed. The
setting-3 witness is historical evidence for an excluded configuration; its
proof artifacts remain unchanged. Core/CLI implementation is active through
C3; C1–C3 are complete for the bounded Day 1 → Night 1 → Day 2 slice. Runnable
checks are dotnet run --project src/Quackies.Cli -- --profile ducks --seed 42
--demo-day and dotnet run --project src/Quackies.Cli -- --profile ducks --seed
42 --inspect; outputs are saved in [the M4 record](duck-migration/m4/README.md).
C4/C5 remain unstarted; no Unity build or device export
is authorized. Preserve the unrelated ProjectSettings draft outside commits.
Current evidence and source decisions:
[M4](duck-migration/m4/README.md).

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

Shell push authentication currently fails. The GitHub connector can read the
repository; the remote migration branch was verified at `c82f7d0` during M3
closeout, behind the three local visual checkpoints above. Do not describe
those checkpoints as pushed until remote publication is verified. Preserve local
history, never force-push or merge main without a request. Temporary publishing
helpers are session conveniences, not durable workflow dependencies.
