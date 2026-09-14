# M3 scattered twigs and centred encounters

14 September 2026. This is a refinement of the approved 43-space Unity visual
proof. The route, all reward payloads, eight havens and underlying board painting
are unchanged from the route-alignment checkpoint.

## Review

- [Empty board — iPad mini](empty-mini.png)
- [Occupied board — iPad mini](occupied-mini.png)
- [Empty board — larger render](empty-large.png)
- [Occupied board — larger render](occupied-large.png)
- [Two-Feather haven inspection](inspection-mini.png)

Twigs are now scattered floor decoration on each biome tile. Chips may cover
some of that decoration; the exact Twig count stays visible at right-middle.
Sleep remains beside the painted moon along the bottom. Larger, brighter haven
Feathers occupy the lower-left pocket, clear of the chip and numbers.

The chip landing is now shared across all tiles, centred horizontally at 48 of
108 board pixels. It no longer shifts left to avoid a twig bundle. The uniform
frame is 64 × 64 with top-left offset (16, -11); the resting duck uses the same
frame. The suggested enlargement was tested, but larger frames obscured haven
Feathers or neighbouring rewards. This is slightly smaller than the previous
72-pixel frame to preserve legibility without moving the approved route.

Oasis Feathers are 56 pixels instead of 64, placed in front of the pool, 11.5
board pixels lower than the previous checkpoint. Its reward backing sits ten
pixels lower, directly underneath them. The space-43 route anchor is unchanged.

## Artwork and authoring

Built-in image generation produced three 1536 × 1024 sheets, with five variants
each. Final sources are in `Assets/Art/DuckLayout`:

- `tile-scattered-wetlands.png`: ordinary 1/2/3 twigs; haven 1/3 twigs.
- `tile-scattered-meadow.png`: ordinary 4/5/6 twigs; haven 4/5 twigs.
- `tile-scattered-wasteland.png`: ordinary 6/7/8 twigs; haven 7/8 twigs.

Wetlands and meadow havens have one Feather; wasteland havens have two. Painted
Twig counts were visually checked on all fifteen variants. The first wasteland
generation needed a targeted count correction and was not imported. Exact
prompts and selected generated-source paths are in [generation.json](generation.json).

These sheets have a baked neutral background, so measured native Sprite Editor
outlines isolate the tile silhouettes, as in the previous tile workflow.
[measure_tile_crops.py](measure_tile_crops.py) reads source pixels and writes only
crop/outline metadata; it does not edit image pixels. Each measured outline
contains over 99% of its tile's coloured/dark pixels. Tile sheets use uncompressed
textures with mipmaps and trilinear filtering to avoid aliasing at tablet scale.

All existing PNG assets are byte-identical to the prior checkpoint. The board
painting remains 1536 × 1024; a larger Game View render is not a higher-detail
painted master. The previously deferred larger master remains deferred.

## Validation

- Unity 6000.6.0f1: compilation completed without errors; Console error query empty.
- Actual 1133 × 744 and 2732 × 2048 Game View targets verified. Both rendered
  scene audits pass; empty and occupied views visually reviewed at both sizes.
- All 43 centre raycasts and PointerClick inspections pass.
- All 16 chip meshes fit at every one of the 42 placement spaces: 672
  combinations. Empty and occupied clearance checks pass against reward text,
  painted moons/Feathers, other chips, the resting duck and oasis Feathers.
- Native-mesh simulation matches 19 active CanvasRenderer meshes. Deliberate
  chip/chip, chip/moon and chip/Feather overlaps fail; restored placement passes.
- All 150 TMP labels use Fredoka SemiBold with black outlines and no overflow.
- All route rows, nest/shelter anchors and rewards match the pre-turn layout.
  Twelve pre-existing document/ProjectSettings drafts remain untouched.

Adjacent JSON files retain tool arguments and results; [validation.json](validation.json)
records source/art hashes and data-preservation checks. Rebuild the scene with
`Quackies/Build Duck Layout Proof`. No Core change, iOS export or device test is
part of this checkpoint. M4 is unstarted; stop here for the user's visual review.
