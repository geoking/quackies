# Duck art brief

## Current experiment: M1 V4 board refinement, 11 September 2026

Keep the approved V2 duck tiles, V3 seed tile and three-biome art style. Refine
only the board design outside Unity, then stop for image review. The
older V1 prompts and manifest remain historical evidence in
[ASSET_MANIFEST.md](ASSET_MANIFEST.md); they are not the current brief.

Use flat goofy tabletop cartoon art with tangible cardboard token/tile presence,
oversized bills and eyes, clear silhouettes and original artwork. Four player
ducks need distinct colours and distinct styling. Encounter tokens should feel
physical, with category-specific silhouettes where useful. The board should be
illustrated, with a continuous readable winding path through three connected
biomes: pleasant pond/grassland, lush comfortable middle, and barren unpleasant
final region. Bridges, token wells, reward values and individual rest places
must be legible. The latest request removes visible position numbers and uses
coin/twig icons with exact reward amounts in the bottom row of each space.

| Proposed asset | Purpose and composition | Suggested source / background |
| --- | --- | --- |
| Existing `duck-player-tiles-v2.png` | Approved four-player identity sheet; retain unchanged | No new generation needed |
| Existing `seed-tile-v3.png` | Approved orange rounded triangular seed tile; retain unchanged | No new generation needed |
| `three-biome-board-v4.jpg` | Incomplete starting nest, unnumbered token wells, coin/twig icon rewards and eight clearly linked shelters | Generated landscape with precise static typesetting; consistent geometry at 11, 44 and 53 |

Save the new sheets and prompts under
`docs/duck-migration/concepts/2026-09-11-v4/`. Approved token images remain
in their original folders. These are design review images, not Unity imports.

Use the exact current [track data](concepts/2026-09-11-v3/track-data.md): separate
duck start 0, spaces 1–53, last encounter 52, final scoring 53. Retain indices only
in layout data. Preserve all Pond penny/Twig pairs, including repeats and zeros.
Print coin icon + amount and twig icon + amount; zero Twigs are omitted, with
the coin pair centered. The nest replaces the dock and start overlay without text.
Eight rests were confirmed by the user; illustrate them at 5,13,20,28,34,40,46,52 with a leafy
frame/feather medallion attached to that exact pad. Do not invent a biome reward
curve. Every rest needs an adjacent recognizable shelter and a short entry spur
to its exact pad. Give 40 a modest log/rock shade shelter while retaining the
more lush cave at 46; align the upper oasis with rest 52, not final space 53.
Runtime rules remain unchanged during this image review.

The sheets demonstrate visual language only; they do not implement a playable
game. Leave out buttons and runtime controls. Use a small printed key to explain
the placement well, Pond pennies, Twigs and rest marker, plus the existing rule
to score the next empty space. Check visible labels and counts against the
source table; a correct prompt alone does not establish a correct image.

The user approved a combined workflow after the first labelled generation was
inaccurate: imagegen supplies clean illustrated scenery, and precise typesetting
places all 53 unnumbered wells, reward pairs and eight rest markers. Preserve a
reproducible static renderer and inspect its exported image. Do not regenerate
the approved duck sheet. No Unity call is part of this process.

## Later implementation boundary

If approved later, use illustrated board artwork with invisible indexed anchors
and hitboxes layered over it. Do not redraw the board as a rigid programmatic
grid and do not infer rules from painted spaces. No Unity import or component
boundary change is part of this M1 revision.

## Repository placement and provenance

Use the built-in `image_gen.imagegen` tool. Keep native concept outputs and
their exact generation/refinement prompts under `concepts/2026-09-11-v4/`, outside
Unity. The attached duck is style inspiration, not artwork to trace. Inspect
the actual outputs before declaring the concepts ready for user review.

Possible future Unity import roots, relative to `unity/Quackies.Unity`, after
a separate request to implement the reviewed direction:

- `Assets/Art/DuckTheme/Backgrounds/`
- `Assets/Art/DuckTheme/Characters/`
- `Assets/Art/DuckTheme/Encounters/`

For later production assets, record prompts, version, output path, actual
dimensions and alpha checks in the asset manifest. Keep `.meta` files stable
on later replacements. Preserve `Assets/Art/raw` and all current
catalog references. Original game art is reference for rule data only; do not
feed it into the generator to imitate its illustration or board composition.

The existing M1 scene and builder remain historical V1 evidence. Do not modify
them during this image-only gate.

## Later assets, after the style review

Essential M3/M4 work: obstacle, tailwind, signpost, splash, nesting reeds,
companion duck and wildflower icons; feather, twig and Pond penny icons; base
scored nest; shelter marker; one reusable lily-pad crossing and landing marker;
simple happy/worn-out duck result poses; water flask and Lucky find treatment;
original Pond happenings presentation. Numeric strengths remain UI overlays.

Keep the scored nest distinct from the daily shelter/resting spot. Nest growth
stages, elaborate animations and additional decorative variants are later polish.
Shelter art does not approve or implement a new shelter reward rule.

## M1 revision gate

- Inspect the refined board at useful size, then pause for the user's
  reaction. No Unity import, editor call, source/settings change or M2 starts.
- Treat the previous V1 Unity screenshot and compile checks as superseded style
  evidence: technically valid, visually rejected.
