# M4 implementation record

14 September 2026. **M4 is complete.** The user accepted C1–C3 and authorized
finishing C4/C5. The ten-Day Core/CLI game, Normal AI and save/Continue now pass
the isolated committed-tree Release build and all 286 tests. Stop for milestone
review before M5 Unity integration. No Core DLL was synced into the scene.

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
legality at execution. On restoration, a new command scope rejects
old in-memory commands; authorization objects are not part of game saves.

This is shared engine infrastructure, not a second public engine or a generic
rules language. Classic rules stay behind their compatibility profile; new duck
phase handlers implement only the changed Adventure/Dream lifecycle.

## C1–C5 implementation result

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
  values were calculated for fixtures at that checkpoint; C4 now applies final
  results and winners in complete matches.
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
- C4 calendar source/tests are 9eba093 / 48f1dca; C5 persistence is
  71d731e / 70be133; Normal policy is c723812 / 7e5ca37; CLI integration is
  6bad090 / 129b2ba. New resume integration tests cover seeds 0/42/137, 427
  after-action JSON restores, 72 purchases, 11 private preview decisions and
  6 pending Day 10 commits. The isolated final Release build passed with zero
  warnings/errors and 286/286 tests at `cc4c11a`. The [final log](m4-final-validation.txt)
  and [completion audit](COMPLETION_AUDIT.md) map the finished requirements to
  evidence. Later checkpoints expose calendar-driven nest levels and correct
  Normal's Most Rested estimate to exclude worn-out rivals (`5ff6996` / `cc4c11a`).
- CLI defaults to human versus Normal; --two-player is a developer mode.
  --demo-game runs a full match with Normal on both sides; --demo-day is the
  earlier Day 1 → Night 1 → Day 2 script; --inspect prints the catalogue.
  Interactive save/continue uses autosave by default, --save selects an
  explicit path, --continue resumes, --new-game starts fresh, r restarts and q
  quits. Demos are ephemeral unless --save or --continue is supplied; an existing
  interactive save offers Continue/New. Unity remains M5.
- Git checkpoints are local while shell GitHub credentials remain unavailable.
  Push attempts fail with `could not read Username for 'https://github.com'`.
- Preserve the unrelated `ProjectSettings.asset` preload-removal change.

Play human versus Normal from the repository root (interactive autosave is on):

```sh
dotnet run --project src/Quackies.Cli -- --profile ducks --seed 42
```

Add `--continue` to resume the saved match, or `--new-game` to explicitly start
fresh. `--two-player` enables developer control of both ducks. `--inspect`
prints the opening catalogue; `--demo-day` retains the short scripted cycle.

Run the full ten-Day Normal-versus-Normal demonstration:

```sh
dotnet run --project src/Quackies.Cli -- --profile ducks --seed 42 --demo-game
```

The saved [full-game output](cli-full-game.txt) and [final save](full-game-save.json)
were produced from the isolated `cc4c11a` build, using `--save` with an explicit
evidence path and `--new-game`. AI finished with 29 Twigs, Human with 27; both
had 12 frozen final Sleep and four Dream Twigs. The [policy review](POLICY_REVIEW.md)
records decision checks and the remaining balance limitations.
