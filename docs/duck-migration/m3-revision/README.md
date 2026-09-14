# M3 board and presentation revision

Status: **open; no completion claim yet.** M2 remains approved. M4 Core/CLI
work waits for the user's command and for this revision to pass review.

The previous [M3 layout proof](../m3/README.md) is retained as historical
validation: it records fixed-data checks, rebuilds, pointer interactions and
Console results. The user rejected its board concept and its assumption that
the native board and 1133 × 744 review size could define production assets.
Those records must not be reused as evidence that this revision is complete.

## Required result

Finish the current layout at the approved board source size, while keeping the
authoring plan ready for a later higher-resolution master. The user explicitly
defers the detailed 3072 × 2048 production master. Keep the UI
resolution-independent, retain the higher-resolution authoring plan with a
4096 import cap, and provide native production sprites at their intended scale.
The 1133 × 744 iPad mini landscape viewport is a preview and check, not the
source-asset limit. Review the result at 1133 × 744 and at a larger iPad or Mac
viewport.

All 50 spaces must be one centered route following the painted path, with
near-even arclength spacing through wetlands, meadow and wasteland. Do not use a
zigzag route or sidebar/stacked wells. The descending and ascending wetland arms
should contain approximately even numbers of spaces. Use larger tiles and token
faces while preserving clear movement paths and usable shelter entries.

Seven haven tiles sit **on the path next to their painted shelters** and never
cover shelter artwork. Space 50 uses the oasis itself and receives no separate
tile. Integrate the single-Feather haven artwork and the two-Feather wasteland
artwork into the haven treatment; remove floating Feather decorations.

Keep the final oasis at **21 Sleep / 9 Twigs / 2 Feathers** unless root changes
the approved data. Geometry may require a Sleep/Twig/haven row reassignment,
but only root decides and persists those data changes; this document supplies
no replacement numbers.

Dream likeness to accepted [Concept B](../concepts/2026-09-13-dream-study/concept-b-nest-mat.png)
and fun, engaging typography are minor implementation follow-ups for M5 when
the Dream view is implemented. They are recorded here for continuity, but are
not current M3 revision completion gates.

## Evidence checklist

M3 revision remains open until each item has evidence linked here or recorded
in the companion validation record:

- [ ] UI anchors/layout are resolution-independent and production sprites are
      native assets rather than 1133 × 744-limited exports; preserve the
      higher-resolution authoring plan and 4096 import cap.
- [ ] One centered route contains all 50 stable-ID spaces with near-even
      arclength spacing across all three biomes.
- [ ] Wetland descending and ascending arms have approximately even space
      counts, with no zigzag or sidebar/stacked layout.
- [ ] Tiles and token faces are larger and remain usable without covering the
      painted path or shelter entries.
- [ ] Seven haven tiles are on path beside painted shelters and cover none of
      the shelter artwork; space 50 is the oasis with no separate tile.
- [ ] Single-Feather haven and two-Feather wasteland treatments are integrated
      into haven art; no floating Feather decorations remain.
- [ ] Final oasis still reads 21 Sleep / 9 Twigs / 2 Feathers, or root's
      persisted data decision is linked if it changed.
- [ ] Representative occupied spaces, haven adjacency, reward visibility and
      token fit are checked at 1133 × 744.
- [ ] The same geometry and readability are checked at a larger iPad/Mac
      viewport.
- [ ] Rebuilds reproduce stable IDs and do not duplicate or omit spaces,
      haven links, offers or encounter variants.
- [ ] Console/layout checks are recorded for the revised scene. This is visual
      evidence only; it does not claim Core gameplay, iOS export or device test.

The deferred 3072 × 2048 master and Dream Concept-B likeness/typography remain
future production and M5 implementation follow-ups. The old M3 captures may be
cited for historical comparison, but passing this current layout checklist is
the basis for closing M3 revision. After review, root may record the final data
decisions and authorize the next milestone.
