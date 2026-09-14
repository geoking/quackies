# Provisional 40-space layout review

14 September 2026. This validates a reviewable visual fixture, **not an approved
40-space rules change or completed M3**. The route-count question remains open;
canonical v1 data still has 50 spaces. M4 has not started.

## Review images

- [Empty board at 1133 × 744](empty-mini.png)
- [Encounter samples at 1133 × 744](occupied-mini.png)
- [Empty board at 2732 × 2048](empty-large.png)
- [Encounter samples at 2732 × 2048](occupied-large.png)
- [Spring-fed refuge inspection](inspection-mini.png)
- Historical comparison: [previous 50-space board, 90 × 66 wells](../m3-revision/adventure-mini.png).

The current wells use 108 × 79.2 design-unit rectangles, a 20% increase in both
dimensions. Ordinary artwork preserves its source aspect ratio and draws within
that rectangle, just as before; the leafy haven frame fills it. Reward rows keep
their existing height. Font size is limited by that height to prevent clipping.
The same ordinary well sprite and biome tint appear inside each haven frame.
One or two large white Feathers occupy the frame's right side. Mipmapped,
trilinear imports keep the dense leaves from sparkling when reduced to mini size.

Both viewport dimensions were read from the actual Game view render texture,
not inferred from the requested screenshot dimensions. The board painting is
still the selected **1536 × 1024** source. A large screenshot is not a larger
painted master; the exact 3072 × 2048 master remains deferred.

## Route and shelter review

The candidate uses 13 wetlands, 13 meadow and 14 wasteland spaces. It retains
eight havens at 3, 10, 15, 20, 24, 29, 33 and 40. The last space binds to the
painted oasis rather than covering it with another well. All route/reward
remappings in [candidate-40.json](candidate-40.json) are visual proposals only.

The lead inspected all 40 centers against the painting in numbered and clean
runtime captures. An independent visual review agreed that the three requested
entry corrections align: 6 Sleep / 1 Twig beside the reeds opening (3),
15 / 5 below the orchard shelter (20), and 20 / 8 below the spring refuge (33).
The log artwork beside 29 is a shelter, not a third bridge. The wasteland left
column is on the sand corridor rather than the cliff, and 39 sits below the
oasis on its approach.

The two bridge decks contain no tile wells. There are intentional tile-free
gaps between 13/14 and 26/27. A reviewer flagged that straight lines between
those centers would cross below the decks. The intended traversal follows the
visible approach curves and deck instead; future duck movement must include
bridge waypoints and must not interpolate directly across the river or ravine.
This static fixture does not yet animate travel. These transitions remain a
specific point for user visual review.

[The route guide](route-guide-candidate.json) records measured candidate
positions. Agreement with that guide is not independent proof of alignment with
painted pixels. The current 40-space fit is feasible at full size; constrained
packing trials on this guide are not proof that every possible 45-space design
would fail. Keeping 45 may require less enlargement or changes to the painting.

## Verification

| Check | Result |
| --- | --- |
| Unity compilation | Up to date; `failed:false`, no errors |
| Final Console error query | 0 errors |
| Runtime layout audit at 1133 × 744 | PASS |
| Runtime layout audit at 2732 × 2048 | PASS |
| Center raycasts and PointerClick inspection events | 40/40 correct spaces and titles |
| Text, reward and encounter-clearance audit | PASS at both viewports |
| Serialized presentation samples | All 16 encounter designs, 11 offer fixtures retained |
| Rebuild consistency | Same hierarchy digest; texture metadata unchanged |

Evidence: [mini audit](audit-mini.json), [large audit](audit-large.json),
[mini viewport](viewport-mini.json), [large viewport](viewport-large.json),
[pointer events](center-pointer-events.json), [final rebuild](rebuild-final.json),
[texture imports](import-stability.json), [compilation](compilation.json),
[Console](console-errors.json). Pointer checks exercised Unity's raycaster and
PointerClick handlers programmatically; they are not physical iPad touch tests.

Hierarchy digest:
`243dc117f52a11599da93370522809df9b74824064ad04de58d9ae639e0a6042`.
An earlier CS1628 compiler error and initial reward-label overflows were fixed
before these final checks. Temporary diagnostic tile numbers were only added in
Play mode and are absent from the saved scene and review images.

Editor is left stopped at the mini viewport. The original Core/CLI, canonical
v1 data, selected board painting and unrelated ProjectSettings edit are preserved.
No iOS export or device test is claimed. The pending route-count choice, gameplay,
balance validation after any count change, and deferred production art remain
outside this candidate's passed layout checks.
