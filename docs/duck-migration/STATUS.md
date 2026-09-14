# Duck migration status

Updated 14 September 2026. **M2 is complete; the revised M3 layout is ready
for user visual review.** The earlier separate iPad layout proof uses fixed
sample data and remains historical validation. M4 is unstarted and waits for
the user's command; no Core gameplay work has begun.

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

## M3 revision ready for visual review

The revision checklist and evidence contract are in
[m3-revision/README.md](m3-revision/README.md), with the current
[validation record](m3-revision/validation.md). The revised board layout is
ready for user visual review: runtime audits pass at 1133 × 744 and 2732 ×
2048. It uses resolution-independent UI and native production sprites; the
detailed 3072 × 2048 production master is explicitly deferred, while the
higher-resolution authoring plan and 4096 import cap remain. All 50 spaces must
be one centered route with near-even
arclength spacing across biomes; the wetland arms must have approximately even
counts. Seven haven tiles must sit on path beside painted shelters without
covering them, while space 50 uses the oasis itself. Haven Feather art must be
integrated (one Feather for the single-Feather haven treatment and two for the
wasteland treatment), with no floating Feather decorations. Larger tiles/token
faces remain explicit review items. Dream Concept-B likeness and fun engaging
typography are minor M5 implementation follow-ups, not current M3 completion
gates. The revised wells are 90 × 66 design units and token faces are 66 units
(previously 74 × 58 and 54); seven haven wells use native alpha beside shelters
at 3, 11, 19, 27, 29, 37 and 44, with space 50 using the oasis. The endpoint
remains 21 Sleep / 9 Twigs / 2 Feathers. Two stable rebuilds and zero Console errors are recorded
and is not claimed here.

M4 Core/CLI and persistence, M5 connected Unity, and M6 balance/export remain
after visual review of M3. M4 is unstarted and waits for the user's command.
The existing engine/API/CLI will be refactored, not restarted.

## Art and historical evidence

The [approved board](concepts/2026-09-12-approved/board-art-approved.png) and
historical proof remain available for reference. The requested detailed
3072 × 2048 master is deferred production work; native production sprites and
the revised geometry are ready for visual review. The approved
[16 encounter designs](concepts/2026-09-13-agreed-token-set/README.md) are opaque
concept sheets are isolated with Unity sprite rectangles and outlines for this
proof, preserving their source bytes. Dream Concept B, V2 player ducks and
the [zzz marker concept](concepts/2026-09-14-most-rested/README.md) remain
available for the revised layout review.

M0's build/129 tests/CLI results in [BASELINE.md](BASELINE.md) are historical.
M2 closure is specification approval, not a completed or playtested duck game.
The pre-existing ProjectSettings preload removal is unchanged and uncommitted.
