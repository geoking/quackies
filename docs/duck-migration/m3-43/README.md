# M3: 43-space board review

The user's selected route is **43 spaces: 14 wetlands / 14 meadow / 15
wasteland**. The revised Unity layout is implemented and checked; visual
approval remains with the user. M4 is unstarted and awaits their command.

The selected 1536 × 1024 painting is unchanged. The detailed 3072 × 2048
painted master remains explicitly deferred. Larger Game-view captures show
the current painting with resolution-independent UI; they are not a newly
painted high-resolution master.

## Layout

- Recentered the wetlands loop and aligned the seven haven wells beside their
  shelter entrances. The 20 Sleep / 8 Twigs wasteland haven is raised toward
  its refuge. Bridge decks remain clear between spaces 14/15 and 28/29.
- Eight havens: **3, 10, 16, 21, 26, 32, 36, 43**. The native oasis has two
  large sideways Feathers on the ground, with **21 Sleep / 9 Twigs** beneath.
- Haven wells retain the biome colour and a leafy rim. Two-Feather rewards use
  two separated images. Ordinary coloured wells are inset within an 86 × 63
  design-unit footprint, with 70-unit encounter tokens overhanging them.
  This improves token size relative to the well; it does not claim every
  token is absolutely larger than the previous, roomier 40-space candidate.
- Reward rows are 24 design units high, with larger live TMP numbers and the
  existing Moon and Twig artwork. Exact values stay independent of painting.
- The frozen 43 reward rows are synchronized into the canonical v1 board
  during this closeout. Shop prices and encounter/event rules remain unchanged.

## Review images and checks

- [Empty board, iPad mini review size](empty-mini.png)
- [Encounter fit, iPad mini review size](occupied-mini.png)
- [Empty board, larger review size](empty-large.png)
- [Encounter fit, larger review size](occupied-large.png)
- [Validation summary](layout-validation.json) and [route coordinates](route-guide.json)

Both actual Game-view sizes, **1133 × 744** and **2732 × 2048**, pass the
rendered geometry/TMP audit. All **43/43** center raycasts select the expected
space and programmatic PointerClick events open its matching inspection.
There are zero compilation and Console errors. A final rebuild passes, with
all 20 texture import metadata hashes unchanged. Empty and occupied captures
were visually reviewed for path alignment, shelter access, bridge gaps,
reward clarity, Feather separation and token fit.

These are Editor layout checks, not Core gameplay, physical-device testing,
an iOS export, or user acceptance. Future movement must follow explicit bridge
waypoints; straight interpolation between tile centers would cut corners.
The shorter route also requires a fresh start-bound audit; optional starting
Feathers need their endpoint boundary resolved before M4 implementation.

Historical 40- and 50-space evidence remains historical. This revision does
not authorize M4 or imply approval of the final visuals.
