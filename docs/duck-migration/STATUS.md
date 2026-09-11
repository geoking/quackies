# Duck migration status

Updated 12 September 2026. **M1 art selection is approved; work is paused at
the user's request after a documentation checkpoint.**

## Current accepted direction

- The user selected **exec-9d44cb08-9cb9-4367-8a00-4a5f8c60b78b**, the first V10
  candidate. Its exact native image is now the [approved base board](concepts/2026-09-12-approved/board-art-approved.png);
  [selection/provenance](concepts/2026-09-12-approved/README.md) distinguishes it
  from the later clearance-corrected alternatives.
- The V2 duck tiles and V3 orange seed token are approved. V5's bubbly painted
  wells, icons and footer remain the overlay style reference. Its old placement
  coordinates were rejected and must not be reused unchanged.
- The [consolidated plan](PLAN.md) defines the layered tabletop board, category
  shapes/colours, readable future tokens and exact reward overlays.
- Bonus-die presentation becomes **Most Rested Duck reward**, a small end-of-day
  bonus. Fortune cards become **World Events**, one shared situation revealed
  at the start of each Day. Existing eligibility, outcomes and event mechanics
  remain the initial rules baseline. These names are planned, not implemented.

## Evidence and remaining work

The approved board has no tiles or labels. Exact 53-space alignment, shelter
mapping, token clearance and iPad readability remain unverified. The V10 audit
that passed sampled widths applies to a later image, **exec-bada2b21**, not the
user-selected **exec-9d44cb08**. Its initial narrow-corridor findings remain
relevant to the eventual layout pass.

The approved native image is 1536 × 1024. The requested detailed 3072 × 2048
master is still outstanding; no resampling or separately billed API fallback
was performed. An earlier API-workflow question remains unanswered.

Eight scenic shelters are approved. Core retains 15 ruby scoring positions;
resolve their visual representation before playable integration. The unchanged
Penny/Twig table and next-empty-space semantics remain authoritative.

M0 was accepted with a clean build, 129 passing tests and a CLI smoke:
[baseline evidence](BASELINE.md). V1's technical Unity checks remain historical
evidence of a rejected visual style: [M1 evidence](evidence/m1/README.md).
V2–V10 concept folders and [PROGRESS.md](../PROGRESS.md) preserve subsequent
image iterations and their validation; they are not current Unity evidence.

## Resume gate

Wait for the user's command. No Unity calls/imports, scene construction, code
changes, further image generation or next milestone follow this plan update.
When authorized, read PLAN.md and inspect live Git/Editor state before acting;
use the accepted board reference, not the highest-numbered generated image.

Branch: `codex/duck-game-milestone-0`. Lead owns small commits/pushes and must
preserve the pre-existing ProjectSettings preload removal. No main merge or
release is authorized. The playable baseline remains usable and unchanged.
