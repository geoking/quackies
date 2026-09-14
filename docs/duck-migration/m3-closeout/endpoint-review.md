# M3 43-space endpoint review

14 September 2026. This bounded rules review uses the approved 43-space board,
the current encounter rules and the ten-card World Event deck. It does not
simulate complete matches or authorize M4 implementation. The reproducible
calculations and input assertions are in
[`balance-audit.py`](../v1/balance-audit.py) and
[`balance-audit.json`](../v1/balance-audit.json).

## Default setting is bounded before the endpoint

With zero starting Feathers, the effective start on every Day, including Day
10, is bounded to **42**. The required first draw therefore has room to place
a chip, including arrival at endpoint 43.

The proof is intentionally conservative:

1. The starting helpful chips provide at most 7 movement, five ordinary white
   chips can resolve safely, and Rain-Softened Seeds can add 2. Day 1 therefore
   moves at most 14 while safe and can earn at most 1 haven Feather.
2. A lethal sixth white can finish the Rain case at space 15. Worn-out ducks
   retain its 4 printed Twigs. Pocket of Driftwood is a different event, so its
   no-Rain maximum is space 13: at most 3 printed Twigs plus 1 event Twig. Day 1
   therefore yields at most 4 Twigs, limiting the default Day 2 Dawn Delivery
   to 1 Feather.
3. Night 1 allows one purchase. Even granting the strongest 6-movement chip,
   Day 2 safe movement is at most 20. Computing Day 1 Twigs separately for each
   starting setting gives Day 2 effective-start bounds of 3, 4, 6 and 7 for
   settings 0–3. Day 2 therefore reaches at most 27, before the first 2-Feather
   haven at 32, and its haven award is also at most 1 Feather.
4. Across Nights 1–9, haven awards are at most `1 + 1 + (7 × 2) = 16` Feathers.
   Dawns 2–10 are at most `1 + (8 × 3) = 25` Feathers for the default setting.
   One temporary Most Rested step gives `16 + 25 + 1 = 42`.

This is an upper bound, not a likely path or a complete-game balance result.
Day 10's haven Feather is recorded after that Day and cannot affect a later
start.

## M4 gate for starting settings 1–3

The approved shared starting-Feather setting remains 0–3. Settings 0 and 1 can
yield at most 4 Twigs on Day 1 and therefore at most 1 Dawn Feather on Day 2.
Settings 2 and 3 can reach a 4-Twig row during the Pocket of Driftwood event and
also gain its event Twig, so their Day 2 Dawn upper bound is 2. The resulting
conservative effective-start bounds for settings 1, 2 and 3 are 43, 45 and 46.
Because the first-draw rule needs an effective start strictly below 43, these
loose bounds do not certify the nonzero settings. They also do not show that any
real match can reach those starts.

Before enabling settings 1–3 in M4, C1 must either establish a tighter exact
reachability proof for those settings or obtain an explicit product decision
for start-at/past-endpoint handling. This review does not approve clipping an
earned Feather, adding a cap, reducing the supported settings or changing the
first-draw rule.

## Evidence boundary

The audit checks that the CSV and JSON contain identical 43 rows, havens at
4/10/16/21/26/32/36/43, the 14/14/15 biome split, totals of 469 printed Sleep
and 211 printed Twigs, the haven rewards, endpoint 21 Sleep/9 Twigs/2 Feathers,
and all shop prices. Its movement and wear-out sections retain their documented
simplifications, including omitted ordinary Splash protection in the opening
movement traversal. Full purchasing histories, stopping policy, opponent
interaction, Most Rested feedback and complete World Event sequencing remain
for later balance validation.
