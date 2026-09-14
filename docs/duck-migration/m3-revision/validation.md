# Revised M3 layout validation

14 September 2026. **Implementation and layout checks pass; ready for user
visual review.** This is a fixed-data scene, not a playable duck rules build.
M4 remains unstarted and waits for the user's command.

## Review images

- [Adventure, 1133 × 744](adventure-mini.png): empty spaces and all eight havens.
- [Occupied, 1133 × 744](occupied-mini.png): all 16 encounter variants plus
  duck, zzz and permanent-trail presentation.
- [Occupied, 2732 × 2048](occupied-large.png): larger viewport, including samples
  on every nonendpoint haven. The layout fits with letterboxing, without stretching.
- [Haven inspection, 1133 × 744](haven-inspection-mini.png): pointer-opened
  Reed hammock showing 6 Sleep, 1 Twig and 1 Feather.

These are native Game view captures. The live render targets were queried
separately: [mini](runtime-mini.json), [large](runtime-large.json). Changing the
capture output size alone was not treated as a viewport check. The board source
is still the approved **1536 × 1024** image; this larger render does not create
new painted detail. The user explicitly deferred the detailed 3072 × 2048 master.
All 17 texture imports use a 4096 cap, and the UI retains a fitted logical canvas.

## Geometry and visual review

The [measured centerline](route-guide.json) and [independent geometry audit](route-audit.json)
record 50 ordered spaces and 49 wells. Run `python3 docs/duck-migration/m3-revision/route-audit.py`
from the repository to reproduce the numeric checks. Each biome has six spaces
on its descending arm and six on its ascending arm, with the remainder at bends,
crossings and the endpoint. There is no staggered double row.

Arclength intervals average 96.56 design pixels, ranging from 88.50 to 124.60;
standard deviation is 9.08. Bends need extra clearance for horizontal reward
strips. Full well/reward rectangles, including crossed pairs, do not overlap.
Visual inspection of both captures confirms the centers follow the painted
route and the shelter artwork remains visible. Numerical projection onto an
authored guide alone is not evidence of alignment with the illustration.

Wells increased from 74 × 58 to **90 × 66** design pixels. Ordinary token faces
increased from 54 to **66**; haven faces use 58.5 with an inset that leaves the
Feather seal visible. Reward strips are 20 design pixels high with a one-logical-pixel
gap. Occupied captures and the live scene audit show visible rewards and no
token/seal overlap. Detailed reward inspection remains available at small sizes.

| Space | Shelter | Treatment |
| ---: | --- | --- |
| 3 | Reed hammock | Path-side well with integrated one-Feather seal |
| 11 | Willow nest | Path-side well with integrated one-Feather seal |
| 19 | Clover hollow | Path-side well with integrated one-Feather seal |
| 27 | Orchard shelter | Path-side well with integrated one-Feather seal |
| 29 | Hayloft hideaway | Path-side well with integrated one-Feather seal |
| 37 | Shaded rock nook | Path-side well with integrated two-Feather seal |
| 44 | Spring-fed refuge | Path-side well with integrated two-Feather seal |
| 50 | Oasis sanctuary | Painted oasis; small two-Feather medallion beside rewards |

The seven well tiles sit beside shelters rather than replacing them. The final
medallion leaves the oasis pool visible. Its rewards remain **21 Sleep / 9 Twigs /
2 Feathers**. Haven links and floating haven Feather badges are absent. The
occupied sample's separate permanent Feather trail is intentionally still shown.
New artwork provenance and native alpha checks are in [art/inspection.json](art/inspection.json).

The [canonical board table](../v1/BOARD_AND_SHOP.md), JSON and CSV match the
layout data. Seven nonendpoint havens retain neighboring Twig plateaus and
local Sleep peaks. Shop prices are unchanged. The refreshed [bounded math
audit](../v1/balance-audit.json) passes, but the earlier haven affects modeled
opening affordability; complete-match balance remains future work.

## Editor and interaction evidence

- [Compilation](compilation.json) completed without errors. The final
  [Console query](console-errors.json) returned zero errors.
- [Rebuild 1](rebuild-1.json) and [rebuild 2](rebuild-2.json) both pass with
  hierarchy digest `2fcb270bf84f4061c6c1535e980c9f184600a159baacfdb099d59124fe099606`.
  [All 17 texture metadata hashes](rebuild-texture-hashes.json) are unchanged.
- Runtime checks at both sizes pass 50 unique spaces, eight havens, 49 wells,
  11 offers, 16 distinct encounter sprites, reward bounds and text overflow.
  An additional [all-havens occupied audit](runtime-all-havens.json) passes.
- Actual computer-use pointer checks passed Encounter fit, Reed hammock
  inspection and Close at mini size; oasis inspection (21/9/2), Close, Dream
  and View adventure at the larger size. Navigation returned to the same board.
- The Editor was stopped and its Game view returned to 1133 × 744 after review.
  Rebuild-only scene serialization and temporary TMP cache changes were discarded;
  the tested scene checkpoint has the same hierarchy digest. The pre-existing
  ProjectSettings modification was preserved and excluded from commits.

No Core/CLI rules, original playable scenes, original Core DLL, iOS export or
device installation changed. The higher-resolution board master remains deferred.
Dream Concept-B likeness and playful typography remain M5 follow-ups. User
approval of this revised concept is not assumed.
