# M4.5 working record

15 September 2026. **M4.5 is complete and accepted by the user**; see the [completion audit](COMPLETION_AUDIT.md).
M5 Unity work remains outside this milestone. The user-approved price adjustment
is now implemented; other rules and board rewards are unchanged. See the
[promotion record](PROMOTION.md) for the current revision 2 baseline and legacy
save compatibility. The user accepted the results and game examples, explicitly deferring hands-on
gameplay feel testing to Unity. M5 waits for a separate command.

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
mutation, DLL sync or device test has occurred in M4.5. Approved price changes
were applied only after user review.

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
These initial results precede the final AI correction below. Complete Night bundles execute consistently;
remaining concerns include search horizon in large bags, final-Day score
pressure, late movement valuation and the relative strength of Reeds.

A concrete pilot replay (seed 20, original seats) had Normal trail 32–40 Twigs,
reach haven 10 with its first Day 10 draw, and settle after a capped three-draw
search. The final score was 38–50. This warrants further AI work before the
validation set; it is not evidence to change the game's rewards.

## Current ownership and remaining work

The final-Day correction is committed in `32b5233`, with focused regressions in
`feebf2c`. The final native Release suite passes **328/328**, with zero build
warnings/errors; [test log](evidence/final-ai-suite.txt), [build log](evidence/final-ai-build.txt). It compares conservative final score bounds and keeps an optimistic
remaining-bag recovery open beyond the completed search horizon only when the
current rest is certain to lose. Exact lethal previews still stop. The seed 20
replay now continues, wears out and loses 36–50: better decision purpose does
not promise a favorable random outcome. Transposition caching was rejected
because it increased runtime without improving completed depth.

Fresh approved-price validation completed **3,600 matches**, six strategy pairs,
300 seeds per pair, both seats. [Summary](evidence/holdout-summary.md) and
[manifest](evidence/holdout-manifest.json) preserve results and commands.
Normal beats the frozen M4 baseline 505–92 with three draws, but unchanged prices
still strongly favor Reeds: movement loses 536 of 600 focused matchups, and
Normal loses 404 of 600 against Reeds. No holdout result was used to retune AI.
[Final checks and full CLI/Continue evidence](evidence/final-ai-validation.md)
include the seed-42 result, AI 56–46 Human, both controlled by Normal.

The [initial price probes](price-probe-initial/REPORT.md) were insufficient.
Final-source development then compared C (Tailwind 4/8/12, Reeds 7/12/17), D
(only Reeds 8/14/20), and E (Tailwind 4/8/12, Reeds 8/14/20). E gave the closest
worst-pair result across the three-policy triangle: Normal/Reeds 40.5%,
Normal/movement 47.75%, movement/Reeds 46.75%, counting ties as half. It was
selected **before reading fresh E results** for 300-seed validation of all three
pairs. That validation completed 1,800 matches: movement/Reeds 46.6%,
Normal/movement 46.7%, Normal/Reeds 44.4%. The [review report](REPORT.md)
records the recommendation. These experiment copies remain frozen evidence.
The user approved E: Core checkpoint `c636390` and tests
`a60ec91` apply its exact prices. The final suite passes **332/332**, and all
144 matched integration games reproduce E outcomes. Legacy saves keep their
original prices; new games use rules revision 2.

The user confirmed that Reeds and movement should be fairly evenly balanced,
with neither an automatic winning choice and the oasis optional. The lead owns
all integration, evidence, final review and Git. No Unity work is authorized.

The [review report](REPORT.md) contains comeback/oasis/purchase/event/pace
findings, uncertainty and representative game stories. The user approved the
exact prices and subsequently accepted closure of M4.5, with hands-on gameplay
feel testing deferred to Unity. **M4.5 is complete.** M5 remains unstarted until
the user's command. [Completion audit](COMPLETION_AUDIT.md).
