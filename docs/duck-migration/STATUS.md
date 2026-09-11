# Duck migration status

**M1 revision — concepts in progress.** On 11 September 2026 the user rejected
the first M1 style. The V1 Unity prototype remains technically valid evidence
but its visual direction is superseded. Branch: `codex/duck-game-milestone-0`.

## Completed work

- V1 generated assets and the separate `DuckStyleTestScene.unity` are retained
  as historical evidence in `ff2c41b`, `42e51b3` and
  [the M1 evidence](evidence/m1/README.md). Technical checks passed, but the
  style was rejected.
- Current work is limited to three concept sheets under
  `concepts/2026-09-11/`: four distinct player duck tiles, a seed encounter
  tile study, and an illustrated three-biome board.
- The proposed biome reward direction is concept only. Existing rules remain
  unchanged; new values, timing and AI effects are unspecified. Future work needs an
  explicit specification and playtest gate extending the shelter experiment.

This is an isolated visual preview. It does not create a MatchSession, implement
M2 terminology support, or change game rules. The original playable scene, Core,
CLI, raw art and shipping scene list remain unchanged. Preserve the pre-existing
ProjectSettings preload removal and keep it out of commits.

## Review gate

Inspect the three concept sheets and pause for the user's reaction. Do not import
them into Unity, alter source/settings, or start M2. The existing 54 logical
positions and next-scoring-space semantics remain the baseline; the illustrated
mapping is not count proof.

## Earlier milestone

M0 completed with a clean solution build, 129 passing tests and a CLI smoke.
Its repository map and evidence are in [BASELINE.md](BASELINE.md); plan/art brief
checkpoint `bc55369`, baseline/review checkpoint `824fa5d`. The user accepted that
milestone. The current gradual roadmap remains [PLAN.md](PLAN.md).

## Resume without repeated discovery

Wait for the user's reaction; do not begin M2 automatically. When resuming,
inspect live Git and Editor state, then read this file and the evidence. Retain
one Editor owner and small worker briefs. Do not repeat unchanged Core tests for
this visual-only milestone. Lead owns Git and pushes coherent checkpoints; no
main merge or release is authorized.
