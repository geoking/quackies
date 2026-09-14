# M3 validation record

## Recorded evidence

- [Adventure capture](adventure.png) — native 1133 × 744 layout.
- [Occupied capture](occupied.png) — fixed encounter-fit sample.
- [Dream capture](dream.png) — Concept B Dream view with 11 offers.
- [Offer inspection](offer-inspection.png) — observed Tailwind 4 offer at 10 Sleep.
- [Oasis inspection](oasis-inspection.png) — separate Moon/Twig/Feather values, 21 / 9 / 2.
- [Rebuild audit 1](rebuild-audit-1.md) and [rebuild audit 2](rebuild-audit-2.md) both report PASS.
- [Rebuild texture hashes](rebuild-texture-hashes.json) records identical metadata for all 14 textures across the two rebuilds.
- [Runtime check](runtime-check.json) — the actual Game view render target is 1133 × 744; no TMP text overflow.
- [Compilation](compilation.json) — completed successfully with no compiler errors.
- [Console errors](console-errors.json) — final query reports zero errors.

The fixed-data audit passes 50 exact board rows, eight havens, endpoint 50 at
21 Sleep / 9 Twigs / 2 Feathers, 11 shop offers and 16 encounter crops. The
source board remains native 1536 × 1024. Six added layout assets are RGBA icons
or well sprites: `feather.png`, `feathers-two.png`, `sleep.png`, `twig.png`,
`well-grass.png` and `well-wasteland.png`.

## Scope and limits

The scene is reached through the three Quackies layout menu commands and uses
fixed sample data. It has no live gameplay, Core/CLI changes, or rules execution.
The production 3072 × 2048 board master is deferred until geometry review. No
iOS export, physical-device install, or device evidence is claimed.

## Interaction and visual review

Actual pointer checks in Unity passed Dream, Tailwind 4 inspection at 10 Sleep,
Close, View adventure, oasis inspection and Encounter fit. The last modal
typography revision was rebuilt and checked again: each reward numeral has its
own no-wrap field. All 16 encounter faces fit without hiding their reward row;
the source-sheet captions are outside their sprite meshes. Painted havens at
21, 38 and 50 keep the native board art instead of an opaque well overlay.

The native captures were visually reviewed, including the endpoint's pair of
Feathers and rewards below the oasis. Small board rewards have larger tap
inspections. Haven connector lines and the fixed route remain subject to the
user's design feedback; the proof does not establish finger accuracy on hardware.

An earlier capture command used a rejected parent-directory path and produced
a tooling error. It was corrected before the final clean verification interval;
the recorded Console query follows the successful captures and interaction run.
Play mode is stopped. The approved board image and existing playable scenes
are unchanged; the pre-existing ProjectSettings edit remains uncommitted.

**M3 complete; stop for user review. M4 has not started.**
