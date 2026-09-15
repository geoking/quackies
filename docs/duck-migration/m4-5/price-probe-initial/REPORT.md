# M4.5 exploratory price probe

## Status

Both specified price changes point toward a less one-sided movement-heavy versus
Reeds-heavy matchup under the initial 3a2315f AI, but neither establishes the
requested fairly even strategic viability. Reeds-heavy still won 152 of 200
matches under both candidates. Candidate B also narrowed normal versus
Reeds-heavy more clearly. These are development diagnostics only: do not promote
either price set without rerunning the completed AI and fresh validation.

## Method and integrity

The source was archived directly from commit
3a2315f9c64b99fde32f33b117e4991c5dcddde3. Quackies.Core contains 92 C# files
and one project file in that archive, with no Unity files or dependencies.
Each candidate was extracted independently and patched only in
src/Quackies.Core/Ducks/Definitions/DuckRules.cs. Candidate A has the three
Tailwind price substitutions 5/10/15 -> 4/8/12. Candidate B has the three Reeds
price substitutions 6/11/16 -> 7/12/17. Recursive comparisons found every other
source file identical to the base, and each source diff contains exactly three
removed and three added Offer lines.

Both isolated evaluation projects built Release from clean archives with zero
warnings and zero errors. Before batching, a seed-1000 instrumented match for
each candidate completed ten Days, two players per Day, final standings, and
full action traces (208 actions for A; 227 for B). No save artifacts exist.

Each row below is 100 development seeds, 1000–1099, with both seats: 200 matches
and 200 player-matches per policy. Policy-A score counts a tie as half. The 95%
interval resamples whole seeds and keeps the two seat assignments paired (1,200
bootstrap resamples). All runs use the CLI schedule and the actual Core runner.

## Outcomes

| Prices | Matchup (policy A vs B) | A wins | B wins | Ties | A score, paired 95% interval |
| --- | --- | ---: | ---: | ---: | ---: |
| Control 5/10/15 Tailwind, 6/11/16 Reeds | movement-heavy vs reeds-heavy | 27 | 173 | 0 | 13.50% [9.50%, 18.00%] |
| A: Tailwind 4/8/12 | movement-heavy vs reeds-heavy | 48 | 152 | 0 | 24.00% [18.50%, 30.00%] |
| B: Reeds 7/12/17 | movement-heavy vs reeds-heavy | 47 | 152 | 1 | 23.75% [18.50%, 29.01%] |
| Control 5/10/15 Tailwind, 6/11/16 Reeds | normal vs reeds-heavy | 64 | 136 | 0 | 32.00% [26.50%, 37.50%] |
| A: Tailwind 4/8/12 | normal vs reeds-heavy | 60 | 139 | 1 | 30.25% [24.50%, 36.50%] |
| B: Reeds 7/12/17 | normal vs reeds-heavy | 80 | 118 | 2 | 40.50% [34.00%, 46.76%] |

Candidate A raised movement-heavy's score by 10.5 percentage points and
Candidate B by 10.25 points on this matched development set. Both leave a large
Reeds advantage. Candidate A did not improve Normal's result; its half-tie score
fell 1.75 points. Candidate B raised Normal's score by 8.5 points, with an
interval that still includes values below an even score.

## Score gaps

Signed gap is mean policy-A final Twigs minus mean policy-B final Twigs. Absolute
margin is the within-match final gap regardless of winner.

| Prices | Matchup | Mean A | Mean B | Signed mean gap | Mean absolute margin | Median absolute margin |
| --- | --- | ---: | ---: | ---: | ---: | ---: |
| Control | movement-heavy vs reeds-heavy | 50.30 | 59.00 | -8.70 | 9.31 | 8 |
| A | movement-heavy vs reeds-heavy | 52.29 | 59.03 | -6.75 | 8.59 | 7 |
| B | movement-heavy vs reeds-heavy | 48.52 | 54.02 | -5.50 | 7.05 | 6 |
| Control | normal vs reeds-heavy | 54.27 | 59.12 | -4.85 | 7.40 | 6 |
| A | normal vs reeds-heavy | 55.15 | 60.15 | -5.00 | 7.86 | 6 |
| B | normal vs reeds-heavy | 51.28 | 54.28 | -3.00 | 6.47 | 5 |

Candidate A's movement effect came through a higher movement score while
Reeds-heavy's mean stayed flat. Candidate B lowered both sides' scores, but
lowered Reeds-heavy more and narrowed both signed and absolute gaps.

## Oasis arrivals per player-match

Counts are player-matches with at least one safe or worn-out arrival at space
43. A player-match can appear in both columns if it had both arrival types.
There are 200 player-matches in every row.

| Prices / matchup | Policy | Safe oasis | Worn oasis |
| --- | --- | ---: | ---: |
| Control movement vs Reeds | movement-heavy | 81/200 (40.5%) | 6/200 (3.0%) |
| Control movement vs Reeds | reeds-heavy | 0/200 | 0/200 |
| A movement vs Reeds | movement-heavy | 97/200 (48.5%) | 7/200 (3.5%) |
| A movement vs Reeds | reeds-heavy | 0/200 | 0/200 |
| B movement vs Reeds | movement-heavy | 52/200 (26.0%) | 4/200 (2.0%) |
| B movement vs Reeds | reeds-heavy | 0/200 | 0/200 |
| Control normal vs Reeds | normal | 13/200 (6.5%) | 2/200 (1.0%) |
| Control normal vs Reeds | reeds-heavy | 0/200 | 0/200 |
| A normal vs Reeds | normal | 21/200 (10.5%) | 3/200 (1.5%) |
| A normal vs Reeds | reeds-heavy | 0/200 | 0/200 |
| B normal vs Reeds | normal | 20/200 (10.0%) | 4/200 (2.0%) |
| B normal vs Reeds | reeds-heavy | 0/200 | 0/200 |

Oasis frequency does not track the win result closely enough to use it as the
balance target here. Candidate A increased movement-heavy oasis arrivals;
Candidate B decreased them even while producing nearly the same head-to-head
win score as A. This is consistent with the user-supplied target that oasis is
optional.

## Night purchase bundles

Every policy has 1,800 observed shopping Nights (Days 1–9 across 200 matches).
The complete bundle dictionaries are in results/aggregate-summary.json; the
following are the changes that explain the price response.

In movement-heavy versus Reeds-heavy, control movement-heavy bought 1,512
Tailwinds: 296/823/393 of values 2/4/6. It had 1,125 Tailwind-only Nights, 387
multi-purchase Nights, no three-chip Night, and one no-purchase Night. Under A it
bought 1,688 Tailwinds: 317/609/762. It had 1,206 Tailwind-only Nights, 482
multi-purchase Nights, 51 three-chip Nights, and one no-purchase Night. Thus A
mainly made the largest Tailwind and additional mixed bundles affordable. Under
B, movement-heavy's own prices were unchanged and its bundle was close to
control: 1,502 Tailwinds (282/864/356), 1,148 Tailwind-only Nights, 354
multi-purchase Nights, one three-chip Night, and two no-purchase Nights.

For the opposing Reeds-heavy policy in that matchup, control produced 996
Reeds-only, 138 Reeds+Seeds, and 435 Reeds+Splash Nights; 1,569 Reeds purchases
yielded 4,300 Reeds Twigs. A left Reeds purchasing nearly unchanged (1,564 Reeds,
4,331 Reeds Twigs) but added 22 Reeds+Tailwind and 34 Tailwind-only fallback
Nights because cheap Tailwinds are shared shop prices. B changed the bundle
shape: 838 Reeds-only, 435 Reeds+Seeds, 139 Reeds+Splash, 241 Splash-only, and
115 Wildflowers-only Nights. Reeds purchases fell to 1,412 and Reeds Twigs to
3,323. Raising the smallest Reeds price from 6 to 7 changes the common remainder
from a 4-Sleep Splash to a 3-Sleep Seed, so the result is more than a uniform
one-Sleep tax.

The same Reeds-heavy bundle shift repeats against Normal. Control had 1,002
Reeds-only, 121 Reeds+Seeds, and 445 Reeds+Splash Nights (1,568 Reeds; 4,290
Reeds Twigs). B had 827 Reeds-only, 475 Reeds+Seeds, and 132 Reeds+Splash Nights
(1,434 Reeds; 3,418 Reeds Twigs). Under A, Reeds-heavy added 20
Reeds+Tailwind and 31 Tailwind-only Nights but otherwise remained close to
control.

Normal is a mixed buyer, so normal versus Reeds-heavy is not a clean
movement-only comparison. A increased Normal's Tailwind purchases from 203 to
466, yet also changed its other bundles and gave Reeds-heavy cheap Tailwind
fallbacks. B reduced Normal's Reeds purchases from 724 to 567 as well as
reducing the opponent's Reeds output. Those interactions are reasons to treat
the head-to-head movement-heavy result as the clearer directional probe.

The top-variant purchase rates show the affordability thresholds directly.
Rates below are purchases per 1,800 shopping Nights for that policy/matchup.

| Prices / matchup | Policy | Tailwind 6 purchases | Reeds 3 purchases |
| --- | --- | ---: | ---: |
| Control movement vs Reeds | movement-heavy | 393/1,800 (21.83%) | 0 |
| Control movement vs Reeds | reeds-heavy | 0 | 44/1,800 (2.44%) |
| A movement vs Reeds | movement-heavy | 762/1,800 (42.33%) | 0 |
| A movement vs Reeds | reeds-heavy | 0 | 52/1,800 (2.89%) |
| B movement vs Reeds | movement-heavy | 356/1,800 (19.78%) | 0 |
| B movement vs Reeds | reeds-heavy | 0 | 41/1,800 (2.28%) |
| Control normal vs Reeds | normal | 5/1,800 (0.28%) | 242/1,800 (13.44%) |
| Control normal vs Reeds | reeds-heavy | 0 | 45/1,800 (2.50%) |
| A normal vs Reeds | normal | 122/1,800 (6.78%) | 234/1,800 (13.00%) |
| A normal vs Reeds | reeds-heavy | 0 | 32/1,800 (1.78%) |
| B normal vs Reeds | normal | 6/1,800 (0.33%) | 194/1,800 (10.78%) |
| B normal vs Reeds | reeds-heavy | 0 | 30/1,800 (1.67%) |

Candidate A more than doubled movement-heavy's Tailwind 6 rate and made it a
material Normal purchase, which explains why its effect cannot be treated as a
small proportional discount. Candidate B barely changed Reeds 3 frequency in
the focused Reeds-heavy policy. Its larger effect came from the Reeds 1 bundle
threshold: Reeds+Seeds rose from 138 to 435 Nights while Reeds+Splash fell from
435 to 139 against movement-heavy, alongside fewer total Reeds purchases.
Whole-Night affordability and the remaining Sleep therefore explain more of B's
response than the nominal one-Sleep increases viewed in isolation.

## Interpretation against the target

Both probes move the focused head-to-head away from the control's 86.5%
Reeds-heavy win rate, but Reeds-heavy still scores about 76%. On this evidence,
neither candidate makes the two focused routes fairly even, and neither should
be promoted. Candidate B is the more consistent directional signal because it
improves both tested policy-A matchups and narrows both score gaps. Candidate A
specifically helps movement-heavy but does not improve Normal against Reeds.
This ranks the two fixed hypotheses for later verification; it does not justify
new numbers or a wider sweep.

## Limits and unresolved facts

- These are reused development seeds and initial-AI controls, not a holdout or
  the fresh 300-seed validation.
- The 3a2315f AI has known remaining-horizon and last-Day limitations. The policy
  now under construction may buy and travel differently, so effect size and
  even direction must be checked again on the frozen final AI.
- Both prices are global. A also helps Reeds-heavy buy Tailwind fallbacks; B
  changes Reeds purchases by Normal as well as by the diagnostic policy.
- Reeds-heavy and movement-heavy are fixed evaluation strategies, not the full
  space of credible human bag plans or opponent combinations. Fair strategic
  viability cannot be certified from these two matchups.
- The 200 matches in a row arise from 100 seeds with two related seat runs.
  Intervals correctly resample 100 whole seeds; the rows are not 200
  independent trials.
- Isolated archives lack Git metadata, so their assembly metadata reports
  version 1.0.0. The explicit source labels, direct git archive, patches, and
  hashes establish provenance.
- Zero Reeds-heavy oasis arrivals does not prove unreachable or undesirable,
  and oasis is not a required winning route under the stated target.
- No human-play evidence is included. Automated outcomes cannot establish fun
  or clarity.

The unresolved decision is whether either exact probe merits a bounded rerun on
the frozen final AI. This report makes no price recommendation and explored no
additional values.
