# Quackies handoff

Updated: 9 September 2026. Goal remains active. See IMPLEMENTATION_GOAL.md for
the current stopping point and RULES_REFERENCE.md for the complete base rules.

## Run and review

Branch: `codex/initial-playable-scene`, repository `geoking/quackies`.
Open `unity/Quackies.Unity` in Unity 6000.6.0f1. From Edit mode choose
**Quackies → Build and Play Initial Scene**. Core checks: `dotnet test Quackies.sln`.
CLI house rule: `dotnet run --project src/Quackies.Cli -- --starting-rubies 0`.
The lead publishes small source, test and Unity checkpoints separately.

## Completed and published

- Nine-round match foundation, Set 1 ingredients, finite supply, shopping, flask,
  scoring, round-six chip and round-nine commitments; immutable actions/views.
- Saved iPad table, preserved source art, aligned pot markers and background camera.
- Normal AI avoids potentially explosive draws and conserves its flask. Six
  regressions include 112 complete matches across the first 14 fortune cards.
- Settings/history API985c092 and tests22096a0: fixed starting bags, full detached
  history and CLI behavior. All 71 tests passed at that checkpoint.
- Scoreboard and full-screen CPU pot60949b4: live scores and round, tied counters,
  fifty-point laps, open/back controls and live enlarged CPU pot. Editor verified.
- User checkpoint783cad3 includes fixed Draw/Stop/Flask slots, flask artwork and
  composed bag motif, fixed StartingBag binding and nine rat-fortune tests.
  Root pointer-handler checks verified drawing, flask return and stopping without
  replacing/moving controls, with no text overflow after the Stop-label fix.
- Rat/exchange fortunes7c59849: A Good Start, Rat Infestation, Rats Are Your Friends,
  Wheel and Deal; ordinary rats start in round two. All nine rat tests pass.
- Typed die observations6a4294c: actual face, recipient, reason, outcome and history.
  Core already resolves dice rewards; Unity presentation is being added.
- Settings paneb9967eb: Normal only, starting-ruby house default off, current versus
  next setup, Back, Apply & restart. Editor verified pending toggles preserve the
  current match and applying starts both players with the chosen setup.

## In progress

1. Core worker: remaining six fortunes and focused tests. First group is Less Is
   More, An Opportunistic Moment and Schadenfreude; then A Second Chance, Well
   Stirred and Strong Ingredient. Enable all 24 in ordinary games when complete.
2. Unity worker: actual die-result display using the supplied six face images,
   recipients, Continue and review controls. Never reroll or award rewards in UI.
3. Lead: review/publish checkpoints, then verify the integrated nine-round game,
   fortune choices/artwork, settings and die outcomes at iPad mini proportions.

The default fortune deck is still empty. Eighteen cards are implemented, and the
empty fortune view is only an explicit fallback, not completed fortune gameplay.

## Completion and later work

This milestone needs a working full fortune deck, settings and visible dice,
including ties, double-roll fortunes, empty orange supply and all-exploded rounds.
Resolve the timing questions in RULES_REFERENCE.md and verify full matches,
restart/rebuild, readable long choices and compilation/runtime errors. Keep the
CLI on the same Core and consolidate the obsolete prototype engine. Attempt an
iOS export if supported; device validation is separate and has not been performed.

The user explicitly deferred test tubes to the next goal. The separate AI-history
pane and further features remain backlog work. Do not merge to main without a
separate request or claim these deferred features are implemented.

## Resume after interruption

Check Git status and live agents first; prior workers may have stopped at a usage
limit. Preserve user commits and generated project-file changes. The lead owns
Git, and only one worker mutates Unity at a time. Check usage between milestones,
save this handoff before the last 1% where possible, and do not claim completion
merely because a limit is near.

If Unity MCP tools are absent, the installed CLI still reaches the Editor:
`/Users/george/.unity/bin/unity command editor_status --json`. Registered commands
use positional arguments, for example `command menu 'Quackies/Build Initial Scene'`.
Build Core from a published snapshot when another worker has unfinished edits;
the DLL must match the presenter's API. Current dice-capable snapshot is6a4294c.
