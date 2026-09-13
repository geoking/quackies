# Duck migration status

Updated 14 September 2026. **Rules/data proposal ready for review; implementation
and Unity work remain paused.** The latest encounter artwork is approved.

The [plan](PLAN.md) and [concise rules](RULES_AT_A_GLANCE.md) now include Mud's
active-flock loss, Splash's immediate-next-chip nuisance protection, uncapped
Dawn gifts, full Twigs/rounded-down half Sleep when worn out, Day 10 haven +2
Sleep, 4-Sleep Dream Twig conversion and the final Most Rested +1 Dream Twig.
Nights 1–9 still grant temporary start +1; a new [zzz marker concept](concepts/2026-09-14-most-rested/README.md)
represents it beyond the updated Feather trail. Endpoint proposal: 21/9/2.

## Reviewable proposal

- [All 50 rewards and 11 prices](v1/BOARD_AND_SHOP.md), with JSON/CSV data.
- [Ten World Events](v1/WORLD_EVENTS.md), shuffled once, one per Day, no repeats.
- [Encounter ordering](ENCOUNTER_RULES.md) and worked Night/final examples.
- [Reproducible bounded bag audit](v1/balance-audit.py) and [results](v1/balance-audit.json).

Values, new events and marked interpretations remain candidates. The audit
checks exact bag arithmetic and opening affordability, not full-game strategy,
Day 5 observed losses or the combined Feather/Most Rested feedback loop.

The finite 50-space route versus uncapped permanent Feather progress remains
unresolved. Proposed shop/housekeeping policies also need review. No complete
implementation-ready or balanced-game milestone is claimed.

## Art and evidence

The [approved board](concepts/2026-09-12-approved/board-art-approved.png) is
unchanged, native 1536 × 1024. A detailed 3072 × 2048 master and exact 50-space,
haven, token and iPad fit are still outstanding. The approved
[16 encounter designs](concepts/2026-09-13-agreed-token-set/README.md) are opaque
concept sheets, not production sprites. The zzz marker is a new review concept.
Dream Concept B and V2 player ducks remain the selected directions.

M0's build/129 tests/CLI evidence in [BASELINE.md](BASELINE.md) is historical.
No new Core/CLI gameplay, scene work or import was performed here; no runtime
checks are implied by documentation/data validation.

## Resume gate

Stop for the user's review. Next resolve the outstanding rules, then await the
explicit implementation milestone. Reuse Core/CLI rather than restarting.
Inspect live Git/Editor state before later implementation; root owns integration
and pushes. Preserve the unrelated ProjectSettings preload removal.
