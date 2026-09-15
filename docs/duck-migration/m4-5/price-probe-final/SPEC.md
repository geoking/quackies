# M4.5 final-AI bounded price diagnostic specification

Declared before source builds or evaluation runs on 15 September 2026.

Status: exploratory development evidence only. This is not a holdout run, final
recommendation, or approved rules change.

## Source

Use committed Quackies.Core and Quackies.Evaluation source from
32b5233, resolved to its full commit hash before execution. Every profile must
come from an independent extraction of the same git archive.

## Profiles

- Control: approved prices unchanged: Tailwind 5/10/15 and Reeds 6/11/16.
- C: Tailwind 4/8/12 and Reeds 7/12/17.
- D: Reeds 8/14/20; Tailwind remains 5/10/15.

Only the named Offer numeric prices in DuckRules.cs may differ. All other source,
rules, policies, schedule behavior, and data remain the 32b5233 version. Do not
explore additional values.

## Match set

Use seeds 1000 through 1099 inclusive, both seat assignments, and the CLI
schedule. Run the full triangle separately for each profile:

- normal vs reeds-heavy
- normal vs movement-heavy
- movement-heavy vs reeds-heavy

This produces 200 matches per profile/matchup, 600 per profile, and 1,800 total.
Do not use seeds 10000 or above. Do not pool earlier-AI controls with these
results.

Use only the real Quackies.Core session and actions issued through
Quackies.Evaluation. Generate no saves and use no save data as policy input.

## Measures and interpretation

Record wins, ties, half-tie policy score with 95% intervals that resample whole
seeds and preserve paired seats, final score gaps, safe and worn oasis arrivals
per player-match, complete Night bundle counts, and Reeds 3 / Tailwind 6
purchase frequency and affordability effects.

The user target is comparable strategic viability for movement-focused and
Reeds-focused bags across opponents and bag combinations. It does not require
exactly 50/50 in one targeted matchup. Development results rank bounded
hypotheses only; root will decide whether to propose a profile and run fresh
validation separately.

Timing may include host load from root's concurrent approved-profile work and
must not be presented as standalone device-cost evidence.

## Final E addendum

Declared after the control/C/D runs and before any E source extraction, build,
or evaluation run on 15 September 2026.

Root authorized one final bounded development hypothesis:

- E: Tailwind 4/8/12 and Reeds 8/14/20.

E combines C's independently motivated Tailwind prices with D's independently
motivated Reeds prices. It is not tuned from holdout data. Every other source,
rule, policy, and datum remains commit
32b52333d49d99ecbce56dc25b91d490e30a74bc unchanged.

Run E on the same declared development design only: seeds 1000–1099, both seat
assignments, CLI schedule, and the three matchups normal vs reeds-heavy, normal
vs movement-heavy, and movement-heavy vs reeds-heavy. This adds 600 matches.
Keep E separate from control/C/D by source label and raw artifact. Do not use
seeds 10000 or above and do not run any further profile after E.

The interpretation remains comparable build strength across opponents, without
forcing exactly 50/50 in one pair. E remains diagnostic; if it is promising,
root will fresh-validate D and E before an exact proposal. No promotion is
authorized by this addendum.

## Post-development selection and fresh-validation addendum

Declared after E's development triangle and before generating or reading any E
fresh-validation output on 15 September 2026. SELECTION.md is the authoritative
pre-output selection record.

Root selected E as the sole primary proposal because it had the closest
development worst-pair score and its movement/Reeds interval included parity.
Freeze E at Tailwind 4/8/12 and Reeds 8/14/20 on source
32b52333d49d99ecbce56dc25b91d490e30a74bc. Do not tune further or run another
variant.

Fresh-validate E only on seeds 10000–10299, both seats, CLI schedule, for all
three declared pairs. Keep the 1,800 E fresh matches separate from all
development data. Compare them with root's already completed approved-price
holdout controls from the same source. Record seed-cluster confidence intervals,
physical/logical seats, score gaps, oasis counts, affordability, and full Night
bundles. Stop after packaging.
