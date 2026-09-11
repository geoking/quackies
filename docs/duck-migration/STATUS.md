# Duck migration status

**M1 V7 — base-board revision generated and visually inspected; ready for user
review.** The user likes V5’s painted style but rejects its spaces’ alignment
with the painted paths.
V7 distinguishes wetlands from middle grasslands: cooler teal/sage damp banks
and reeds versus warm sunny open grasslands, broad short-grass route, taller soft
grasses, meadow flowers and leafy deciduous cover around the three existing
shelters. Preserve the desert composition, far-upper-right oasis, nest, eight
shelters, bridges, left access to the upper meadow shelter and broad corridors.
This image contains no tiles, tokens, numbers, reward text or legend. Branch:
`codex/duck-game-milestone-0`.

## Completed work

- V1 generated assets and the separate `DuckStyleTestScene.unity` are retained
  as historical evidence in `ff2c41b`, `42e51b3` and
  [the M1 evidence](evidence/m1/README.md). Technical checks passed, but the
  style was rejected.
- Three selected concept sheets are generated, inspected and saved under
  [concepts/2026-09-11](concepts/2026-09-11/README.md): four distinct player duck
  tiles, a seed encounter tile study, and an illustrated three-biome board.
  Exact prompts, native output dimensions and hashes accompany the images.
  The plan revision is published in `d0ac497`.
- V3 preserves the existing Penny/Twig table. The proposed biome reward curve
  is deferred; the latest direction is eight illustrated rests, rounded up from
  15 ruby spaces, with unchanged printed Penny/Twig values. Source rules are
  unchanged. [Track audit](concepts/2026-09-11-v3/track-data.md) records the data.
- The user approved precise typesetting over generated scenery after the first
  labelled board image skipped/mislabelled spaces and rewards. That image is
  excluded as a final deliverable. The finished static renderer uses the
  verified data: all 53 numbered spaces and reward pairs, eight rest markers,
  bounds and non-overlap checks passed. The final 3072 × 2048 JPEG was visually
  inspected for readable labels and visible rest medallions.
- [V3 images and review notes](concepts/2026-09-11-v3/README.md) are ready.
  Data/brief checkpoint: `63b4ceb`; seed and clean background: `8990e3e`;
  final board, renderer and QA: `47434dc`.
- [V4 board and review notes](concepts/2026-09-11-v4/README.md) contain the
  incomplete nest, unnumbered wells, coin/twig reward rows and eight distinct
  shelters linked by duck-footprint trails. Brief/data: `d5aebab`; native
  generated artwork: `d4523f5`; final renderer, JPEG and QA: `a66c8d3`.
  All 53 reward rows match the existing table, zero Twigs are omitted on the
  first five rows, and geometry/bounds checks pass. Lead visual inspection of
  the final 3072 × 2048 JPEG passed; its overlay style was subsequently superseded.
- [V5 finished labelled board](concepts/2026-09-11-v5/README.md) combines the
  unchanged selected background with painted bubbly stones, full leafy white
  feather rest frames and the larger illustrated legend. Brief/references:
  `f05432f`; painted components: `cd32631`; final image/renderer/QA: `db76c1c`.
  All 53 reward rows match the current table. Measured numeral bounds fit inside
  the painted capsules; zero-Twig rows and eight rests are correct. Full image
  and label close-ups passed lead visual inspection. The user likes its painted
  appearance but rejected the alignment of its spaces with the illustrated
  paths.

V5 remains historical evidence and is superseded as the current visual review;
its static QA does not prove path alignment. V6 is saved under
`concepts/2026-09-11-v6/` and was visually inspected as a review candidate. It
has no overlay, so 53-space fit, reward placement and exact indexed alignment
remain unresolved. Keep the full rules baseline and do not infer a Unity
position map from this image.

V6 is historical review evidence at checkpoint `94fd3bb`; its visual inspection
does not establish overlay alignment. V7 is saved under
`concepts/2026-09-11-v7/` and was visually inspected as a review candidate. It
has no overlay, so 53-space fit, reward placement and exact indexed alignment
remain unresolved.

This pass generated concept images only, outside Unity, with no Editor calls.
Source, Unity assets, packages and shipping scene list remain unchanged. Preserve the pre-existing
ProjectSettings preload removal and keep it out of commits.

## Review gate

V7 is ready for the user’s reaction. Do not import images into Unity, alter
source/settings, or start M2. Unity positioning was discussed only, not
authorized. The existing 54 logical positions and next-scoring-space semantics
remain the baseline; this base image does not establish Unity behavior.

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
