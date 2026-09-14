# Duck migration status

Updated 14 September 2026. **M2 is complete. M3 has not started and is awaiting
the user's explicit command.** No gameplay implementation or Unity work is
part of this closure checkpoint.

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

## Next: M3 iPad layout proof, on command

Use a separate fixed-data Unity scene to check the selected board's exactly
50 wells, common token footprint, occupied reward visibility, eight haven links,
duck/zzz overlays and full-screen Dream shop with all 11 offers. This is layout
evidence, not a live game. Do not begin scene work, imports or generation until
the user commands M3. Then inspect live Git/Editor state and preserve unrelated
work; root owns integration and one agent owns Editor mutations at a time.

After M3 review: M4 Core/CLI and persistence, M5 connected Unity, M6 balance/export.
The existing engine/API/CLI will be refactored, not restarted.

## Art and historical evidence

The [approved board](concepts/2026-09-12-approved/board-art-approved.png) remains
native 1536 × 1024. The requested detailed 3072 × 2048 master and exact 50-space
fit remain future production work. The approved
[16 encounter designs](concepts/2026-09-13-agreed-token-set/README.md) are opaque
concept sheets, not production sprites. Dream Concept B, V2 player ducks and
the [zzz marker concept](concepts/2026-09-14-most-rested/README.md) are available
for the layout milestone.

M0's build/129 tests/CLI results in [BASELINE.md](BASELINE.md) are historical.
M2 closure is specification approval, not a completed or playtested duck game.
The pre-existing ProjectSettings preload removal is unchanged and uncommitted.
