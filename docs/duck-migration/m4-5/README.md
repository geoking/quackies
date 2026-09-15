# M4.5 working record

15 September 2026. M4.5 is authorized and active; follow [the plan](PLAN.md).
M5 Unity work remains outside this milestone. Rules, prices and board rewards
remain unchanged while AI quality and balance are assessed.

## Committed engineering checkpoints

| Checkpoint | Evidence |
| --- | --- |
| `a0be5f4`: shared authoritative encounter transition | Isolated original AI/runtime checkout passed 82 Adventure/Day/Night regressions; [log](placement-validation.txt). |
| `17363b6`: contingent Normal planning and complete Night bundles | Current/future Signpost observations, optional future settlement, completed depths up to 6 under one 8000-node total budget. |
| `674ae8f`: planning regressions | 39 focused policy/placement cases, including good continuations, prudent stops, information value and whole-bundle/skip choices. |
| `3922aeb`, `3a2315f`: real-Core evaluation runner and tests | Nine tests cover accounting, privacy, exact issued actions, seat assignments and schedules. |
| `dd71992`, `ec28b20`: paired statistical analysis and tests | Seven independent analysis tests; duplicate/incomplete records rejected. |

Root Release validation passed all **322 tests** at `3a2315f`. The frozen M4
baseline algorithm was checked exactly against `327f751` after only its
documented namespace/class/decision-DTO adaptations. The evaluation runner
and policies receive observations and issued actions; they never use saves to
choose a draw. Saves are optional review artifacts.

GitHub pushes are authenticated and working. Three pre-existing Unity font and
ProjectSettings modifications remain outside these checkpoints. No Unity
mutation, DLL sync, rule-data retuning or device test has occurred in M4.5.

## Evaluation state

[Run protocol](RUN_PROTOCOL.json) declares pilot seeds 0–23, development 1000–1099
and fresh holdout 10000–10299. Each selected pair plays both seat assignments.
Equal-policy swaps duplicate the same seeded gameplay and are treated as one
seed cluster, never as independent statistical evidence. Source labels identify
the pinned gameplay/evaluation revision; source assembly metadata is also saved.

The preserved Debug pilot contains 48 complete games: new Normal won 42, with
2.3% one-draw Days versus 22.3% for the frozen baseline. Wear-outs rose from 2.5%
to 11.0%; no oasis arrivals occurred. These are pilot results, not a balance or
fun certification. [Raw compressed records](evidence/pilot-baseline-normal-0-23.jsonl.gz)
and [summary](evidence/pilot-summary.md) preserve the observations.

The first development set and declared follow-ups completed 2400 games across
eight strategy pairings and four additional scheduler checks.
No fresh holdout has run yet. Complete Night bundles execute consistently;
remaining concerns include search horizon in large bags, final-Day score
pressure, late movement valuation and the relative strength of Reeds.

A concrete pilot replay (seed 20, original seats) had Normal trail 32–40 Twigs,
reach haven 10 with its first Day 10 draw, and settle after a capped three-draw
search. The final score was 38–50. This warrants further AI work before the
validation set; it is not evidence to change the game's rewards.

## Current ownership and remaining work

The AI worker owns a bounded follow-up in `DuckNormalPolicy` and focused tests:
avoid repeated hypothetical states and improve provably losing final-Day
settlements. The lead owns evidence, analysis, review, documentation and Git.
The initial evaluation runner is stable. An independent worker is testing two
price hypotheses only in isolated archives of the initial candidate: Tailwind
5/10/15 → 4/8/12 and Reeds 6/11/16 → 7/12/17. Each uses the declared development
seeds against matched controls. These are diagnostic experiments, not changes
to approved prices, and will need reassessment after the AI follow-up. No
worker may change canonical rules in the working project.

Remaining gates: finish AI follow-up and focused comparisons; validate important
matchups on fresh seeds; report comeback/oasis/purchase/event/pace findings with
uncertainty; preserve representative game stories; collect human playability
feedback; review any exact balance proposals and apply only agreed changes;
then final checks and milestone review. **M4.5 is not complete.**
