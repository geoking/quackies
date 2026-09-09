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

- MatchSession implements nine-round Set1, finite supply, unlocks, flask, rats,
  evaluation and Normal AI. Core remains Unity-independent.
- Normal AI avoids risky draws, conserves flask according to round. Testsc3334f1
  cover every available fortune batch plus32 mixed-deck nine-round matches.
- Twenty-three fortune cards implemented. **Default deck remains empty.**
  A Second Chance is missing; Strong Ingredient needs the correction below.
- Well Stirred7993bca/tests5da103a: first placed white can return without flask,
  including a white selected through blue; no repeated offer after redrawing.
- Strong Ingrediente3c9bba/testseafab38: sequential final previews in start-player
  order, protected placement, skips exploded/full/empty players. Immediate action
  suppressed. Its own deferred ingredient effect is still incorrectly enabled.
- Full-pot blue45991dd/testsa210bca: no further blue-selected placement at52,
  including nested blue. Spoon scoring remains unchanged.
- Final-round source14cecc3/testsfe02c97: safe pots get rawVP + floor(coins/5);
  exploded pots automatically get max(rawVP,floor(coins/5)), no choice/shopping.
  Ten tests pass. Earlier rounds retain the strategic explosion choice.
- Unity: fixed primary controls/starting bag, live scoreboard/full CPU pot,
  settingsb9967eb (Normal, zero-ruby default, pending settings Apply & restart),
  dice2b093f3 (actual Core outcomes/art, round-labelled review), fortune caption
  a41f413 (title below unobstructed artwork, clickable full reference).
- Latest combined focused run:27 tests pass (10 final-fortune,10 scoring,7 AI).
  Then2 full-pot blue tests passed separately. Run full suite at next Core milestone.
- Unity currently embeds Core6a4294c, so recent rules fixes require a DLL sync.

## Next work, in order

1. Correct Strong Ingredient's deferred effect. Publisher Herb Witches p4 says
   the placed chip's action is not carried out, not merely its immediate action.
   Keep physical position and white total; exclude this chip's own green ruby or
   purple/black evaluation contribution. Preserve earlier chips' last-two physical
   positions. Root chose this direct reading; prior green-benefit test must change.
   A per-placement effect-enabled flag can reset naturally when the bag resets.
2. Implement A Second Chance: brewing-start snapshot, first five actual placements,
   protection for the card-driven draws, one keep/restart choice after placement
   choices drain, restart without repeating preparation/round-six setup. Restore
   flask from brewing-start state. See updated worker /tmp handoff if available.
3. Enable all24 in standard RuleSet.SetOne. Old focused fixtures should explicitly
   opt out with an empty deck. Verify no repeated cards and complete AI matches.
4. Sync published Core, compile Unity, verify active fortune art and long choices,
   settings/dice and full nine-round interaction, restart/rebuild, iPad layout.
   Fix pending header phase wrap (SHOPPING currently broke across two lines).
5. Complete rules/architecture audit and supported iOS export attempt. Report
   Editor success, iOS export and physical-device evidence separately. No device
   install/public release requested. Do not mark complete with missing evidence.

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
