# Quackies handoff

Updated: 9 September 2026, after the full-pot checkpoint. Goal remains active.
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

1. Finish the live Unity full-match probe. Temporary harness:
   /tmp/QuackiesUnityFullMatchProbe.cs; report:
   /tmp/quackies-unity-full-match-probe.txt. Terra is launching it after the latest
   DLL rebuild. Check live agent/process state before resuming; do not restart a
   still-running probe based only on a stale file. It uses NormalPolicy for human
   decisions and dispatches matching real Button pointer events, alongside the
   existing Normal AI. Verify nine distinct fortunes, final score, settings
   restart, references/long choices, scoreboard/CPU return and runtime errors.
2. Capture and inspect the active fortune/full table and final results at iPad
   mini proportions. Existing temporary image paths below are earlier checks;
   do not describe them as proof of full-deck completion.
3. Attempt iOS export with the new DLL, using the initial scene and an ignored
   Builds/iOS output. iOS module is installed. Report toolchain failure accurately
   if export fails; no signing/install/device test or public release requested.
4. Update this handoff, progress and README with actual final evidence. Audit
   requirements before marking the goal complete. Do not merge main.

The rules sweep is recorded in RULES_AUDIT.md, including corrected boundaries and
explicit Second Chance timing interpretations. Core must remain independent of
Unity. Pre-existing prototype compatibility types are retained; current gameplay
uses MatchSession exclusively.

## Ownership and recovery

Workers complete_fortunes (Sol/high) owns Core exceptAI, CLI and focused card tests;
settings_and_dice (Terra/high) owns Unity and is sole Editor mutator. Workers do
not delegate or commit. Check live agents and Git status after interruptions.
Unrelated generated Assembly-CSharp.csproj changes must stay out of checkpoints.

Unity MCP tools are available again. Use `capture_game_view` with source=screen
in Play mode for overlay UI; screenshot/camera omits it. Verified source=screen
captures are Assets/Temp/Quackies/dice-reward-validation.png and
fortune-caption-validation.png (both at1133×744). Root visually inspected the die
reward. If tools disappear, /Users/george/.unity/bin/unity connects via CLI;
commands take positional arguments. Editor was stopped at the last handoff.

Shell push authentication fails; GitHub connector publishing works. Session stores
publishCheckpointJs/checkpointExtractChunkedCmd contain helpers. If lost, load
/tmp/quackies-publish-helper.js and /tmp/quackies-publish-extract-command.txt.
They publish already committed HEAD via GitHub, verify identical trees, and align
local HEAD with canonical remote commit. Large scene payloads are chunked.

Usage exhausted before the previous window could save this update. The current
window has resumed; check usage between checkpoints and save before it is near
exhaustion. Quota is not grounds to mark the goal complete.

Latest window reached 87% usage before this checkpoint; save probe state and
results before the next interruption. No Core/CLI/test edits remain unpublished.
