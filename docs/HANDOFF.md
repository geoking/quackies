# Quackies handoff

Updated: 10 September 2026, after Unity scene checkpoint d40d4e4. Goal remains
active.
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

## Remaining integration work

1. Finish the live nine-round Unity interaction probe on scene checkpoint
   d40d4e4. Terra is creating the durable ignored
   `Temp/QuackiesValidation/` harness after the temporary `/tmp` probe vanished
   during reboot. Verify nine distinct fortunes, final score, settings restart,
   references/long choices, scoreboard/opponent-pot return and runtime errors.
2. Capture and inspect the active fortune, full table and final-results views at
   iPad mini proportions. No new UI completion claim is recorded until those
   checks produce durable evidence.
3. Attempt iOS export with the integrated DLL and report any toolchain limitation;
   no signing, installation or physical-device validation is implied.
4. Update this handoff, progress and README with actual final evidence, then
   audit requirements before marking the goal complete. Do not merge main.

The rules sweep is recorded in RULES_AUDIT.md, including corrected boundaries and
explicit Second Chance timing interpretations. Core must remain independent of
Unity. Pre-existing prototype compatibility types are retained; current gameplay
uses MatchSession exclusively.

## Ownership and recovery

Workers complete_fortunes (Sol/high) owns Core exceptAI, CLI and focused card tests;
settings_and_dice (Terra/high) owns Unity and is sole Editor mutator. Workers do
not delegate or commit. Check live agents and Git status after interruptions.
Unrelated generated Assembly-CSharp.csproj changes must stay out of checkpoints.

Unity MCP is currently unavailable. The CLI connector can reach the Unity Editor;
the Editor was stopped and ready as of 06:13 UTC. Use the durable validation
harness under `Temp/QuackiesValidation/` when Terra finishes it. Existing image
captures are earlier checks and do not prove the full-deck interaction flow.

Shell push authentication fails; GitHub connector publishing works. Durable
helpers are `Temp/QuackiesValidation/publish-helper.js` and
`Temp/QuackiesValidation/publish-extract.txt`; root also retains the publish
helper and extraction command in its session stores.
They publish already committed HEAD via GitHub, verify identical trees, and align
local HEAD with canonical remote commit. Large scene payloads are chunked.

Usage exhausted before the previous window could save this update. The current
window has resumed; check usage between checkpoints and save before it is near
exhaustion. Quota is not grounds to mark the goal complete.

Latest window reached 87% usage before this checkpoint; save probe state and
results before the next interruption. No Core/CLI/test edits remain unpublished.
