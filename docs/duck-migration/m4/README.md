# M4 implementation record

14 September 2026. The user authorized C1 through C3, followed by a progress
report. C4/C5 remain future work; Unity integration stays in M5. No Core DLL is
synced into the approved scene during this run.

## Baseline

Before source changes, `dotnet build Quackies.sln` passed with zero warnings or
errors, all 129 existing tests passed, and the classic CLI started with seed 42
and exited cleanly on closed input. Starting checkpoint: `d005223`.

## Shared command boundary

`MatchSession<TView>` owns issued-action authorization and dispatch for each
profile. An internal profile runtime owns its state and phase rules; this moves
the original body into `ClassicMatchRuntime` without copying or rewriting it.
The existing nongeneric `MatchSession` facade retains the classic API. The duck
factory will return the same typed session boundary with a `DuckMatchView`, so
clients receive real Day/Sleep/Twig fields instead of repurposed classic fields.

Commands are bound to their issuing session, player, player revision and phase
window. A repeated draw cannot silently draw twice, and another player's issued
action cannot be used. Independent opponent actions keep a still-legal command
valid; changing phase/Day/decision beat invalidates the old window. Human and AI
submit issued actions, never reconstruct IDs. Runtime validation still rechecks
legality at execution. On future restoration, a new command scope will reject
old in-memory commands; authorization objects are not part of game saves.

This is shared engine infrastructure, not a second public engine or a generic
rules language. Classic rules stay behind their compatibility profile; new duck
phase handlers implement only the changed Adventure/Dream lifecycle.

## Work in progress

Checked checkpoints: `ac1d991` shared command source, `c23eda2` four command tests;
`6cb2ccc` resumable randomness, `3a0d0c5` six continuation tests; `34f4b31` exact
duck definitions, `5ed4ada` seven catalogue tests. All 146 tests pass at the
catalogue boundary. Each push was attempted and blocked by shell authentication.

- C1: shared boundary, exact immutable catalogue, physical-chip identity and
  saveable state foundation. A separate bounded analysis checks optional
  starting Feathers against the endpoint.
- C2/C3 are not yet implemented. Final completion requires every encounter,
  every World Event fixture and a verified Day 1 → Night 1 → Day 2 CLI slice.
- Git checkpoints are local while shell GitHub credentials remain unavailable.
  Push attempts fail with `could not read Username for 'https://github.com'`.
- Preserve the unrelated `ProjectSettings.asset` preload-removal change.
