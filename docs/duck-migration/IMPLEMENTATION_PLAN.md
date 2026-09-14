# Quackies v1 implementation plan

14 September 2026. **M2 is complete.** The user approved the rules, settings,
shop policy, local autosave/resume and this implementation scope, with final-Day
only simultaneous drawing and final-Night Sleep breaking equal total Twigs.
M2 changed documentation only. **M3 is complete and approved by the user. M4
Core/CLI implementation is authorized through C3, followed by a progress report.**
The accepted visual result is recorded in [M3 closeout](m3-closeout/README.md);
the short product direction is in [PLAN.md](PLAN.md).

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
| Reaching or passing 43 | Place once at 43, resolve the complete token including Exhaustion, then finish safely or worn out as appropriate. No bonus for overshoot and no further token placements. Reaching 43 does not end the match. |
| Starting Feathers | Every duck starts at nest 0 with zero Feathers. Permanent Feathers come only from safe haven rewards and Dawn Delivery: deficit 0–2 gives 0, 3–6 gives 1, 7–10 gives 2, and 11+ gives 3. Most Rested gives a temporary +1 and never a Feather. The default pre-Day-10 bound is at most 42; no effective-start cap is needed. |
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
A duck wearing out or reaching 43 during resolution cannot cancel another
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

The old generic zero-start bound of 46 was derived for 50 spaces and does not
certify the shorter route. The current contract's bound is at most 42 before
Day 10: 25 Dawn Feathers (one first delivery plus eight later maximum gifts),
16 haven Feathers (one in each of the first two Days plus seven later havens),
and one temporary Most Rested step. The setting-3 witness is retained as
historical evidence for an excluded configuration; its proof JSON/script remain
unchanged. No new clamp, conversion or loss of the one-Feather/one-step rule is
needed.

## Original implementation gaps and their resolution

This audit describes the classic code before C1. C1–C3 have implemented the
duck identity, private draws, state, Night resolution and command authorization
items below; their source boundaries and validation are recorded in
[M4](m4/README.md). Normal AI remains C5 work. Classic references below remain
as context for the migration, not a claim that duck logic still uses them.

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
not a complete save. The classic pending choices contain delegates and
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

M0 and earlier M1 experiments remain history. The deliberate order was to resolve
the visual risk in M3 before implementing the duck rules. That visual gate is now
closed. M4 C1–C3 are now authorized as one bounded run. Stop after C3 for a
progress report; keep compiling checkpoints within the run.

### M2 — Rules sheet complete

Closed by the user on 14 September 2026. Rules, defaults, exact rewards/prices,
events and persistence scope are approved, including final-Day-only simultaneous
drawing and final-Night retained Sleep as the victory tie-break.

### M3 — Complete and visually approved

Closed by the user after the reward-number refinement at `2c7cd6a`. The accepted
fixed-data scene uses 43 spaces split 14/14/15, with havens at 4, 10, 16, 21, 26,
32, 36 and 43. Canonical JSON/CSV now match the accepted layout, including the
full first-haven payload moved from 3 to 4. The oasis gives 21 Sleep/9 Twigs/2
Feathers. Space 4 ends the one-Twig plateau; space 5 begins the two-Twig plateau.

The final presentation has distinguishable biome tiles, scattered twig artwork,
right-middle Twig numerals, bottom moon/Sleep information, prominent lower-left
haven Feathers, centrally fitted chips and a smaller oasis Feather pair with
winnings below. Rounded outlined typography is implemented. The approved route
centres, shelter alignment, bridges and scenery clearances are preserved.

[M3 closeout](m3-closeout/README.md) links the actual mini/large rendered audits,
43 centre interactions, all 672 chip/space checks, 19 rendered-mesh comparisons,
150 fitted labels and final zero-error compilation/Console evidence. This is a
visual proof with fixed sample data; no duck Core session is bound. Preserve the
completed original playable scene as the reference game.

The detailed 3072 × 2048 painted master remains deferred. M5 will bind actual
state/actions and finish Dream Concept-B presentation, nest growth and runtime
overlays. It should build on the approved board rather than reopen its design.
Full-match balance and iOS export remain M6 work. Historical/rejected visual
studies remain in their evidence directories and do not prescribe current layout.

### M4 — Core and CLI, in small compiling checkpoints

| Checkpoint | Result and focused evidence |
| --- | --- |
| C1: profile/data/state foundation | Complete: preserve classic SetOne; introduce duck definitions, ten-Day settings and 43 occupied reward rows. Validate stable IDs, movement versus ability quantities, exact data and saveable state/profile lifecycle. Every duck starts at nest 0 with zero Feathers. |
| C2: adventure and exact draws | Opening bag, movement, Explore/Settle, Exhaustion, Log/Mud/Splash/Goose and all helpful tokens; ordered private previews, independent Days 1–9 drawing, final-Day commitment support and endpoint/empty-bag behavior. CLI can expose each new action as it lands. |
| C3: one complete Day and Night | Complete for the bounded slice: Night/Dream/Dawn/CLI integration, event fixtures and the Day 1 → Night 1 → Day 2 CLI cycle with authoritative breakdowns. |
| C4: complete ten-Day match | Unstarted: extend the working daily cycle across all ten Days, Day 5 Goose, shared deck, repeated Dawn awards, nest tiers, Day 10 decisions, final conversion and winners. |
| C5: Normal AI and resume | Unstarted: add the new Normal policy, versioned local save/continue and exact continuation tests. |

### C1 — The first work after M4 approval

1. **Record the current baseline.** Run the existing solution build, classic
   regression suite and CLI smoke test. Keep the original profile and tests as
   the reference; no Unity edits or DLL sync occur in C1.
2. **Add the duck profile and exact catalogue.** Load/validate all 43 board
   rewards, eight havens, 11 priced shop variants, seven helpful types, five
   obstacles and ten World Event identities. Keep nest 0 separate from scorable
   1–43. Use the agreed data; do not retune prices or powers while importing it.
3. **Separate identity from quantities.** Give encounters stable definition IDs
   and owned-chip identity. Movement, obstacle Exhaustion, Reeds yield and shop
   type are distinct fields. A Reeds ×3 remains one chip moving one space.
4. **Prepare authoritative, saveable state.** Separate persistent Twigs/trail,
   Day position/Exhaustion/flock, frozen and spendable Sleep, purchases, private
   ordered previews, event state and final-Day commitments. Keep serializable
   pending-decision descriptions and resumable randomness in the design from
   the outset. Filesystem writes and complete Continue behaviour arrive in C5.
5. **Record the starting-position contract.** Every duck starts at nest 0 with
   zero Feathers. The default pre-Day-10 bound is at most 42, so no
   effective-start cap is needed. Keep the setting-3 witness as historical
   evidence for an excluded configuration; do not expose an optional starting
   Feather setting or alter the one-Feather/one-step rule.
6. **Expose and check the foundation through the CLI.** Select/inspect the duck
   profile and its initial data/state through the existing client boundary.
   Check exact board/shop/catalogue values, stable identities, independent state
   and original-profile regressions. The bounded CLI checks are:
   dotnet run --project src/Quackies.Cli -- --profile ducks --seed 42 --demo-day
   and dotnet run --project src/Quackies.Cli -- --profile ducks --seed 42 --inspect.
   C1–C3 now establish the bounded Day/Night slice; they do not claim the full
   ten-Day game, Normal AI or working save/resume.

Keep the existing snapshot/legal-actions/execute API shape and one authoritative
match engine. Use focused profile policies/adapters where behaviour differs;
Core stays independent of Unity. The visible C1 handoff is a compiling solution,
validated duck catalogue/initial state, retained classic checks and small source
and test commits. C2 then implements real drawing, movement and settlement.

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

- All 43 rewards, exact haven versus neighbouring ordinary finishes, and worn
  versus safe arrival at 43; nothing scores the next empty space.
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
- Dawn gaps 0/2/3/6/7/10/11/large; zero starting Feathers and threshold gifts;
  permanent versus temporary start; Day 5 Goose added once across resume.
- Restore after an ordinary-Day action, while a Day 10 decision is pending, after Signpost,
  after a purchase, after Night rewards and at final scoring; same future state
  and results as uninterrupted play, without duplicated grants or rerolled bags.
