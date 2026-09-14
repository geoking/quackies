# M3 closeout — approved board and presentation

14 September 2026. The user explicitly approved the final visual result and
asked to close M3. **M3 is complete. M4 is unstarted and awaits confirmation of
its implementation plan.** This turn updates documentation/data and its bounded
math audit only; no Core, CLI or Unity implementation is changed.

## Accepted result

The final visual checkpoint is `2c7cd6a`, following `3bd9ce1` (route/readable
typography) and `d068229` (scattered twigs/central chips/Feathers).

- 43 spaces split 14 wetlands / 14 meadow / 15 wasteland, with havens at
  4, 10, 16, 21, 26, 32, 36 and 43. The nest is separate and unscored.
- The first haven's complete payload moved from 3 to 4. Space 3 is 5 Sleep /
  1 Twig; space 4 is 6 Sleep / 1 Twig / 1 Feather. Space 5 begins the 2-Twig
  plateau. The other six nonendpoint havens match both neighbours' Twigs.
- Native oasis at 43: 21 Sleep / 9 Twigs / 2 Feathers. Smaller Feathers sit in
  front of the pool, with winnings directly beneath.
- Distinguishable 108 × 84 biome tiles, fifteen variants with correctly scattered
  twig counts, exact right-middle Twig numerals and bottom moon/Sleep numbers.
  Haven Feathers are prominent in the lower-left pocket. No external reward strips.
- Central shared 64-pixel chip/duck frames, offset (16, -11), with tested spacing
  and clearance from reward numbers, moons, Feathers and neighbouring chips.
- Approved route centres follow paths and shelter entrances; bridge decks stay
  clear. Scenery/log/tree overlaps were corrected in the accepted placement pass.
- Fredoka SemiBold with black outlines; board rewards use the stronger 0.36
  outline and 0.03 face dilation. Twig font is 22; Sleep labels sit 0.84 pixels
  higher for stroke clearance. The footer explains rest-where-you-land scoring.

The current artwork and authoring metadata live in
`unity/Quackies.Unity/Assets/Art/DuckLayout/`. Keep their stable identities and
source pixels. The painted board remains 1536 × 1024; the detailed 3072 × 2048
master is explicitly deferred. The current fixed-data Dream fixture has all 11
offers and View adventure navigation. Core binding, runtime nest/award/event
presentation and further Dream Concept-B polish belong to M5.

## Recorded visual evidence

| Check | Evidence |
| --- | --- |
| Route alignment, 43 centre inspections, 19 actual/simulated mesh comparisons and deliberate collision rejection | [Route refinement](../m3-route-refinement/README.md) |
| Fifteen scattered-twig variants, central encounters, larger haven Feathers and oasis treatment | [Scattered twigs](../m3-scattered-twigs/README.md) |
| Final mini/large scene audits, 150 fitting labels, all 672 chip placements, zero compilation/Console errors | [Reward typography](../m3-reward-typography/README.md) |

Review the [final empty board](../m3-reward-typography/empty-mini.png) and
[final occupied board](../m3-reward-typography/occupied-mini.png). Recorded Game
View targets are 1133 × 744 and 2732 × 2048. These are Editor layout checks,
not physical-device evidence or proof that the new rules are implemented.
The historical original-game tests and earlier visual studies are preserved.

## Documentation and data reconciliation

Canonical `v1/board.json`, `v1/board.csv` and the board/reward table now agree
with all 43 approved Unity rows. The 11 shop prices, event data and encounter
powers are unchanged. The plan, implementation sequence, status, handoff, rule
recap and production brief describe the accepted result rather than superseded
50-space/first-haven-3/reward-strip versions.

The bounded audit is regenerated for 43 spaces and the final haven IDs. It
checks board totals (469 Sleep / 211 Twigs), data identity, shop prices, scoped
bag arithmetic and the [endpoint bound](endpoint-review.md). Default starting
Feathers 0 is bounded to start at most at 42. The approved optional settings
1–3 still require tighter reachability or a specific product decision in C1;
no cap, conversion, lost Feather or removed setting was introduced.

No complete-game balance is claimed. Preserve the unrelated ProjectSettings
preload-removal draft outside this checkpoint.

## Exact next work, after confirmation

Start [M4 C1](../IMPLEMENTATION_PLAN.md#c1--the-first-work-after-m4-approval):
record the existing classic baseline, add the exact duck profile/catalogue,
separate encounter movement from ability quantities, establish saveable state,
complete the starting-position safety check, and expose/verify the foundation
through the CLI. Preserve the classic profile and existing API boundary. C1
ends with compiling, tested foundations; C2 begins real Adventure actions.

The rest of M4 completes one Day/Night, all ten Days, Normal AI and local
save/Continue. M5 binds the approved Unity presentation to that committed Core.
The user has not authorized M4 implementation in this closeout request.
