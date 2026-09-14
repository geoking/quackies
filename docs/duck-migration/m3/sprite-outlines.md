# M3 sprite crop and outline metadata

`encounter-crops.json` records the source rectangles and source-measured silhouette
polygons for the approved M3 layout source sheets. Root corrected the initial approximate crops against the original image pixels,
including Companion and all Reeds variants. Read-only colour-boundary analysis
produced simplified outline coordinates; no raster pixels were written.
Coordinates are integer
source pixels with the origin at the top left. Each outline is relative to its
crop's top-left corner.

The metadata contains 22 finite, disjoint source rectangles: five everyday
encounters, three Tailwind variants, three Reeds quantities, five white
obstacles, four player ducks, one Most Rested marker, and the fixed Dream nest
crop. Token outlines retain the cardboard rim and its concave/rounded shape;
the five white obstacles use the common outer octagonal bevel while leaving
their nuisance illustrations untouched. Captions, headings and sheet shadows
are outside the crops. The Dream nest crop is intentionally rectangular and has
no outline because it is a fixed presentation panel rather than a sprite mesh.

These are layout metadata only and do not alter source PNG pixels. The builder
applies the rectangles and outlines through Unity's sprite import API, preserves
stable sprite identities, and uses the sprite mesh when rendering UI images.
The polygons follow measured outer rims; the completed [M3 review](validation.md)
checks their rendered bounds at 1133 × 744 with all 16 encounter variants.
The Dream crop stops above the old caption. Production alpha extraction remains
separate from this native source-sheet proof.
