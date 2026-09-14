# Quackies v1 implementation plan

14 September 2026. **M2 is complete.** The user approved the rules, settings,
shop policy, local autosave/resume and this implementation scope, with final-Day
only simultaneous drawing and final-Night Sleep breaking equal total Twigs.
It changes documentation only. **No Core/CLI or Unity implementation begins in
this checkpoint.** The short product direction is in [PLAN.md](PLAN.md).

## Decision: evolve Core; rebuild the changed game presentation

Keep the solution, Core project, match-session ownership, deterministic random
source injection, owned inventory, stateless rule definitions, legal-action
validation, immutable observations and CLI client loop. Preserve the three client
operations: get a snapshot, get legal actions, execute an issued action.

Replace the rules tied to Quacks: colour/value-only encounter identity,
position-plus-one scoring, nine-round transitions, dice/rubies/flask phases,
old buying restrictions, old final conversion and Normal AI heuristics. Build
new Adventure/Dream views around the new authoritative state. This is a
substantial internal refactor, not a new engine or a rename-only reskin.

Keep the original profile as a regression reference using shared infrastructure.
Do not copy the complete match loop, force duck rules into old coin/ruby fields,
or introduce a generic rules language. New public observation data should use
Sleep, Twigs, Feathers and Day names; retain classic adapters where needed.
New gameplay must not extend the retained legacy QuackiesGame prototype.

## Approved M2 rules and defaults

The ten events in [WORLD_EVENTS.md](v1/WORLD_EVENTS.md) are now accepted initial
rules, alongside the existing [encounters](ENCOUNTER_RULES.md) and numerical
[board/shop data](v1/BOARD_AND_SHOP.md). Approval is not full-match balance proof.
The following defaults are approved. This closes the rules-sheet milestone;
implementation and full-match balance validation remain future work.

| Topic | Approved v1 default |
| --- | --- |
| First draw and empty bag | Draw at least one token before claiming that Day's route rewards. An empty bag ends exploration after the final token fully resolves. |
| Reaching or passing 50 | Place once at 50, resolve the complete token including Exhaustion, then finish safely or worn out as appropriate. No bonus for overshoot and no further token placements. Reaching 50 does not end the match. |
| Starting Feathers | Default 0; offer the same 0–3 starting setting to both ducks. Under current ten-Day sources the conservative latest start is 46 + setting, at most 49, so there is room for a first draw. Reject unsupported settings rather than silently clipping Feathers. |
| Dream shopping | All 11 offers available from Night 1, unlimited stock initially, one chip per token type per Night, within that player's 1/2/3 purchase cap. Variants share a type. Unspent Sleep expires; purchases enter the next Day's bag. No Night 10 shop. |
| Final tie | Rank total Twigs including Dream Twigs first, then frozen retained Sleep from Night 10 before conversion. If both match, the game is a draw. No distance tiebreak or separate safe-player filter. |
| Recovery | No separate flask, rewind or redraw in v1. Splash is only next-token nuisance protection. |
| Worn-out payout details | Retain printed/Reeds/earned-event Twigs and floor(Sleep/2); safe-only haven Feathers, Flowers, flock and safe event rewards do not apply. A final unprotected Pebbles/Brambles penalty still applies before halving/final conversion as documented. |
| Drawing rhythm | Days 1–9: each duck may Draw or Settle independently; completed actions are visible and other ducks may react. Day 10 only: all active ducks commit Draw/Settle before that beat is revealed. Finished ducks leave later beats; AI cannot see a pending final-Day decision. |

On **Days 1–9**, resolve and publish each player's complete action immediately.
There is no paired-decision wait or forced alternating turn order. Human and AI
may use the latest public placements and finished/resting state when deciding
whether to continue. Never expose private Signpost previews or future bag order.
Collective events and Night scoring still wait until every duck has finished.

On **Day 10 only**, freeze the active participant cohort and public state at the
start of each decision beat. Each member commits from that state plus their own
already-known preview. Resolve the committed cohort in fixed player order and
publish placements, logs and new previews atomically after the whole beat.
A duck wearing out or reaching 50 during resolution cannot cancel another
committed action. Evaluate all-finished/Night transitions after the reveal.
This remains a profile rule for the final Day, not a round-nine constant.

For tied total Twigs, the secondary score is **Night 10 frozen retained Sleep**:
include all eligible bonuses and apply worn-out halving first, then preserve
that Sleep value before Dream Twig conversion. Compare it among the players
tied on total Twigs, without an extra Most Rested eligibility check. A further
tie is a draw. Conversion must not replace it with Dream Twig count or a remainder.

Approved Day order: reset temporary state; add the Goose once during Day 5
preparation; snapshot Twig scores and award Dawn gifts; activate last Night's
temporary Most Rested step; prepare the bag and reveal today's shared event;
freeze effective start, then explore. The event deck is shuffled once at match
creation. Nothing in the current event deck grants permanent Feathers.

The zero-start bound is 9 prior haven awards ×2 + 9 Dawn deliveries ×3 + at most
1 temporary step = 46. This is an intentionally loose upper bound, not an
expected trip. New Feather sources or shorter/longer match settings require a
new check. The shared 0–3 setting is approved, with zero as the default.

## Important implementation gaps found in the current code

- **Separate token identity and quantities.** Current
  [Token](../../src/Quackies.Core/Tokens/Token.cs#L5) is colour plus value.
  Movement, danger and shop identity reuse that value. Duck definitions need
  stable IDs, a helpful type or obstacle subtype, intrinsic movement and separate
  ability quantities. A Reeds ×3 is one chip moving one, not a value-three mover.
  Session state must distinguish physical Companion placements from active flock.
- **Make Signpost previews exact and private.** The current
  [draw](../../src/Quackies.Core/Match/BrewingPhaseHandler.cs#L29) selects a fresh
  random bag index, while [PreviewBag](../../src/Quackies.Core/Match/MatchSession.cs#L260)
  samples without fixing the next draw. A duck-profile ordered bag/forced-draw
  queue must guarantee the previewed next token(s), including repeat Signposts.
  Previewing must not place a chip or expose an opponent's future draws.
- **Separate state from display.** Replace old VP/coins/rubies/rat/flask assumptions
  in [player state](../../src/Quackies.Core/Match/PlayerRoundState.cs#L28) with
  clearly owned duck state: today/total Twigs, earned/frozen/remaining Sleep,
  permanent trail, temporary start, effective Day start, final rest, Exhaustion,
  pending nuisances/protection, preview queue and per-event counters.
- **Use one ordered Night resolution.** Collective conditions wait for everyone;
  eligible event/encounter payouts occur once; deductions and wear-out treatment
  precede frozen Sleep; Most Rested precedes spending/final conversion. No UI or
  animation callback may award anything again. See [encounter timing](ENCOUNTER_RULES.md).
- **Generalize commitment and stale-action checks.** Current
  [round-nine decisions](../../src/Quackies.Core/Match/MatchSession.cs#L513) are
  special-cased. Ordinary `draw`/`stop` action IDs can be reused when legal again;
  step/preview-sensitive actions need session revision or sequence checks so
  delayed taps cannot submit a second decision. Hide pending decision kinds in
  the other player's view/log until the Day 10 beat resolves. Days 1–9 publish
  completed actions immediately while still rejecting stale/duplicate input.
- **Rework Normal AI.** Retain its observation/legal-action boundary, not its
  [old heuristics](../../src/Quackies.Core/AI/NormalPolicy.cs#L22). Evaluate exact
  known previews, wear-out risk, haven comfort, Twigs, remaining Days, purchases,
  flock competition and collective conditions. Never inspect the other duck's
  private queue or react to an unannounced pending choice.

## Save and resume: approved v1 requirement

There is currently no persistent match save/load. Restart creates a fresh match;
Unity scene reconstruction and the old fortune rewind do not provide resume.
**Local autosave and Continue game are approved v1 scope.** They remain to be
implemented; this is local persistence with no cloud-account requirement.

Plan serializable state from the foundation even if the persistence adapter is
built later: stable rule/encounter IDs, ordered bags/deck, random continuation
state, pending commitments and previews, event counters, frozen rewards,
purchase counts, settings and format/rules version. Include Day 10 beat
ID/revision, frozen participant cohort and reveal status when applicable, plus
action-issuance sequences on every Day. An observation snapshot is
not a complete save. Current pending choices contain delegates and
[SeededRandomSource](../../src/Quackies.Core/Randomness/SeededRandomSource.cs#L5)
does not export RNG state; these cannot simply be JSON-serialized as-is.

Core should capture/restore authoritative data without Unity or filesystem
references. CLI/Unity hosts own atomic local writes and storage errors. Save
after each completed Days 1–9 action and each completed purchase/other command.
On Day 10, save a pending commitment with its full beat checkpoint or an atomic
reveal, never midway through resolving the cohort. On resume, rebuild views
from the restored session and do not reapply rewards or replay partial animations
as actions. Do not reroll an unfinished decision or a known Signpost preview.
Keep classic randomness/tests stable while adding a resumable duck source.

## Build milestones and review evidence

M0 and earlier M1 experiments remain history. The future M3/M4 ordering is now
changed deliberately: prove the approved board's fit before the full Core build.
Stop for user feedback after each milestone. All builds below are future work.

### M2 — Rules sheet complete

Closed by the user on 14 September 2026. Rules, defaults, exact rewards/prices,
events and persistence scope are approved, including the two corrections above.
Keep this contract stable for the first complete playtest. **M3 has not started
and must wait for the user's explicit command.**

### M3 — iPad layout proof

Create a separate reproducible Unity layout scene using the approved board and
fixed sample data, without a live rules session. Preserve the playable baseline
scene. Use the existing [1133 × 744 fitted viewport](../../unity/Quackies.Unity/Assets/Scripts/Presentation/FittedViewport.cs#L5)
and confirm the target iPad mini viewport during this milestone.

Place exactly 50 stable-ID wells with common token bounds, readable Sleep/Twig/
Feather strips, eight clearly linked havens, player/rest/zzz overlays and a
legible event/Exhaustion summary. Prove the full-screen Concept-B Dream layout
with all 11 offers, nest levels, resources and View adventure. Test representative
occupied spaces, not just empty wells. Taps may open inspection rather than
requiring tiny text to contain the entire rule.

Record normalized layout anchors/bounds in a duck board-layout asset so the
later art catalogue can reuse the measured positions. Confirm no overlaps,
clipped text, ambiguous haven links or hidden rewards at actual display size.
Rebuild twice to check exact IDs and no duplicates. This is **layout evidence,
not a playable game or rules validation**. No new gameplay calculations belong
in these fixed visual fixtures.

Use the accepted native art for fit. Prepare isolated production sprites and
the requested verified 3072 × 2048 board master after geometry is settled;
a concept sheet or a simple upsize is not evidence of newly resolved detail.
Do not regenerate the approved composition to hide alignment problems.

### M4 — Core and CLI, in small compiling checkpoints

| Checkpoint | Result and focused evidence |
| --- | --- |
| C1: profile/data/state foundation | Preserve classic SetOne; introduce duck definitions, ten-Day settings and 50 occupied reward rows. Validate IDs, quantities and all data rows. Design saveable state and profile-owned lifecycle rules. |
| C2: adventure and exact draws | Opening bag, movement, Explore/Settle, Exhaustion, Log/Mud/Splash/Goose and all helpful tokens; ordered private previews, independent Days 1–9 drawing, final-Day commitment support and endpoint/empty-bag behavior. CLI can expose each new action as it lands. |
| C3: one complete Day and Night | All ten event handlers tested in isolated Day fixtures; collective conditions, safe/worn outcomes, Reeds/Flowers/flock, frozen Sleep, Most Rested, Night 1 purchases, capped Dawn delivery, permanent trail and temporary zzz activation. A Day 1 → Night 1 → Day 2 CLI slice works with authoritative breakdowns. |
| C4: complete ten-Day match | Extend the working daily cycle across all ten Days: once-only Day 5 Goose, shared deck, repeated Dawn awards, nest-tier transitions, Day 10 hidden decisions, final haven bonus, conversion and Twig-then-Sleep winners. Run seeded full matches using deterministic legal test policies while Normal is completed. |
| C5: Normal AI and resume | New policy uses legitimate information and the new rewards. Add versioned local save/continue and exact continuation tests. Record representative decisions and match outcomes. |

Each Core source checkpoint and its focused tests are separate commits, pushed
promptly with the checked source/test relationship recorded. Run full original
regressions at meaningful boundaries and full duck checks at the milestone.
Do not wait until the end to adapt the CLI: it is the first runnable client for
each slice. No extra debug-only rule engine or unvalidated prototype shortcut.

### M5 — Connect the Unity game

Adapt the layout proof into Adventure/Dream views driven exclusively by Core
observations and issued actions. Keep the presenter/view/editor split, safe-area
and modal infrastructure; replace the cauldron-specific board, perimeter VP
scoreboard, nine-round header and old currency controls.

Sync only a committed Core build outside Play mode using
[sync-unity-core.sh](../../tools/sync-unity-core.sh), preserving DLL `.meta` identity.
Record the source commit and DLL hash at each handoff. Rebuild the scene,
validate bindings, run a full human/AI match and verify the final score/restart.
Exercise app pause, closure and restoration independently
of scene-builder reconstruction. One agent owns Editor mutations at a time.

### M6 — Balance, readability and export

Record haven landing frequency, wear-outs, purchase variety, lead changes,
Dawn recovery, Companion/Reeds value, event effects and endpoint frequency.
Use multiple deterministic policies/scenarios so agreement between identical
AIs is not mistaken for balance. First tune AI decisions and identify problems;
ask before changing approved rule/reward numbers. Inspect touch targets, colour
and silhouette recognition, overlays, frame/memory behaviour and help text.

Run milestone checks and verify iOS export. Editor success, export and a physical
install are distinct evidence. No device install, release or merge is authorized.
Networking, extra AI levels, more event cards, shorter games, alternative token
rules and cosmetic expansion remain later work.

## Essential scenario coverage

- All 50 rewards, exact haven versus neighbouring ordinary finishes, and worn
  versus safe arrival at 50; nothing scores the next empty space.
- Mud before/after Companions, no negative flock, no retroactive movement;
  repeated Log, Splash expiring on a helpful chip, suppressed final penalties,
  protected/unprotected Goose and Still Air plus Log applied only once.
- Signpost followed by Signpost; one/two/zero tokens left to preview; Mist;
  opponent privacy; immediate visible actions on Days 1–9; hidden pending
  decisions and atomic reveal on Day 10; stale or duplicated actions.
- Every shared event with all eligible, one ineligible and all worn out;
  safe-only versus worn-eligible payouts, Night 10 stacking and conversion.
- Purchase count across reopening/resume, variant types, zero affordable offers,
  Sleep expiry, all-safe/all-worn/tied Most Rested; unequal Twigs outrank Sleep,
  equal Twigs use Night 10 retained Sleep, and equal values on both are a draw.
- Dawn gaps 0/4/5/8/9/large; zero and highest accepted starting setting;
  permanent versus temporary start; Day 5 Goose added once across resume.
- Restore after an ordinary-Day action, while a Day 10 decision is pending, after Signpost,
  after a purchase, after Night rewards and at final scoring; same future state
  and results as uninterrupted play, without duplicated grants or rerolled bags.
