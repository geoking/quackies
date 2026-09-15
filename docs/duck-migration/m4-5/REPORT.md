# M4.5 review — AI corrected; price decision and human feedback pending

15 September 2026. The improved AI is committed and validated. **The approved
prices do not yet meet the user's balanced-strategy target.** Reeds beats both
Normal and movement-focused buying consistently. The selected isolated price
proposal substantially closes the gap on fresh games; no new prices are active
in Core.

## What improved

Normal now plans contingent sequences of draws, including the option to rest
after a future chip, and compares complete affordable Night shopping bundles.
It uses the same pure encounter transition as the live game. Its observations
contain no hidden bag order or opponent previews.

A reproduced final-Day fault also needed correction: seed 20 stopped after one
draw at haven 10 despite a public score deficit that made that rest certain to
lose. Conservative score bounds and an optimistic remaining-bag recovery bound
now keep a possible recovery open beyond the search horizon. That particular
replay still loses 36–50 after wearing out. The fix improves the choice, not luck.

Final gameplay source is `32b5233`; focused regressions are `feebf2c`.
**328 tests pass**, the Release build has zero warnings/errors, and a complete
CLI match plus finished-save Continue agree at AI 56–46 Human for seed 42.
Both seats in that demonstration use Normal. [Validation and transcripts](evidence/final-ai-validation.md).

## Fresh games on the approved prices

Each comparison has 300 fresh seeds (10000–10299), played in both seats: 600
match records. Uncertainty resamples whole seeds so the paired seats stay
together. Equal-policy swaps reproduce the same gameplay; selfplay has 300
distinct games. Win score counts a draw as half.

| First strategy vs second | Wins–losses–draws | First strategy's win score, 95% interval |
| --- | ---: | ---: |
| Improved Normal vs M4 baseline | 505–92–3 | 84.4% [81.3%, 87.4%] |
| Normal vs movement-focused | 385–212–3 | 64.4% [60.8%, 68.0%] |
| Normal vs Reeds-focused | 194–404–2 | 32.5% [29.2%, 36.3%] |
| Movement-focused vs Reeds-focused | 63–536–1 | 10.6% [8.1%, 13.3%] |
| Deliberately stop early through Day 3 vs Normal | 217–379–4 | 36.5% [32.8%, 40.2%] |

Against the old AI, Normal draws 7.1 chips per Day rather than 4.1. One-draw
Days fall from 19.2% to 2.2%; worn-out Days increase from 2.9% to 9.4%.
The extra exploration wins more often on unchanged rules. The preserved M4
baseline reproduces its original seed-42 saved state exactly after documented
JSON representation normalization. [Baseline reproduction](evidence/baseline-reproduction.md).

These are defined buying policies, not optimal versions of every possible bag.
Movement-focused buying prioritizes movement and avoids Reeds when another
non-Reeds purchase is available; Reeds-focused buying prioritizes Reeds yield.
Both use the same improved adventure policy. Their large gap warrants a price
change experiment, but does not prove that every human movement build loses.
[Full results](evidence/holdout-summary.md), [complete aggregates](evidence/holdout-summary.json),
[source and commands](evidence/holdout-manifest.json).

## Recovery, adventure and the oasis

In Normal selfplay, a duck 3–6 Twigs behind after Day 5 still wins **42.9%** of
the time (105 distinct seeds in that cohort; interval 33.3–52.4%). After Day 9,
the same deficit recovers 22.7%; a 7–10 deficit recovers 9.5%. Large late deficits
are harder to recover. Only 17 distinct games had a Day-9 deficit of 11+, too
few to conclude that recovery is impossible from zero observed wins.

Deliberate weak opening play earns extra Dawn gifts but loses more often than
it wins. It averages 9.03 Dawn Feathers per match against 0.10 for its opponent;
the final score still averages 43.3–46.3 Twigs. This one exploit probe is evidence
to **retain the 3/7/11 Dawn thresholds provisionally**, not proof that every
possible gift-farming strategy is harmless. These outcomes do not isolate
Dawn's causal contribution from purchases, bag order or other rewards.

Normal selfplay averages 7.5 draws per Day and 11.9% worn-out Days. Resting
positions progress from a mean 6.9 on Day 1 to 24.0 on Day 9. Across all Days,
57.6% of rests are in wetlands, 35.3% in meadow and 7.1% in wasteland. The
physical seats score 48.5% and 51.5%, with both intervals including parity.

| Strategy and opponent | Ever arrives safely at oasis | Ever arrives worn out |
| --- | ---: | ---: |
| Normal against Normal | 1.0% | 0.17% |
| Movement-focused against Normal | 11.3% | 1.5% |
| Movement-focused against Reeds-focused | 35.0% | 3.0% |
| Reeds-focused against either Normal or movement-focused | 0 observed | 0 observed |

Rates are per duck per match, not per Day. Safe and worn arrivals can both occur
in one match. Movement's first safe arrivals against Reeds range from Day 7 to
10, averaging Day 9.3. Its frequent arrivals alongside poor win rate are the
clearest sign that travel currently costs too much relative to direct Twigs.
Dawn gifts are also greater against the stronger Reeds opponent, so the different
oasis rates are not purely an item effect. The oasis remains optional.

Endpoint completion and clamping are covered by Core tests. These aggregate
records do not contain unbounded proposed movement, so they do not measure the
exact number of overshoots. No speculative overshoot statistic is reported.

## Concrete game examples and events

Three preserved original-seat selfplay examples make the outcomes inspectable:

- **Seed 10042:** a duck trails 13–17 after Day 5, reaches haven 32 on Day 7,
  and wins 62–45. Both ducks wear out on Day 9; the winner still recovers with
  a safe final Night and six Dream Twigs.
- **Seed 10006:** a duck safely reaches the oasis after 19 draws on Day 10 and
  wins 66–54. Its final Night earns 28 Sleep and eight Dream Twigs.
- **Seed 10009:** both ducks finish on haven 21 with 48 total Twigs. Final Sleep
  of 18 versus 17 resolves the result, exactly as approved.

[Per-Day outcomes, purchases and raw field references](evidence/representative-games.md)
allow review without mistaking these selected examples for typical frequencies.

Goose introduction on Day 5 does not produce a wear-out spike in Normal
selfplay: Day 4 wears out 9.2%, Day 5 7.3%, while mean draws fall from 6.9 to
6.1. Bags, events and AI decisions also change, so this is not an isolated
Goose effect. Across the ten World Events, observed wear rates range from 9.7%
to 15.0%; event order and Day are confounded. The event rewards and Companion/
Mud/Splash interactions have authoritative rule regressions; these aggregate
rates alone do not justify changing their powers. Retain them for human review.

## Shopping and remaining AI limits

Normal selfplay buys 1.44 chips per shopping Night on average and leaves 0.69
Sleep unspent. The most frequent bundles are Reeds alone, Companion alone,
Reeds plus Splash, and Companion plus Splash. All seven helpful token types
are purchased, but Signpost is rare and late buying favors Reeds. Full bundles
are in the aggregates; individual variant counts alone would miss the trade-off
between a large Reeds chip and two cheaper complementary chips.

The planner considers up to six draws under one 8,000-node budget, using only
completed depths. Larger bags can finish only three depths. Its Night scoring
is still an additive heuristic; it is not an optimal full-game opponent. A
transposition-cache experiment was removed because it added runtime without
increasing completed depth. The final-Day optimistic recovery bound can justify
trying a low-probability comeback, but cannot claim that an optimistic path is
reachable in the actual hidden order.

Release selfplay policy decisions average 0.87 ms on this Mac, with an observed
24.3 ms maximum. These are bounded desktop measurements, **not iPad timings**.
Unity integration must pace decisions and measure on the target device later.

## Issue dispositions

| Finding | Category / confidence | Disposition |
| --- | --- | --- |
| Early-stop bias and greedy individual purchases | Confirmed AI limitations / high | Corrected; retained cautious-stop and bundle regressions, and fresh win comparison. |
| Final-Day certain-loss rest, seed 20 | Confirmed policy fault / high | Corrected and covered by actual full-match regression. |
| Reeds wins too consistently under approved prices | Balance concern / high for tested policies | Selected price proposal passed fresh comparison; user decision pending. |
| Limited late-bag horizon and purchase heuristics | Remaining AI limitation / high | Explicit bounded design; no optimality claim. Review against human play before closure. |
| Dawn gift farming | Potential balance concern / moderate | Tested one deliberate weak-opening strategy; no demonstrated winning exploit. Retain thresholds provisionally. |
| Oasis rare in mixed bags | Design observation / high for tested policies | Movement can reach it; reassess with proposed prices. User wants optional oasis. |
| Tension, clarity, desire to replay | Human-play feedback / missing | Still pending; automated scores do not certify fun. |

## Recommended price adjustment

| Token | Current Sleep price | Proposed Sleep price |
| --- | ---: | ---: |
| Tailwind, move 2 | 5 | 4 |
| Tailwind, move 4 | 10 | 8 |
| Tailwind, move 6 | 15 | 12 |
| Reeds, gain 1 Twig | 6 | 8 |
| Reeds, gain 2 Twigs | 11 | 14 |
| Reeds, gain 3 Twigs | 16 | 20 |

All Reeds still move one space. All other prices, effects, board rewards and
Dawn rules stay as approved. These exact values are candidate E. It was selected
from development results before its fresh validation outputs were read; no
further variants were tuned. Earlier experiments and their weaker results are
preserved rather than discarded.

| Comparison | Current first-strategy score | Proposed first-strategy score, 95% interval | Proposed wins–losses–draws |
| --- | ---: | ---: | ---: |
| Movement-focused vs Reeds-focused | 10.6% | **46.6% [42.9%, 50.3%]** | 277–318–5 |
| Normal vs movement-focused | 64.4% | **46.7% [42.9%, 50.6%]** | 278–318–4 |
| Normal vs Reeds-focused | 32.5% | **44.4% [40.8%, 48.1%]** | 264–331–5 |

This is another 1,800 complete matches, the same fresh 300 seeds and both seats
for each comparison, on a copy of frozen source `32b5233` with only the six
price literals changed. These are held-out seeds relative to candidate selection;
they are deliberately shared with approved-price controls for comparison.

Reeds retains a modest edge, particularly against mixed Normal buying. The
results support **both focused strategies being credible**, not perfect parity
or a guarantee about optimal human play. Physical-seat scores for movement
against Reeds are 49.2% and 44.0%. Normal against Reeds scores 40.5% and 48.3%;
that difference reverses the development set's direction. A universal first-seat
advantage is not established by these results.

Movement's safe oasis rate rises from 11.3% to **19.5% against Normal**. Against
Reeds it is **31.8%**, down slightly from 35.0%, while its wins rise sharply.
The latter matchup now gives movement 3.29 Dawn Feathers per game rather than
7.80 because its score deficit is smaller. Reeds wins without observed oasis
arrivals. The result preserves two different routes to victory and an optional
oasis rather than making the endpoint compulsory.

Reeds-focused final scores against movement fall from 59.9 to 50.8 Twigs;
movement averages 49.0 instead of 49.5. The adjustment mainly removes the cheap
recurring-Twig advantage. Higher Reeds prices also change affordable shopping
bundles, not just individual purchases. Normal still buys about 1.4 chips per
Night and leaves about 0.9 Sleep unspent. No new event or reward power is needed.

[Exact experiments, commands, source patches and complete results](price-probe-final/REPORT.md)
provide the reviewable proposal. All 1,800 proposed-price records passed the
analyzer's structural checks. They do not substitute for canonical-data and
save-version tests after an approved promotion.

## Remaining review

The exact proposal has been presented for user approval. If accepted, apply it
as a small Core checkpoint, reconcile canonical shop data and save rules-version
behavior, run relevant price and persistence checks, and recheck the existing
effective-start bound. The observed maximum start in proposed-price validation
is 26; that is an observation, not a proof of the legal maximum.

Human feedback on tension, clarity and purchase choices remains pending. The
representative games above are available for review; a user-played CLI match
would provide stronger feedback than reading outcomes. M4.5 remains open until
that review and any agreed corrections are complete. M5 Unity work waits for
the milestone review and the user's command.
