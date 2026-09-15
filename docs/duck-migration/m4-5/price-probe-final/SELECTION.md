# Primary proposal selection and validation freeze

Recorded on 15 September 2026 after the declared 100-seed development triangles
for control, C, D, and E, and before generating or reading any E fresh-validation
output.

## Sole primary proposal

Select profile E for fresh validation:

- Base source: commit 32b52333d49d99ecbce56dc25b91d490e30a74bc.
- Tailwind prices: 4 / 8 / 12 for Tailwind 2 / 4 / 6.
- Reeds prices: 8 / 14 / 20 for Reeds 1 / 2 / 3.
- Every other source file, rule, policy, and datum remains the base commit.
- Source-only archive:
  profile-e-tailwind-4-8-12-reeds-8-14-20-source.tar.gz,
  SHA-256 6e36a0b766fa1e166ba2cf74716aebeb1d4d1a33ed81f0a0b73d7ff85fd31f30.
- Exact patch: profile-e.patch,
  SHA-256 76662f3dc6b240c6293580e05957d489e9b08ad37048d8a5bdfcc72a2fd68838.

The proposal is frozen. Do not tune prices after viewing fresh results and do
not run another variant.

## Development-only selection basis

E gave the closest worst-pair score in the completed development triangle:

- normal vs Reeds-heavy: Normal 40.50% [34.50%, 46.75%].
- normal vs movement-heavy: Normal 47.75% [41.25%, 54.50%].
- movement-heavy vs Reeds-heavy: movement-heavy 46.75% [40.75%, 52.76%].

Each result is 100 development seeds (1000–1099), both seats, 200 matches, with
a whole-seed paired-seat bootstrap interval. E's worst listed score is closer
to 50% than D's 38.25%, and E's movement/Reeds interval includes 50%. The target
is comparable strength across opponents, not forced exact parity in one pair.

This selects E for validation; it does not approve or promote the prices.

## Fresh-validation protocol

Run the frozen E source on seeds 10000–10299, both seat assignments, CLI
schedule, separately for:

- normal vs Reeds-heavy
- normal vs movement-heavy
- movement-heavy vs Reeds-heavy

That is 300 seed clusters and 600 matches per pair, 1,800 E matches total.
Compare only with root's already completed approved-price holdout controls from
the same 32b5233 source. Do not pool development matches into fresh estimates.
Report whole-seed paired-seat confidence intervals, physical/logical seat split,
final score gaps, safe/worn oasis player-match counts, Reeds 3 and Tailwind 6
affordability/frequency, and complete Night bundle effects.
