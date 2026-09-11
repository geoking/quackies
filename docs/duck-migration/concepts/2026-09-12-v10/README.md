# V10 — wider token routes and a dramatic wasteland crossing

[Review the revised base board](board-art-v10.png).

The meadow's interior planting is more compact, leaving wider open ground on
both sides. The wetland's returning bank and the desert's shelter edges were
also cleared so the same token design can be used throughout. The desert route
ends in an open bay below the oasis rather than squeezing behind it.

The wasteland crossing is now a timber-and-rope bridge over a deeper rocky
cleft. Its supported ends meet the meadow's upward approach and the sandy
downward route into the desert. The first bridge retains its simpler grounded
construction.

## Clearance review

The existing visual reference uses a 76 × 72 ordinary body and a 96 × 96 rest
frame. The initial 120px route-width target did not survive generation at all
pinch points, so a second edit compacted scenery further and requested
140–150px corridors. A final local edit compacted the lower desert grotto to
open the required main route on its right. Prompt dimensions are design
targets, not measurements.

The bounded manual audit passes its sampled cross-sections against the 96px
reference: meadow estimates are about 110–140px, the corrected grotto-right
lane is about 110–130px, and the smallest sampled estimate is 100px on the
wetland's returning bank. These visual estimates have roughly ±10px
uncertainty; they are not exact clearance guarantees for every future position.

The [corridor audit](corridor-audit.json) records conservative visual transects,
the rejected first candidate, the selected image and any remaining limitations.
The design check preserves eight shelters, the three meadow entrance directions
and the shortcut-blocking vegetation. Exact placement of all 53 spaces is a
separate unresolved layout task; this base image contains no tiles or labels.

## Files and scope

Built-in image generation edited the [V9 base](../2026-09-11-v9/board-art-v9.png).
The [initial prompt](prompt.json) and
[general clearance correction](clearance-correction-prompt.json) and
[local grotto correction](grotto-clearance-prompt.json) are saved with
[inspection and provenance](inspection.json). The selected native PNG is copied
without conversion.

Native resolution remains **1536 × 1024**. The earlier 3072 × 2048 master is
still outstanding; the unanswered API-workflow approval was not treated as
authorization. No API fallback, resizing, Unity import or rules change ran.
The width and bridge artwork is ready for user review; no Unity work follows
automatically.
