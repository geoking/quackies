# M4.5 final-AI bounded price diagnostic

## Status

Profile E (Reeds 8/14/20 plus Tailwind 4/8/12) is the closest of the four tested
profiles to comparable strategic viability across this development triangle.
Normal's half-tie score is 40.50% against Reeds-heavy and 47.75% against
movement-heavy; movement-heavy scores 46.75% against Reeds-heavy. Its worst
listed policy-A result is closer to parity than D's 38.25%, and the focused
movement-heavy versus Reeds-heavy interval includes 50%.

Profile C (combined mild Reeds increase and cheaper Tailwind) makes Normal versus
movement-heavy even, but Reeds-heavy still clearly beats both policies. Neither
E nor D was promoted from development alone. E was selected as the sole primary
proposal and fresh-validated below; applying it to authoritative rules still
requires user review.

E was then frozen as the sole primary proposal in SELECTION.md before fresh
outputs. Fresh seeds 10000–10299 support the selection: E's three policy-A
scores are 44.42%, 46.67%, and 46.58%, compared with the approved-price
control's 32.50%, 64.42%, and 10.58%. Two E intervals include 50%; Normal versus
Reeds-heavy remains below parity at 44.42% [40.83%, 48.09%]. This is validation
evidence for the frozen proposal, not an approved rules edit.

## Method and provenance

The specification was written before builds or evaluation. Source was archived
directly from commit 32b52333d49d99ecbce56dc25b91d490e30a74bc:
Quackies.Core plus Quackies.Evaluation only. The control extraction is
byte-identical to the base. C changes exactly six Offer price lines:
Tailwind 5/10/15 to 4/8/12 and Reeds 6/11/16 to 7/12/17. D changes exactly three:
Reeds 6/11/16 to 8/14/20. Every other source file is identical.

After those runs were preserved, root authorized E in a written specification
addendum before any E extraction, build, or match. E changes exactly six price
lines from control: Tailwind 5/10/15 to 4/8/12 and Reeds 6/11/16 to 8/14/20.
It combines the independently motivated Tailwind and D price hypotheses; no
holdout result was used.

All four isolated projects built Release from clean archives with zero warnings
and zero errors. A seed-1000 instrumented match for each profile completed ten
Days, final standings, and full action traces before the batches. No save was
requested or produced.

For every profile, the full triangle uses seeds 1000–1099, both seats, and the
CLI schedule:

- normal vs reeds-heavy
- normal vs movement-heavy
- movement-heavy vs reeds-heavy

Each row has 100 seeds and 200 matches. There are 600 matches per profile and
2,400 total across control/C/D/E. Earlier-AI controls are excluded. Policy-A
score counts a tie as half; its 95% interval uses 1,200 bootstrap resamples of
whole seeds, preserving the two related seat assignments.

## Outcomes

| Profile | Policy A vs B | A wins | B wins | Ties | A half-tie score, paired 95% interval | A score by seat: human / AI |
| --- | --- | ---: | ---: | ---: | ---: | ---: |
| Control | normal vs reeds-heavy | 68 | 131 | 1 | 34.25% [28.24%, 40.01%] | 38.5% / 30.0% |
| Control | normal vs movement-heavy | 128 | 71 | 1 | 64.25% [58.25%, 70.50%] | 66.5% / 62.0% |
| Control | movement-heavy vs reeds-heavy | 27 | 173 | 0 | 13.50% [9.00%, 18.00%] | 20.0% / 7.0% |
| C | normal vs reeds-heavy | 68 | 132 | 0 | 34.00% [27.50%, 40.50%] | 45.0% / 23.0% |
| C | normal vs movement-heavy | 102 | 98 | 0 | 51.00% [44.50%, 57.50%] | 54.0% / 48.0% |
| C | movement-heavy vs reeds-heavy | 60 | 138 | 2 | 30.50% [24.00%, 37.25%] | 29.0% / 32.0% |
| D | normal vs reeds-heavy | 97 | 102 | 1 | 48.75% [42.25%, 55.50%] | 54.5% / 43.0% |
| D | normal vs movement-heavy | 102 | 95 | 3 | 51.75% [45.00%, 58.75%] | 54.0% / 49.5% |
| D | movement-heavy vs reeds-heavy | 75 | 122 | 3 | 38.25% [31.00%, 45.26%] | 37.5% / 39.0% |
| E | normal vs reeds-heavy | 80 | 118 | 2 | 40.50% [34.50%, 46.75%] | 46.0% / 35.0% |
| E | normal vs movement-heavy | 95 | 104 | 1 | 47.75% [41.25%, 54.50%] | 56.5% / 39.0% |
| E | movement-heavy vs reeds-heavy | 92 | 105 | 3 | 46.75% [40.75%, 52.76%] | 51.5% / 42.0% |

The two D comparisons involving Normal include 50% within their paired-seed
intervals. D's movement-heavy versus Reeds-heavy interval remains below 50%;
the result supports credible wins, not even head-to-head strength. Seat effects
also matter. In D normal versus Reeds-heavy, Normal scores 54.5% in the human
seat and 43.0% in the AI seat. C is still more seat-sensitive in that matchup
(45.0% versus 23.0%). Both-seat averages must not hide this scheduler/seat
interaction.

E moves the focused movement-versus-Reeds comparison closer still, but its
Normal rows show larger seat effects: 11 points against Reeds-heavy and 17.5
points against movement-heavy. The paired averages remain appropriate for the
declared design, while the seat split is a material limit on claims of general
balance.

## Final score gaps

Signed gap is mean policy-A Twigs minus mean policy-B Twigs. Absolute margin is
the per-match gap regardless of winner.

| Profile | Policy A vs B | Mean A | Mean B | Signed mean gap | Mean absolute margin | Median |
| --- | --- | ---: | ---: | ---: | ---: | ---: |
| Control | normal vs reeds-heavy | 53.68 | 58.86 | -5.19 | 7.66 | 7 |
| Control | normal vs movement-heavy | 48.15 | 44.99 | +3.17 | 5.89 | 6 |
| Control | movement-heavy vs reeds-heavy | 49.96 | 58.82 | -8.86 | 9.42 | 8 |
| C | normal vs reeds-heavy | 51.80 | 55.81 | -4.01 | 7.08 | 6 |
| C | normal vs movement-heavy | 47.88 | 46.97 | +0.92 | 6.37 | 6 |
| C | movement-heavy vs reeds-heavy | 50.26 | 55.16 | -4.90 | 7.27 | 6 |
| D | normal vs reeds-heavy | 48.03 | 49.87 | -1.85 | 5.99 | 6 |
| D | normal vs movement-heavy | 44.77 | 44.10 | +0.67 | 5.07 | 5 |
| D | movement-heavy vs reeds-heavy | 46.98 | 50.28 | -3.30 | 5.71 | 5 |
| E | normal vs reeds-heavy | 48.60 | 50.80 | -2.21 | 5.80 | 5 |
| E | normal vs movement-heavy | 46.24 | 47.03 | -0.79 | 5.58 | 6 |
| E | movement-heavy vs reeds-heavy | 49.69 | 51.46 | -1.77 | 5.81 | 5 |

D narrows all three absolute and signed gaps, while also lowering the overall
Twig totals because Normal buys Reeds and the targeted Reeds policy earns less.
Movement-heavy's mean also falls against D Reeds-heavy (49.96 to 46.98); its
better win result comes from Reeds-heavy falling further, not from an absolute
movement score increase.

E raises movement-heavy's mean relative to D because it adds cheaper Tailwind,
and produces the smallest movement-versus-Reeds signed gap. D retains slightly
smaller absolute margins in the two Normal comparisons.

## Oasis arrivals per player-match

Counts are player-matches with at least one safe or worn-out arrival at space 43.
Each policy has 200 player-matches per row. A player-match can appear in both
columns if it had both arrival types.

| Profile / matchup | Policy A safe / worn | Policy B safe / worn |
| --- | ---: | ---: |
| Control normal vs Reeds | normal 15/200 / 2/200 | reeds-heavy 0 / 0 |
| Control normal vs movement | normal 1/200 / 0 | movement-heavy 26/200 / 1/200 |
| Control movement vs Reeds | movement-heavy 79/200 / 5/200 | reeds-heavy 0 / 0 |
| C normal vs Reeds | normal 37/200 / 3/200 | reeds-heavy 0 / 0 |
| C normal vs movement | normal 6/200 / 0 | movement-heavy 36/200 / 4/200 |
| C movement vs Reeds | movement-heavy 68/200 / 4/200 | reeds-heavy 0 / 0 |
| D normal vs Reeds | normal 11/200 / 2/200 | reeds-heavy 0 / 0 |
| D normal vs movement | normal 8/200 / 1/200 | movement-heavy 15/200 / 0 |
| D movement vs Reeds | movement-heavy 39/200 / 7/200 | reeds-heavy 0 / 0 |
| E normal vs Reeds | normal 11/200 / 1/200 | reeds-heavy 0 / 0 |
| E normal vs movement | normal 2/200 / 1/200 | movement-heavy 42/200 / 7/200 |
| E movement vs Reeds | movement-heavy 69/200 / 5/200 | reeds-heavy 0 / 0 |

D reduces movement-heavy safe-oasis matches while improving its relative result
against Reeds-heavy. Oasis frequency therefore remains a distinct behavior and
should not be used as the proxy for strategic viability. This is consistent
with the user's target that reaching the oasis is optional.

## Top-variant affordability and purchase frequency

Every policy has 1,800 shopping Nights (Days 1–9). “Budget” below means the
opening frozen Sleep for that Night met or exceeded the listed price; it does
not assert supply or post-purchase legality. Values are opening-budget Nights /
actual purchases. The focused movement-heavy and Reeds-heavy policies bought
their preferred top variant on every Night whose opening budget reached its
price.

| Profile / matchup | Policy | Tailwind 6 budget / bought | Reeds 3 budget / bought |
| --- | --- | ---: | ---: |
| Control normal vs Reeds | normal | 362 / 5 | 287 / 242 |
| Control normal vs Reeds | reeds-heavy | 139 / 0 | 45 / 45 |
| Control normal vs movement | normal | 308 / 4 | 226 / 192 |
| Control normal vs movement | movement-heavy | 321 / 321 | 200 / 0 |
| Control movement vs Reeds | movement-heavy | 393 / 393 | 286 / 0 |
| Control movement vs Reeds | reeds-heavy | 141 / 0 | 44 / 44 |
| C normal vs Reeds | normal | 704 / 140 | 217 / 195 |
| C normal vs Reeds | reeds-heavy | 497 / 0 | 20 / 20 |
| C normal vs movement | normal | 691 / 125 | 191 / 171 |
| C normal vs movement | movement-heavy | 727 / 727 | 167 / 0 |
| C movement vs Reeds | movement-heavy | 716 / 716 | 206 / 0 |
| C movement vs Reeds | reeds-heavy | 444 / 0 | 20 / 20 |
| D normal vs Reeds | normal | 380 / 15 | 80 / 78 |
| D normal vs Reeds | reeds-heavy | 140 / 0 | 1 / 1 |
| D normal vs movement | normal | 362 / 11 | 60 / 57 |
| D normal vs movement | movement-heavy | 301 / 301 | 32 / 0 |
| D movement vs Reeds | movement-heavy | 356 / 356 | 88 / 0 |
| D movement vs Reeds | reeds-heavy | 170 / 0 | 3 / 3 |
| E normal vs Reeds | normal | 691 / 130 | 78 / 77 |
| E normal vs Reeds | reeds-heavy | 463 / 0 | 3 / 3 |
| E normal vs movement | normal | 698 / 125 | 45 / 44 |
| E normal vs movement | movement-heavy | 731 / 731 | 76 / 0 |
| E movement vs Reeds | movement-heavy | 741 / 741 | 134 / 0 |
| E movement vs Reeds | reeds-heavy | 499 / 0 | 1 / 1 |

C makes Tailwind 6 affordable to movement-heavy on roughly 40% of Nights
(716–727/1,800), versus 17.8–21.8% under control. D makes Reeds 3 almost absent
for Reeds-heavy: 1–3 purchases, versus 44–45 under control. More consequentially,
D shifts purchases down the Reeds ladder. Against movement-heavy, control
Reeds-heavy bought Reeds 1/2/3 in counts 1004/521/44; D bought 1145/226/3.
E bought 1158/223/1: the D-like Reeds ladder remains, while cheap Tailwind
changes fallback purchases and the opponent's movement build.

## Whole-Night bundles

The complete Night bundle dictionary for every policy and matchup is stored
under matchups[].players[].nightBundlesByType in
results/all-profiles-summary.json. The following focused counts show the material
threshold effects; all use 1,800 Nights.

Against Reeds-heavy, control movement-heavy bought 1,512 Tailwinds
(296/823/393 by values 2/4/6). It had 1,125 Tailwind-only Nights, 387
multi-purchase Nights, no three-chip Nights, and one empty Night. C bought 1,674
Tailwinds (319/639/716), with 1,212 Tailwind-only, 462 multi-purchase, 40
three-chip, and one empty Night. D retained approved Tailwind prices and stayed
near control: 1,526 Tailwinds (268/902/356), 1,169 Tailwind-only, 357
multi-purchase, one three-chip, and one empty Night.

Against movement-heavy, control Reeds-heavy had 996 Reeds-only, 138
Reeds+Seeds, and 435 Reeds+Splash Nights. It bought 1,569 Reeds and earned 4,277
Reeds Twigs. C had 827 Reeds-only, 445 Reeds+Seeds, and 142 Reeds+Splash Nights;
it bought 1,414 Reeds and earned 3,427 Reeds Twigs. It also had 100
Tailwind-only fallback Nights because C's Tailwind 2 costs four. D had 944
Reeds-only, 169 Reeds+Seeds, 201 Reeds+Splash, and 58 Reeds+Wildflowers Nights;
it bought 1,374 Reeds and earned 2,588 Reeds Twigs. D also moved more
unaffordable Nights to other single purchases: 79 Companion, 228 Splash, and 88
Wildflowers Nights.

The pattern repeats against Normal. Control Reeds-heavy had 1,002 Reeds-only,
121 Reeds+Seeds, and 445 Reeds+Splash Nights, earning 4,258 Reeds Twigs. C had
868 Reeds-only, 453 Reeds+Seeds, and 138 Reeds+Splash Nights, earning 3,589.
D had 949 Reeds-only, 151 Reeds+Seeds, 196 Reeds+Splash, and 62
Reeds+Wildflowers Nights, earning 2,608.

Normal's total number of purchases stays similar, but its composition changes
because prices are global. Against Reeds-heavy, control Normal bought 724 Reeds
and 203 Tailwinds; C bought 525 Reeds and 529 Tailwinds; D bought 493 Reeds and
222 Tailwinds. Normal had 790 multi-purchase Nights under control, 776 under C,
and 790 under D. Thus C changes Normal into a much more movement-mixed buyer,
while D mostly removes high-value Reeds without making cheap Tailwind a dominant
substitute.

E's focused movement bundle is close to C: 1,687 Tailwinds
(294/652/741), 1,228 Tailwind-only Nights, 459 multi-purchase Nights, 42
three-chip Nights, and one empty Night. E Reeds-heavy bought 1,382 Reeds and
earned 2,748 Reeds Twigs against movement-heavy. Its full bundle counts were
961 Reeds-only, 162 Reeds+Seeds, 193 Reeds+Splash, and 66
Reeds+Wildflowers, plus 100 Tailwind-only fallback Nights.

Against Normal, E Reeds-heavy bought 1,363 Reeds and earned 2,674 Reeds Twigs:
966 Reeds-only, 151 Reeds+Seeds, 188 Reeds+Splash, and 58
Reeds+Wildflowers Nights. E Normal bought 486 Reeds and 516 Tailwinds against
Reeds-heavy, so the global cheap-Tailwind effect seen in C remains present.

## Interpretation against the user target

The control has a strict observed ordering: Reeds-heavy beats Normal, Normal
beats movement-heavy, and Reeds-heavy wins 86.5% against movement-heavy. C makes
movement-heavy comparable to Normal but leaves Reeds-heavy clearly ahead of
both. Its simultaneous price changes also obscure which threshold supplies each
part of the outcome.

D is the profile where both Normal comparisons are centered near 50%, while
movement-heavy remains disadvantaged against Reeds-heavy. E distributes the
remaining imbalance more evenly: the three listed policy-A scores are 40.50%,
47.75%, and 46.75%, and movement-heavy versus Reeds-heavy includes 50% in its
paired interval. E therefore best fits the cross-opponent comparability target
by worst observed pair in this bounded development triangle.

E's residual Normal-versus-Reeds disadvantage, large Normal seat effects, and
the same sharp reduction in top Reeds availability as D all require review.
This development evidence led to E's recorded pre-output selection for fresh
validation; it did not itself approve either profile.

## Fresh validation of frozen E

SELECTION.md records E as the sole primary proposal before any fresh E output
was generated or read. Validation uses 300 new seed clusters, 10000–10299, with
both seats and the CLI schedule: 600 matches per pair and 1,800 E matches.
Root's approved-price controls use the same source, seeds, seats, and schedule
and were completed independently. Development data is excluded from every
estimate in this section.

| Profile | Policy A vs B | A wins | B wins | Ties | A score, seed-cluster 95% interval |
| --- | --- | ---: | ---: | ---: | ---: |
| Approved control | normal vs reeds-heavy | 194 | 404 | 2 | 32.50% [29.17%, 36.25%] |
| Approved control | normal vs movement-heavy | 385 | 212 | 3 | 64.42% [60.75%, 68.00%] |
| Approved control | movement-heavy vs reeds-heavy | 63 | 536 | 1 | 10.58% [8.08%, 13.25%] |
| Frozen E | normal vs reeds-heavy | 264 | 331 | 5 | 44.42% [40.83%, 48.09%] |
| Frozen E | normal vs movement-heavy | 278 | 318 | 4 | 46.67% [42.91%, 50.58%] |
| Frozen E | movement-heavy vs reeds-heavy | 277 | 318 | 5 | 46.58% [42.92%, 50.33%] |

E materially narrows every pair. Its worst half-tie score is 44.42%, and the
movement/Reeds and Normal/movement intervals include 50%. Normal/Reeds remains
a statistically visible Reeds advantage in this sample, but both strategies
win hundreds of matches. The observed triangle is consistent with comparable,
non-automatic build strength rather than exact pairwise equality.

### Physical and logical seats

“Original” means policy A occupies the physical human seat; “swapped” means
policy A occupies the physical AI seat. The logical policy identities remain
policy A and B in both rows.

| Profile / logical pair | Assignment | Physical policy-A seat | A wins | B wins | Ties | A half-tie score |
| --- | --- | --- | ---: | ---: | ---: | ---: |
| Control normal/Reeds | original | human | 102 | 198 | 0 | 34.00% |
| Control normal/Reeds | swapped | AI | 92 | 206 | 2 | 31.00% |
| E normal/Reeds | original | human | 120 | 177 | 3 | 40.50% |
| E normal/Reeds | swapped | AI | 144 | 154 | 2 | 48.33% |
| Control normal/movement | original | human | 197 | 101 | 2 | 66.00% |
| Control normal/movement | swapped | AI | 188 | 111 | 1 | 62.83% |
| E normal/movement | original | human | 141 | 157 | 2 | 47.33% |
| E normal/movement | swapped | AI | 137 | 161 | 2 | 46.00% |
| Control movement/Reeds | original | human | 33 | 266 | 1 | 11.17% |
| Control movement/Reeds | swapped | AI | 30 | 270 | 0 | 10.00% |
| E movement/Reeds | original | human | 146 | 151 | 3 | 49.17% |
| E movement/Reeds | swapped | AI | 131 | 167 | 2 | 44.00% |

The development E Normal/Reeds seat gap does persist in magnitude but reverses
direction: development Normal scored 46% in the human seat and 35% in the AI
seat; fresh validation scores 40.5% and 48.33%. This instability argues against
attributing the aggregate result to a fixed physical-seat advantage. Fresh E
Normal/movement differs by 1.33 points across seats; movement/Reeds differs by
5.17 points.

### Fresh score gaps and oasis

| Profile | Policy A vs B | Mean A | Mean B | Signed gap | Mean absolute margin | Median |
| --- | --- | ---: | ---: | ---: | ---: | ---: |
| Control | normal vs Reeds | 53.56 | 59.17 | -5.61 | 8.52 | 8 |
| Control | normal vs movement | 47.83 | 44.47 | +3.36 | 6.22 | 6 |
| Control | movement vs Reeds | 49.46 | 59.92 | -10.47 | 10.92 | 10 |
| E | normal vs Reeds | 47.82 | 50.28 | -2.46 | 6.21 | 6 |
| E | normal vs movement | 46.00 | 46.50 | -0.50 | 5.55 | 5 |
| E | movement vs Reeds | 49.03 | 50.76 | -1.73 | 5.14 | 4 |

| Profile / matchup | Policy A safe / worn oasis | Policy B safe / worn oasis |
| --- | ---: | ---: |
| Control normal/Reeds | normal 64/600 / 9/600 | reeds-heavy 0 / 0 |
| Control normal/movement | normal 5/600 / 1/600 | movement-heavy 68/600 / 9/600 |
| Control movement/Reeds | movement-heavy 210/600 / 18/600 | reeds-heavy 0 / 0 |
| E normal/Reeds | normal 46/600 / 11/600 | reeds-heavy 0 / 0 |
| E normal/movement | normal 18/600 / 3/600 | movement-heavy 117/600 / 12/600 |
| E movement/Reeds | movement-heavy 191/600 / 17/600 | reeds-heavy 0 / 0 |

E's movement-focused oasis use remains common, but the balance improvement is
not explained by increasing it: movement/Reeds safe-oasis matches fall from 210
to 191. Reeds-heavy has no oasis arrival in either profile.

### Fresh affordability and complete bundles

Each policy has 5,400 fresh shopping Nights. Values below are opening-budget
Nights / actual purchases.

| Profile / matchup | Policy | Tailwind 6 budget / bought | Reeds 3 budget / bought |
| --- | --- | ---: | ---: |
| Control normal/Reeds | normal | 1058 / 22 | 820 / 681 |
| Control normal/Reeds | reeds-heavy | 426 / 0 | 118 / 118 |
| E normal/Reeds | normal | 2002 / 360 | 226 / 222 |
| E normal/Reeds | reeds-heavy | 1363 / 0 | 3 / 3 |
| Control normal/movement | normal | 919 / 13 | 655 / 554 |
| Control normal/movement | movement-heavy | 967 / 967 | 551 / 0 |
| E normal/movement | normal | 2019 / 348 | 164 / 163 |
| E normal/movement | movement-heavy | 2117 / 2117 | 226 / 0 |
| Control movement/Reeds | movement-heavy | 1058 / 1058 | 740 / 0 |
| Control movement/Reeds | reeds-heavy | 407 / 0 | 122 / 122 |
| E movement/Reeds | movement-heavy | 2134 / 2134 | 340 / 0 |
| E movement/Reeds | reeds-heavy | 1378 / 0 | 6 / 6 |

The complete fresh bundle dictionaries are recorded in
results/holdout-comparison-summary.json under
matchups[].players[].nightBundlesByType. The focused movement/Reeds changes are:

- Control movement-heavy buys 4,476 Tailwinds (954/2464/1058), with 3,438
  Tailwind-only Nights and 1,038 multi-purchase Nights. E buys 5,077
  (934/2009/2134), with 3,719 Tailwind-only, 1,358 multi-purchase, and 102
  three-chip Nights.
- Control Reeds-heavy buys 4,699 Reeds (2898/1679/122), with 3,104 Reeds-only,
  386 Reeds+Seeds, and 1,209 Reeds+Splash Nights, earning 13,355 Reeds Twigs.
  E buys 4,067 (3488/573/6), with 2,905 Reeds-only, 411 Reeds+Seeds, 556
  Reeds+Splash, and 193 Reeds+Wildflowers Nights, earning 8,063 Reeds Twigs.
  E also creates 292 Tailwind-only fallback Nights.

Against Normal, control Reeds-heavy buys 4,661 Reeds and earns 12,922 Reeds
Twigs; E buys 4,086 and earns 7,920. E Normal buys 1,432 Reeds and 1,495
Tailwinds against Reeds-heavy, compared with control Normal's 2,160 Reeds and
584 Tailwinds. Against movement-heavy, E Normal similarly moves from 2,226
Reeds / 545 Tailwinds to 1,539 / 1,462. The full bundle maps show that E changes
Normal's build composition as well as the two targeted policies.

The validation reproduces the development mechanisms at three times the seed
count: Tailwind 6 becomes roughly twice as affordable to movement-heavy, while
Reeds 3 nearly disappears and Reeds-heavy shifts strongly from Reeds 2/3 to
Reeds 1 and non-Reeds fallback purchases.

## Limits and unresolved facts

- The control/C/D/E development comparisons use reused seeds 1000–1099. The
  fresh E section separately uses seeds 10000–10299 and never pools development.
- The three rows within a profile reuse seeds, and each row's two seats are
  related. Reported intervals preserve paired seats but do not turn 200 matches
  into 200 independent samples.
- Fixed movement-heavy and Reeds-heavy purchase policies test strong biases,
  not every credible human bag mixture or opponent combination.
- Global price changes affect Normal's mixed purchases as well as the targeted
  policy. Cross-profile outcomes combine both players' responses.
- Normal matchups remain seat-sensitive under D and especially E; scheduler
  effects need to remain visible in any proposal review.
- D and E nearly remove the top Reeds token from the targeted policy and reduce
  Reeds Twigs substantially. Whether that purchase ladder still offers
  satisfying human choices is unresolved by simulated wins.
- No human-play evidence is included. Automated outcomes do not establish fun,
  clarity, or perceived agency.
- Isolated archives lack Git metadata, so assembly metadata is 1.0.0. Explicit
  source labels, the direct git archive, exact patches, and hashes establish
  provenance.
- Timing overlapped other host work and is not suitable as standalone iPad or
  device-cost evidence.
- Zero Reeds-heavy oasis arrivals do not prove unreachable, and oasis is not a
  required strategy target.

No values beyond control, C, D, and the separately declared final E were run.
The remaining decision belongs to root/user review before any proposed rules
edit or fresh validation.
