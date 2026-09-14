# M4 implementation record

14 September 2026. The user authorized C1 through C3, followed by a progress
report. The user accepted that result and authorized completing C4/C5 and
closing M4. Calendar completion, Normal AI and save/Continue are in progress;
Unity integration stays in M5. No Core DLL is
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
factory returns the same typed session boundary with a `DuckMatchView`, so
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

## Bounded C1–C3 result

Checked checkpoints: `ac1d991` shared command source, `c23eda2` four command tests;
`6cb2ccc` resumable randomness, `3a0d0c5` six continuation tests; `34f4b31` exact
duck definitions, `5ed4ada` seven catalogue tests. All 146 tests pass at the
catalogue boundary. Each push was attempted and blocked by shell authentication.

- C1: shared boundary, exact immutable catalogue, physical-chip identity and
  saveable state foundation are checked (`dae9174` / `a286e25`). Typed CLI
  inspection shows the correct opening state. The original foundation passed
  twelve cases; the state/Night boundary passed all 174 cases before removal
  of the nonzero starting options.
- The [starting-Feather witness](starting-feather-review.md), committed as
  `c66997c`, is retained as historical evidence for an excluded setting-3
  configuration and its old Dawn bands. The current contract starts every duck
  at nest 0 with zero Feathers; no effective-start cap is needed because the
  default pre-Day-10 bound is at most 42. Current Dawn bands are 0–2 → 0,
  3–6 → 1, 7–10 → 2 and 11+ → 3.
- Night calculation is committed (`c4d4608` / `3625342`) with sixteen focused
  cases. It covers all 43 safe/worn rewards, final penalties, safe bonuses,
  collective events and duplicate-award rejection. Final-Night conversion
  values are calculated for fixtures; applying final results/winners is C4.
- C2 source/test work is committed at `3c2e73c` / `bebf161`. That source
  checkpoint referenced the then-untracked Dream handler; the standalone
  dependency was closed by the subsequent C3 source checkpoint `64f4c58`.
  The C2 worktree tests passed, but no standalone C2 checkpoint compilation is
  claimed.
- C3 Night/Dream/Dawn/CLI integration is complete for the bounded Day 1 → Night
  1 → Day 2 slice. C3 source/tests are `64f4c58` / `dc19f33`; CLI/tests are
  `2ac42de` / `754f544`. Two additional Adventure regressions (endpoint
  overshoot and final-Day wear-out preserving another draw) are in
  `673a177`. An isolated archive of that committed Core/CLI/test snapshot
  builds in Release with zero warnings/errors and passes all **241 tests**.
  [Build and full-suite output](committed-validation.txt) confirms that this
  checkpoint is self-contained, including the previously missing dependency.
- The [seed-42 cycle output](cli-day-cycle.txt) and
  [catalogue inspection](cli-inspection.txt) are saved. Both commands exited
  successfully. The cycle gives a concrete small-gap case: the AI is one Twig
  behind at Dawn and receives no Feather; both ducks receive the tied Most
  Rested temporary step and begin Day 2 with 14 chips after buying Seeds.
- C4 full-calendar play and C5 Normal AI/save-resume remain unstarted. Developer
  CLI controls are not presented as Normal AI; Unity remains M5.
- Git checkpoints are local while shell GitHub credentials remain unavailable.
  Push attempts fail with `could not read Username for 'https://github.com'`.
- Preserve the unrelated `ProjectSettings.asset` preload-removal change.

Run the bounded daily demonstration from the repository root:

```sh
dotnet run --project src/Quackies.Cli -- --profile ducks --seed 42 --demo-day
```

Replace `--demo-day` with `--inspect` to inspect the opening catalogue, or omit
both flags to control the two ducks through the developer CLI.
