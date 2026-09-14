# Duck migration status

Updated 14 September 2026. **M2 is complete; M3 is reopened for refinement.**
The earlier separate iPad proof and previous revised-M3 captures are
historical/rejected. The current `board-layout.json` is a provisional 40-space
visual fixture; canonical v1 data remains 50 spaces while the user considers
40 larger versus 45 smaller versus 45 with an extended painted route. M4 is
unstarted and waits for the user's command; no Core gameplay work has begun.

## Approved M2 contract

- Ten Days, all 50 board rewards, eight havens, 11 shop offers/prices, all
  encounter rules and ten shared World Events are approved.
- Days 1–9 use independent Draw/Settle actions whose completed results are
  public. Players may react to each other's progress. Only Day 10 uses hidden
  simultaneous commitments and atomic reveal; private previews stay private.
- Final rank is total Twigs including Dream Twigs, then frozen retained Night 10
  Sleep before conversion (bonuses and worn-out halving included). If both tie,
  the game is a draw. Most Rested's safe-only award remains a separate rule.
- Starting Feathers use the same 0–3 setting for both ducks, default zero.
  Each awarded Feather permanently advances one step; Dawn gives 1/2/3 for
  deficits 1–4/5–8/9+. Maximum effective start is bounded to 49 at setting 3.
- First draw, empty-bag and endpoint handling, one-per-type/unlimited-stock shop,
  Sleep expiry, recovery exclusion and safe/worn payout details are approved.
- Local autosave and Continue game are approved v1 requirements, not implemented
  features. Save/restore must preserve previews, final-Day commitments, random
  continuation, frozen scores and purchase state.

The [plan](PLAN.md), [implementation plan](IMPLEMENTATION_PLAN.md),
[recap](RULES_AT_A_GLANCE.md), [encounter timing](ENCOUNTER_RULES.md),
[board/shop rules](v1/BOARD_AND_SHOP.md) and [events](v1/WORLD_EVENTS.md) define the
closed contract. No M2 rule defaults remain pending. The bounded
[math audit](v1/balance-audit.json) is design evidence, not full-match balance.

## Historical M3 iPad layout proof

The separate `DuckLayoutProof` scene checks exactly 50 spaces, a common token
footprint, occupied reward visibility, eight haven links, duck/zzz overlays and
the full-screen Dream shop with all 11 offers. [Review evidence](m3/README.md)
records target-size captures, two stable rebuilds, pointer checks and zero final
Console errors. This evidence is retained for history, but the user rejected
that board concept and its 1133 × 744/native-art framing; it does not close the
reopened M3 revision.

## M3 refinement open

The refinement decision record is in
[m3-refinement/README.md](m3-refinement/README.md). The current
`Assets/Art/DuckLayout/board-layout.json` is a provisional 40-space visual
fixture, not an approved rules board. Compare 108 × 79.2 wells with the prior
90 × 66 treatment and compare 40 larger against 45 smaller and 45 with an
extended painted route. Candidate validation passes actual 1133 × 744 and
2732 × 2048 audits, 40/40 center raycasts and PointerClick inspections; see
[validation](m3-refinement/validation.md). The detailed 3072 × 2048 production
master is explicitly deferred, while the higher-resolution authoring plan and
4096 import cap remain.

The provisional 40-space fixture proposes haven IDs 3, 10, 15, 20, 24, 29, 33
and 40, with bottom shelter entries aligned at 20 and 33. Every candidate
center must sit on painted path art, including wasteland curves; bridges remain
tile-free with approach/deck gaps between spaces 13/14 and 26/27. Future duck
animation needs explicit bridge waypoints rather than straight interpolation.
Same-biome tile colours, green leafy nest borders and large
integrated Feather 1/2 treatments are covered by the candidate evidence.
Canonical v1 data still has 50 rows and endpoint 21 Sleep / 9 Twigs / 2 Feathers.
The route-count decision and M3 approval remain open. Dream
Concept-B likeness and playful typography remain M5 follow-ups.

M4 Core/CLI and persistence, M5 connected Unity, and M6 balance/export remain
after the route-count decision and visual review. M4 is unstarted and waits for
the user's command.
The existing engine/API/CLI will be refactored, not restarted.

## Art and historical evidence

The [approved board](concepts/2026-09-12-approved/board-art-approved.png) and
historical proof remain available for reference. The requested detailed
3072 × 2048 master is deferred production work; native production sprites and
the revised geometry are ready for visual review. The approved
[16 encounter designs](concepts/2026-09-13-agreed-token-set/README.md) come from opaque
concept sheets and are isolated with Unity sprite rectangles and outlines for this
proof, preserving their source bytes. Dream Concept B, V2 player ducks and
the [zzz marker concept](concepts/2026-09-14-most-rested/README.md) remain
available for the revised layout review.

M0's build/129 tests/CLI results in [BASELINE.md](BASELINE.md) are historical.
M2 closure is specification approval, not a completed or playtested duck game.
The pre-existing ProjectSettings preload removal is unchanged and uncommitted.
