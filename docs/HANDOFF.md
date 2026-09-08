# Quackies handoff

Updated: 8 September 2026. Read this alongside `PROGRESS.md` and
`IMPLEMENTATION_GOAL.md`. This is an unfinished game; the full goal remains active.

## Run and review

- Working branch: `codex/initial-playable-scene` in `geoking/quackies`.
- Open `unity/Quackies.Unity` in Unity 6000.6.0f1.
- From Edit mode, choose **Quackies → Build and Play Initial Scene**.
- Core and CLI checks: `dotnet test Quackies.sln`.
- CLI house setup: `dotnet run --project src/Quackies.Cli -- --starting-rubies 0`.
- The lead publishes small source, test and Unity checkpoints separately.
  Check Git status before editing; Unity can regenerate project files.

## Completed and published

- Nine-round Core foundation, Set 1 ingredients, finite supply, legal actions,
  shopping, rewards, flask, round-six chip and round-nine commitments.
- Initial saved Unity table, source-art catalogue, background camera, corrected
  pot marker alignment, draw/stop/purchase callbacks and restart.
- Normal AI declines potentially explosive draws and conserves its flask using
  the agreed round-dependent remaining-bag threshold. Six tests include 112
  complete matches across the 14 implemented fortune cards without explosions.
- Core settings support starting with zero or one ruby; the official default
  remains one. Core exposes a fixed starting bag and full structured history.
  CLI uses those settings, Normal AI, recent history and human round advancement.
  Source checkpoint: `985c092`; focused settings/history tests are pending.
- User checkpoint `cd3ed90` saved the action visibility and text-layout changes.
  Fresh runtime verification of all dynamic layouts is still required.

## Work in progress now

1. Unity worker: add a full-screen CPU pot and a live scoreboard using `board.png`,
   with visible open/back controls, player counters and the round marker.
2. Core worker: regression tests for starting settings, immutable starting bags,
   complete detached history and CLI input behavior.
3. Lead: review, publish each checkpoint and maintain this handoff. Next Unity
   change is fixed-position draw/stop/flask controls using the supplied flask art.

Workers may have stopped after a usage interruption. Check live agent status and
the working tree before restarting work; saved files are more reliable than an
old status message. The lead owns Git and only one worker mutates Unity at a time.

## Still required

- Stable primary controls: the flask appearing must not move Draw or Stop. Use
  artwork and readable labels; unavailable controls should have a clear state.
- Complete all 24 fortune cards and enable the deck in the normal game. The
  default deck is currently empty, which causes the blank fortune view.
  Fourteen cards exist; the ten remaining are listed in `RULES_REFERENCE.md`.
- Show the active fortune artwork, title and complete readable rules in its view.
- Settings pane: Normal only, starting-ruby house default off, optional test tubes.
- Implement all test-tube track choices/rewards and show the full cauldron when
  enabled. No new ingredient sets before this base-game variant is complete.
- Full AI history pane, accessible from the CPU board.
- Bind Unity's starting-bag reference to Core's fixed `StartingBag`, not a live
  remaining-bag observation or a snapshot taken after a preparation reward.
- Correct ordinary rat steps to begin in round two. Audit the remaining fortune
  timing questions recorded in `RULES_REFERENCE.md` and test their interactions.
- Consolidate obsolete prototype engine/tests; keep CLI and Unity on one Core.
- Verify full matches, all new buttons, scrollable choices and long text at iPad
  mini landscape. Rebuild the scene and check compilation/runtime errors.
- Attempt iOS export. Only command-line developer tools were selected at the last
  check; an iPhone SDK and a signed physical-device run are not verified.

## Before a usage pause

Update completed/in-progress/remaining work here, record test evidence and known
failures in `PROGRESS.md`, and publish coherent checkpoints. Check limits between
milestones and prepare this handoff before the final 1% where possible: an abrupt
service limit can prevent a final conversational update. Never mark the full goal
complete merely because a limit is near.
