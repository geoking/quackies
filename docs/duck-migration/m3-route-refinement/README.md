# M3 route and readable-type refinement — 14 September 2026

Review checkpoint for `Assets/Scenes/DuckLayoutProof.unity`. This keeps the selected
decorative, clearly bordered tile treatment and adjusts its placement on the
existing painting. The fixed M3 proof still has 43 spaces: 14 wetlands, 14 meadow
and 15 wasteland. This is not the playable duck rules implementation.

## Review images

- [Empty board, iPad mini preview](empty-mini.png)
- [Occupied board, iPad mini preview](occupied-mini.png)
- [Empty board, larger preview](empty-large.png)
- [Occupied board, larger preview](occupied-large.png)
- [Rest inspection](inspection-mini.png) and [Dream catalogue](dream-mini.png)

## What changed

- Reviewed and redistributed the complete route, keeping both bridge decks clear.
  Moved the lower meadow 13/5 and lower wasteland 12/8 tiles back onto the path.
  Shifted the upper wasteland approach away from the shelter's logs and kept
  the lower bend above the fallen branches and foreground rocks.
- Moved the first haven to space 4 by swapping the full reward and shelter
  payloads of spaces 3 and 4. The haven now gives 6 Sleep, 1 twig and 1 Feather;
  space 3 gives 5 Sleep and 1 twig. All other reward payloads are unchanged.
  The eight visual havens are 4, 10, 16, 21, 26, 32, 36 and 43.
- Repositioned shelter tiles near their painted entrances while retaining a
  usable route. Raised the oasis anchor 30 source pixels and its reward backing
  another 12 pixels relative to its anchor, making room for the final approach.
- Kept tiles at 108 × 84 board pixels. Reduced chip frames from 78 to 72 pixels,
  moved them right toward the painted twigs, and adjusted their vertical offset.
  The horizontal limit respects each tile's twig and Feather artwork with a
  two-pixel frame gap. The resting duck uses the same placement calculation.
- Added the OFL-licensed Fredoka SemiBold face with a black SDF outline throughout
  the proof. Reward numbers are larger; their text boxes preserve the painted
  centres while accommodating the font's line height. Expanded inspection detail
  space and fitted the compact Night label to prevent wrapping overflow.

All 23 existing PNG assets, including the board, tile variants and encounter
artwork, remain byte-identical to the previous checkpoint. The painted board is
still 1536 × 1024; the larger render does not replace the deferred board master.

## Validation

- Unity 6000.6.0f1: compilation complete with no errors; Console error query empty.
- Actual Game View render targets verified at 1133 × 744 and 2732 × 2048.
  Both rendered scene audits pass. Empty and occupied images were visually
  inspected for path centring, shelter placement and scenery collisions.
- All 43 centre raycasts and PointerClick inspections pass, including the new
  space-4 haven. All 11 Dream offers can be hit and inspected with no active
  text overflow. All 150 TMP labels use the rounded outlined font and fit.
- The new editor audit simulates all 16 native chip meshes at every one of the
  42 chip-placement spaces: 672 combinations. It checks reward glyphs with
  outline clearance, painted moons/twigs/Feathers, other chips, the resting duck
  and oasis Feathers. Both empty and occupied configurations pass.
- The simulation matches Unity's actual CanvasRenderer vertices and triangles
  for all 19 active sample meshes. Deliberately coincident chips and a chip on
  a painted moon both fail the audit; restoring their positions returns PASS.
  Transparent sprite corners no longer produce false rectangle collisions.
  Existing tile bounds, reward containment, data, reference and overflow checks
  remain in place.

Tool arguments and outputs are retained in the adjacent JSON files. Source and
bitmap hashes are recorded in [validation.json](validation.json). The Unity
menu `Quackies/Build Duck Layout Proof` reconstructs the saved scene from the
layout data and builder; `Quackies/Write Duck Layout Audit` runs its scene audit.

## Scope and next decision

The user will assess this refinement before any further direction change.
M4/Core is unstarted. Existing rule-document and ProjectSettings drafts were
preserved outside this checkpoint. In particular, the visual decision to move
the first haven to 4 must be reconciled with the canonical rules tables before
the future Core implementation. No iOS export or physical-device test is claimed.
