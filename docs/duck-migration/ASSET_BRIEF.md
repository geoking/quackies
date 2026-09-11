# Duck art brief

## Current experiment: M1 art redirection, 11 September 2026

Generate three original raster concept sheets outside Unity and stop for user
review after inspection. This replaces the rejected M1 visual direction. The
older V1 prompts and manifest remain historical evidence in
[ASSET_MANIFEST.md](ASSET_MANIFEST.md); they are not the current brief.

Use flat goofy tabletop cartoon art with tangible cardboard token/tile presence,
oversized bills and eyes, clear silhouettes and original artwork. Four player
ducks need distinct colours and distinct styling. Encounter tokens should feel
physical, with category-specific silhouettes where useful. The board should be
illustrated, with a continuous readable winding path through three connected
biomes: pleasant pond/grassland, lush comfortable middle, and barren unpleasant
final region. Bridges, irregular spaces and rest places must be legible. Do not
paint a final numeric track mapping into the concept sheet.

| Proposed asset | Purpose and composition | Suggested source / background |
| --- | --- | --- |
| `duck-player-tiles-v2.png` | Four-player sheet: four distinct cardboard duck tiles, varied colour and styling | Landscape sheet; each tile readable in isolation and visibly original |
| `seed-tiles-v2.png` | Seed encounter token design study with physical edge/material and optional category silhouette variants | Square or landscape study sheet; seed identity readable at small size |
| `three-biome-board-v2.png` | Illustrated board study showing one continuous path, three connected biome loops, bridges and irregular rest spaces | Landscape board concept; no generated UI text or exact numbered mapping |

Save the three sheets under `docs/duck-migration/concepts/2026-09-11/` with the
filenames above. These are concept references, not Unity-ready imports.

No generated UI text, exact numbered track, token values, buttons or logos. The
concept board may show visual rest places, but it must not imply approved reward
numbers, timing, AI or balance. Existing 54 logical positions and
next-scoring-space semantics remain the baseline until a later rules approval.

The sheets demonstrate visual language only; they do not implement a playable
game. Leave out runtime highlights, resource labels and controls. The board's
painted route and resting places are the focus of this image review.

## Later implementation boundary

If approved later, use illustrated board artwork with invisible indexed anchors
and hitboxes layered over it. Do not redraw the board as a rigid programmatic
grid and do not infer rules from painted spaces. No Unity import or component
boundary change is part of this M1 revision.

## Repository placement and provenance

Use the built-in `image_gen.imagegen` tool. Keep native concept outputs and
their exact generation/refinement prompts under `concepts/2026-09-11/`, outside
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

- Inspect all three concept sheets at useful size and pause for the user's
  reaction. No Unity import, editor call, source/settings change or M2 starts.
- Treat the previous V1 Unity screenshot and compile checks as superseded style
  evidence: technically valid, visually rejected.
