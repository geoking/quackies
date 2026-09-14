# M3: painted rewards inside larger tiles

14 September 2026. The rewards-inside concept is integrated into the saved
`Assets/Scenes/DuckLayoutProof.unity` scene. This is a fixed-data visual proof
for review, not a playable implementation of the new duck rules. M4 is unstarted.

## Review

- [Occupied board — iPad mini review size](occupied-mini.png)
- [Empty board — iPad mini review size](empty-mini.png)
- [Occupied board — larger review size](occupied-large.png)
- [Empty board — larger review size](empty-large.png)
- [Haven inspection](inspection-mini.png)

Use **Quackies → Build and Play Duck Layout Proof** to recreate and inspect it.
The scene is saved with Play mode stopped. Empty wells and Encounter fit switch
between the two visual fixtures; tapping a space opens its exact reward preview.

## Implemented direction

- Fifteen painted sprite variants supply all 42 nonendpoint tiles. Twigs, moons,
  stars and haven Feathers are in the artwork; only exact reward numerals are
  live TMP text. The selected native oasis remains uncovered, with its established
  two ground Feathers and separate 21 Sleep / 9 Twigs treatment.
- Wetland tiles have continuous green grass, reeds and flowers with no added
  water. Meadow tiles use warmer grass and clover. Ordinary wasteland tiles
  retain dry soil and sparse decoration; havens add lush green rims.
- Haven artwork is more decorative and has one or two larger white Feathers
  projecting over the tile rim. Illustrated twig counts match their values.
- Tiles are 108 × 84 board units. Encounter tokens are 78 units, up from 70
  in the previous checkpoint (11.43%). The resting duck uses the same footprint.
  Tokens start 12 units left and 19.15 units above the tile's upper-left corner,
  keeping the painted rewards and live numbers clear.
- Centers follow the available painted route. The lower wetlands loop and meadow
  bend were spread out to prevent overlap. All eight haven centers are unchanged
  beside their shelter access paths; both bridge decks remain clear.
- The existing 43 reward rows, 14/14/15 biome split and haven IDs
  3/10/16/21/26/32/36/43 are unchanged. This checkpoint changes presentation.

The approved 1536 × 1024 board painting is unchanged. The larger 3072 × 2048
painted master remains deferred. Larger screenshots render the existing painting
with live UI; they do not constitute a larger source painting.

## Evidence and limits

Both actual Game-view render targets, **1133 × 744** and **2732 × 2048**, passed
the rendered geometry audit. It checks tile/tile and token/token separation,
token clearance from every reward number and painted reward region, number
containment and exact values, the resting duck's shared footprint, oasis Feather
clearance, serialized references and TMP overflow. A focused regression exercise
deliberately overlapped two spaces and shrank the duck; both were rejected, and
the restored scene passed.

All **43/43** center raycasts selected the correct space and programmatic
PointerClick events opened its matching inspection. Rebuilding preserved the
hierarchy and all texture import metadata. Final compilation and Console checks
report zero errors. [Validation](validation.json) records source hashes;
[route centers](route-centers.json) and [rendered bounds](rendered-rects.json)
record the geometry. Raw checks are saved alongside these files.

The lead and a read-only visual reviewer inspected path fit, shelter access,
decorations, counts and readability. The wetlands lower loop is visually dense
at mini size; final visual approval belongs to the user. These are Editor and
static-image checks, not physical-device testing, an iOS export or gameplay tests.

## Artwork provenance

Built-in image generation produced the three 1536 × 1024 atlases from concept
`exec-c4923b21-a3b9-4ed5-ac09-f64601ff5353`. Their
[prompts](art-prompts.json) and [source hashes](art-provenance.json) are retained.
The PNGs are RGB, not transparent exports: tight Sprite Editor mesh outlines
clip their exterior checkerboard without modifying the original bitmap bytes.
Tile imports use mipmaps and trilinear filtering to keep downsampling smooth.

Broader rule-document and balance-audit drafts remain paused and outside this
checkpoint. M4/Core work waits for the user's command after visual review.
