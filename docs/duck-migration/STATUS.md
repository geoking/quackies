# Duck migration status

Updated 13 September 2026. The user accepted the revised duck-game direction;
this is a fresh-rules proposal and token-art checkpoint. No implementation of
this new direction has started in Core, CLI or Unity.

## Current accepted direction

- Standard play is **ten Days**. A shorter-match setting is deferred. Encounter
  powers and World Events will be original designs, not required Quacks mappings.
- The board keeps the selected **exec-9d44cb08-9cb9-4367-8a00-4a5f8c60b78b**
  composition unchanged. The approved native image is
  [the selected board](concepts/2026-09-12-approved/board-art-approved.png);
  its exact fit is still unverified.
- The playable route has **50 spaces plus the separate starting nest**. Rest
  and score use the final occupied space. The precise 50-row table, endpoint,
  no-draw, rewind and final-night handling remain to be specified.
- **Sleep score** records earned sleep and is frozen for Most Rested comparison;
  the remaining nightly allowance is tracked separately for Dream purchases.
  Most Rested is the highest earned Sleep among eligible non-worn ducks, with
  tied winners sharing a bounded temporary **Start +1 tomorrow** reward.
- Night opens the full-screen Dream view based on
  [Concept B](concepts/2026-09-13-dream-study/concept-b-nest-mat.png), with a
  **View adventure** button and enough room for the complete legal encounter
  catalogue. The initial ten-Day nest schedule covers Days 1–3, 4–6 and 7–10 and allows 1, 2 or
  3 purchases per Night respectively; they are not tied to Twig score.
- Twigs are persistent victory score and are shown in the nest. Each Feather
  automatically and permanently advances the starting trail by one space;
  Feathers have no banking, spending menu, refill, final conversion or
  once-per-Night cap.
- **Dawn Delivery** replaces rat-tail assistance: at dawn, the stork gives
  1 Feather for a 5–9 Twig deficit and 2 Feathers for a deficit of 10 or more.
  Day 1's equal zero Twig scores normally mean no gift. These public criteria
  are initial tuning values, not verified balance.
- Three biomes remain, with the approved eight shelters remapped 2/3/3 across
  the 50 spaces. Each earlier wasteland haven awards two Feathers and the
  endpoint haven three; exact row values and shelter indices are open.
- The user approved the [token-family style](concepts/2026-09-13-token-family/README.md).
  The [updated study](concepts/2026-09-13-obstacle-study/README.md) removes default
  ‘1’ badges, shows Tailwind →2/→4/→6 and adds five distinct white nuisances.
  Revised sheets and exact effects are for review, not production sprites.

The current bag proposal is eight whites plus five colours, with five Exhaustion
safe and the sixth white worn out. The study records seven helpful powers,
four regular mild nuisances and an optional Goose, plus exact effect-free draw
math. These are candidates awaiting review, not approved balanced rules.

The accepted direction and remaining specification work are in [PLAN.md](PLAN.md).
Preserve the tested base game as a reference while defining the complete new
duck rules. Its eight categories/24 events do not prescribe the new rule content.

## Evidence and remaining work

The selected board is 1536 × 1024. The requested high-detail 3072 × 2048
version remains outstanding, and 50-space fit, token clearance, shelter
placement and iPad readability are unverified. Do not apply the later V10
clearance audit to the selected image.

Next bounded work is: write the exact rules/table; implement and test Core plus
CLI; fit the selected board in Unity; validate a playable full-Day/Dream loop;
then run ten-Day AI, balance and full verification. Final-night Sleep,
Feather and Dawn handling must be explicit before implementation.

M0 remains the tested historical reference: clean build, 129 passing tests and
CLI smoke are recorded in [BASELINE.md](BASELINE.md). Existing baseline art
iterations and Unity evidence remain historical until the new direction is
implemented and revalidated.

## Resume gate

No Unity calls/imports, source changes, or gameplay implementation are part of
this checkpoint. When the next milestone is authorized, read PLAN.md and inspect
live Git/Editor state before acting. Root owns the authoritative plan and Git
checkpoints; preserve unrelated work and the pre-existing ProjectSettings
preload removal.
