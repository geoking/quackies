# Duck migration status

Updated 14 September 2026. **M2 is complete; M3 is reopened for revision.**
The earlier separate iPad layout proof uses fixed sample data and remains
historical validation. Gameplay and persistence implementation remain M4 work
and wait for the user's command.

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

## Open: M3 revision

The revision checklist and evidence contract are in
[m3-revision/README.md](m3-revision/README.md). Current completion covers the
board layout at the approved source size, resolution-independent UI and native
production sprites, using 1133 × 744 only as a preview/check and also verifying
a larger iPad/Mac viewport. The detailed 3072 × 2048 production master is
explicitly deferred; retain the higher-resolution authoring plan and 4096 import
cap. All 50 spaces must be one centered route with near-even
arclength spacing across biomes; the wetland arms must have approximately even
counts. Seven haven tiles must sit on path beside painted shelters without
covering them, while space 50 uses the oasis itself. Haven Feather art must be
integrated (one Feather for the single-Feather haven treatment and two for the
wasteland treatment), with no floating Feather decorations. Larger tiles/token
faces remain explicit review items. Dream Concept-B likeness and fun engaging
typography are minor M5 implementation follow-ups, not current M3 completion
gates. Sleep/Twig/haven row adjustments are root-owned and pending; the final
oasis remains 21 Sleep / 9 Twigs / 2 Feathers unless root changes it.

M4 Core/CLI and persistence, M5 connected Unity, and M6 balance/export remain
after M3 revision. The existing engine/API/CLI will be refactored, not
restarted.

## Art and historical evidence

The [approved board](concepts/2026-09-12-approved/board-art-approved.png) and
historical proof remain available for reference. The requested detailed
3072 × 2048 master is deferred production work; native production sprites and
revised geometry remain open for this revision. The approved
[16 encounter designs](concepts/2026-09-13-agreed-token-set/README.md) are opaque
concept sheets are isolated with Unity sprite rectangles and outlines for this
proof, preserving their source bytes. Dream Concept B, V2 player ducks and
the [zzz marker concept](concepts/2026-09-14-most-rested/README.md) are available
in the completed layout proof.

M0's build/129 tests/CLI results in [BASELINE.md](BASELINE.md) are historical.
M2 closure is specification approval, not a completed or playtested duck game.
The pre-existing ProjectSettings preload removal is unchanged and uncommitted.
