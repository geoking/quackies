# M4.5 working record

15 September 2026. The user authorized M4.5. Follow [the plan](PLAN.md);
M5 Unity work remains outside this milestone. Current rules and numeric data
stay unchanged while AI quality is improved and measured.

## Starting checkpoint

- Repository: `codex/duck-game-milestone-0`, plan checkpoint `327f751`.
- M4 Core/CLI source: `cc4c11a`, isolated Release build with 286 passing tests
  as recorded in [M4 evidence](../m4/COMPLETION_AUDIT.md).
- Baseline policy: exact `DuckNormalPolicy.cs` from `327f751`, retained under
  a distinct evaluation-only name before comparisons with the candidate.
- GitHub pushes are authenticated and working. Existing Unity font asset and
  ProjectSettings changes are unrelated and must remain outside checkpoints.

## Current work

| Owner | Files / responsibility | State |
| --- | --- | --- |
| AI worker (Sol high) | Core `Ducks/AI`, shared pure `DuckAdventureRules` extraction and its handler integration; focused planning/policy tests | In progress |
| Evaluation worker (Sol high) | `tools/Quackies.Evaluation`, solution/project references and focused evaluation tests | In progress |
| Lead | Architecture review, aggregate analysis, evidence/report, integration, documentation and Git | In progress |

The approved implementation approach shares authoritative encounter transitions
between gameplay and planning, preserving the handler's private previews,
history and phase/commitment responsibilities. Planning branches only over
observable composition and known previews, with bounded deterministic work and
the choice to settle after future draws. Shopping compares complete affordable
Night bundles within Core's type/budget/capacity restrictions.

The evaluation tool runs real Core legal actions. It will retain the baseline,
candidate and reference strategies separately and export per-Day/per-match
observations, timing and reproducible traces. Planned 24-seed pilot, 100-seed
development comparisons and fresh 300-seed validation remain unrun.

## Completion evidence still required

AI decision regressions and runtime parity; verified telemetry; baseline and
candidate comparisons; comeback/oasis/purchase/event/pace analysis; representative
game stories and human feedback; reviewed disposition of balance concerns; any
agreed corrections and final full-suite/continuation checks. Do not close the
milestone based only on compiling code or a stronger win rate.
