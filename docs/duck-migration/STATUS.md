# Duck migration status

Updated 14 September 2026. **Board rewards, prices and all ten World Events approved.
Implementation plan reviewed; code and Unity work remain paused.** The latest encounter artwork is approved.

The [plan](PLAN.md) and [concise rules](RULES_AT_A_GLANCE.md) now include Mud's
active-flock loss, Splash's immediate-next-chip nuisance protection, Dawn gifts capped
at three Feathers (1–4 behind: 1; 5–8: 2; 9+: 3), full Twigs/rounded-down half Sleep when worn out, Day 10 haven +2
Sleep, 4-Sleep Dream Twig conversion and the final Most Rested +1 Dream Twig.
Nights 1–9 still grant temporary start +1; a new [zzz marker concept](concepts/2026-09-14-most-rested/README.md)
represents it beyond the updated Feather trail. Approved endpoint: 21/9/2.

## Reviewable proposal

- [All 50 rewards and 11 prices](v1/BOARD_AND_SHOP.md), with JSON/CSV data.
- [Ten World Events](v1/WORLD_EVENTS.md), shuffled once, one per Day, no repeats.
- [Encounter ordering](ENCOUNTER_RULES.md) and worked Night/final examples.
- [Reproducible bounded bag audit](v1/balance-audit.py) and [results](v1/balance-audit.json).

Board values, prices and all ten events are approved. Remaining marked
interpretations/defaults and autosave scope still need review. Public rules now say “token types” rather than “families”. The audit
checks exact bag arithmetic and opening affordability, not full-game strategy,
Day 5 observed losses or the combined Feather/Most Rested feedback loop.

The new Dawn cap bounds a default ten-Day start to at most 46, below the
endpoint. Nonzero starting-Feather settings and draw overshoot still need rules.
The [implementation plan](IMPLEMENTATION_PLAN.md) recommends exact endpoint
rules, a shared 0–3 starting setting, simultaneous decision beats and local
autosave. Shop/housekeeping defaults also need review. No complete balanced-game
or runtime milestone is claimed.

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

Stop for the user's review. Close outstanding defaults, then await M3: a
separate iPad layout proof before the M4 Core/CLI build. Reuse Core/CLI rather than restarting.
Inspect live Git/Editor state before later implementation; root owns integration
and pushes. Preserve the unrelated ProjectSettings preload removal.
