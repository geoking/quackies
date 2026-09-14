# M3 reward-number readability

14 September 2026. A typography refinement of the scattered-twig board.

- The 86 board reward labels use a dedicated Fredoka material with a stronger
  black outline (0.36 instead of 0.24) and slight face dilation (0.03).
- Normal-tile Twig numbers increase from 21 to 22 board units. Sleep numbers
  move upward by 0.84 board pixels to keep the thicker stroke inside the tile.
- Other interface labels retain their existing material. Tile positions,
  rewards, artwork and chip placement are unchanged.
- The builder creates/reuses `Fonts/Fredoka-SemiBold SDF - Reward Outline.mat`
  alongside the existing font, keeping the same SDF atlas. Existing glyph
  checks allow 1.1 units for the stronger outline, previously 0.65.

[Empty board](empty-mini.png) · [Occupied board](occupied-mini.png)

Actual 1133 × 744 and 2732 × 2048 scene audits pass. All 150 labels fit;
86 reward labels receive the stronger material. All 672 chip/space combinations
remain clear of rewards. Compilation completed without errors and the Console
error query is empty. Mini and larger renders were visually reviewed for
legibility and open digit counters. Adjacent JSON files retain the checks.

This is a completed typography checkpoint, awaiting visual feedback. M4/Core
remains unstarted; pre-existing document and ProjectSettings drafts are preserved.
