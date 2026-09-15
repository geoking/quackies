# Quackies Architecture

The implementation described below is the completed original rules baseline.
On 13 September 2026 the user accepted a distinct Day/Dream rules direction.
The planned evolution is in [duck-migration/ENGINE_EVOLUTION.md](duck-migration/ENGINE_EVOLUTION.md)
and the current [plan](duck-migration/PLAN.md). M4 has now implemented the full
ten-Day duck Core/CLI baseline described below. M4.5 completed the shared planning and evaluation transition, including
user-approved price tuning, before M5 binds this boundary to Unity. The
later classic sections document the retained reference profile; its currencies
and phase rules do not prescribe the duck game.

## M4 shared boundary

M4 now evolves that baseline. `MatchSession<TView>` shares command dispatch and
authorization between typed profiles. The nongeneric `MatchSession` remains
the classic compatibility facade; its original state and phase orchestration
now live in internal `ClassicMatchRuntime`. The duck factory uses a typed duck
observation so Sleep, Twigs, Day and permanent trail remain distinct from
classic fields. Each session owns one runtime; clients never access its state.

Issued actions carry internal match/player/revision/window authorization.
`Execute` rejects repeated, stale and foreign commands, then rechecks current
profile legality. Independent actions by the other player do not invalidate a
still-legal command; phase/Day/final-decision-beat changes invalidate its window.
The classic action IDs remain compatible, but clients must submit the returned
action object rather than reconstructing commands.

The duck catalogue is immutable and checked against canonical board/shop data.
Definition identity, physical-chip identity, movement, Exhaustion and Reeds
quantity are separate concepts. `ResumableRandomSource` has portable versioned
PCG state for later exact save/Continue; the classic random sequence is unchanged.
The M4 implementation record and checkpoint evidence are in
[duck-migration/m4/README.md](duck-migration/m4/README.md).

### Duck daily cycle and calendar (C1–C4)

`MatchSession.CreateDuck(seed)` creates the same session boundary with a
`DuckMatchView`. `DuckMatchRuntime` owns typed state, the shuffled event deck,
ordered physical-chip bags and the resumable random source. All ducks start at
nest 0 with zero Feathers. Snapshots expose public placements and scoring but
only the observing duck's exact Signpost preview; bag composition does not
disclose future draw order.

`DuckAdventureHandler` resolves complete chips before endpoint/empty-bag finish
or wear-out. Days 1–9 actions are immediate. Day 10's typed commitments freeze
each active cohort until all decisions arrive, then reveal in a fixed order.
`DuckNightResolver` evaluates collective conditions across the completed cohort
and applies rewards once. Immutable Night outcomes preserve the breakdown;
frozen Sleep determines Most Rested before any spending.

`DuckDreamHandler` spends only remaining Sleep, tracks purchase type and nest
capacity, and adds physical chips to inventory for the next bag. After both
ducks finish shopping, `DuckDayPreparation` snapshots Twig deficits, awards
Dawn Feathers using the 3/7/11 thresholds, activates the temporary Most Rested
step and rebuilds/shuffles each inventory into its next bag. Permanent trail
and temporary movement remain separate. No effective-start cap is applied.

The calendar now continues through all ten Days, adds the Goose once on Day 5
and changes the nest purchase allowance at the approved Day boundaries.
Night 10 applies Dream Twigs once and exposes immutable final standings ranked
by total Twigs and then frozen retained Sleep. It offers no further shop or
Dawn. Full-game tests preserve the original classic regressions.

### Duck policy and persistence (C5)

`IDuckPlayerPolicy` accepts only a `DuckMatchView` and issued legal actions.
`DuckNormalPolicy` is deterministic and stateless. Its bounded estimates use
private previews available to that duck or remaining bag composition, public
opponent progress, haven rewards, nuisances, Most Rested and remaining Days.
Its purchase heuristic weighs movement, direct Twigs and portfolio composition.
These are decision heuristics; Core phase handlers remain the authority on
outcomes. Policy explanations are available for developer review and never
expose the opponent's private previews in the normal CLI.

`DuckSaves.Capture` returns a detached, serializer-neutral `DuckSaveData`.
It includes format/profile/rules versions, exact bag/deck order, PCG state,
physical identities, all counters and protections, purchases, Night outcomes,
awards/history, Day 5 insertion, pending final-Day commitments, final standings
and per-player command revisions. `Restore` validates supported data and its
cross-field invariants, deep-copies it, and creates a fresh command scope.
Restoration neither replays actions nor generates new random draws. Local
saves contain private state for the host and must never become policy inputs.

The CLI owns JSON and storage. `DuckSaveStore` writes and flushes a temporary
file beside the destination before replacement, retaining a validated previous
action in `.bak`. Continue reports backup recovery when the primary file is
unreadable. A failed write stops play and preserves the preceding save.
Save after each completed command, including a pending final-Day commitment;
the synchronous session never exposes an intermediate cohort resolution.

The CLI selects the duck profile with `--profile ducks`; human versus Normal
is the default, and `--two-player` enables explicit developer controls.
`--demo-game` runs both ducks under Normal; `--demo-day` retains the short
scripted slice. `--inspect` reads the catalogue, `--continue` restores a local
game, and `--save path` selects a save file. Interactive play saves by default;
demos save only when requested with `--save`, or when continuing a saved game.
Unity still uses its previous compiled Core DLL; M5 owns runtime binding and
the DLL update. M4 closeout evidence is recorded in the linked implementation
record.

### M4.5 shared planning and evaluation boundary

M4.5 keeps the approved ten-Day, 43-space rules and data authoritative in Core.
`DuckAdventureRules` is the pure shared transition used by the live adventure
handler and bounded Normal planning, so evaluation cannot grow a second rules
engine. Planning branches over observable contingent draws and current/future
Signpost previews, permits stopping after each future draw, and considers whole
Night shopping bundles including `FinishDream` within bounded work. Final-Day
choices compare conservative final-score bounds; when a current rest certainly
loses, an optimistic bound over the observable remaining bag can preserve a
possible recovery beyond the completed search horizon. This is a policy
estimate, never an alternative scoring rule or access to hidden order. The
evaluation runner drives real issued legal actions and records reproducible
JSONL telemetry for baseline, candidate and diagnostic policies; its current
pilot and declared development/holdout limits are recorded in the [M4.5
working record](duck-migration/m4-5/README.md) and [run protocol](duck-migration/m4-5/RUN_PROTOCOL.json).

The approved M4.5 prices are rules revision 2 for new games. `DuckMatchState`
retains an immutable rules revision; restoration selects the matching catalogue,
so revision 1 saves keep their original shop prices. Save format 1 and the duck-v1
product profile remain unchanged. Normal's Night bundle pricing comes from
`DuckMatchView.ShopOffers`, matching both the legal actions and the running
match's prices. The implementation and 332-test validation are recorded in the
[promotion review](duck-migration/m4-5/PROMOTION.md).

Quackies has one rules engine with two front ends: Unity and a command-line
debugging client. Neither front end decides whether a move is legal or awards
game resources. Both submit actions issued by the same match session.

## Core

`Quackies.Core` targets `netstandard2.1`. It must never reference `UnityEngine`,
Unity packages, asset APIs, MonoBehaviours or ScriptableObjects. Rules and policy
tests run without opening Unity.

### State, observations and actions

`MatchSession` owns mutable match state, the supply, the random source and phase
transitions. Brewing, evaluation, shopping and ruby spending have separate phase
handlers. `PlayerRoundState` stays internal to Core. A chip remains in the owned
inventory while moving between the bag, pot and a temporary selection.

Front ends use three operations:

1. `GetSnapshot(playerId)` returns a detached, immutable `MatchView`.
2. `GetLegalActions(playerId)` returns the actions available at that moment.
3. `Execute(playerId, action)` validates the action against current state and
   applies it through Core.

Treat action IDs as opaque. In particular, choice IDs include a sequence number;
reconstructing them in a front end would defeat stale-choice protection. Supply
availability is checked again when executing a choice, since another player may
have taken a chip since the last observation.

The physical pot track has distinct numbered positions even when printed coin
values repeat. Evaluation freezes the scoring space before die and ingredient
rewards advance a droplet. Round nine records each active player's draw/stop
decision before revealing either draw.

`OwnBag` supplies the observing policy with its own remaining composition. It is
not a shuffled future draw order. The human presentation uses a fixed starting
bag reference rather than displaying this live composition. Full match history
and recent display messages are observations, never a way to mutate the session.

### Ingredient and fortune extensions

`RuleSet` groups a board track, ingredient definitions, shop definitions and a
fortune deck. A session copies supply counts and card order, so immutable rule
definitions can be shared between matches.

An ingredient extends `IngredientRule` and implements `OnPlaced` and/or
`OnEvaluation`. Its `IngredientContext` exposes narrow operations such as gaining
rubies, advancing a droplet or offering a bag selection. Register the replacement
handler for its color in the `RuleSet`; do not add ingredient switches to Unity.

A fortune extends `RoundEventRule`. Override only the relevant lifecycle hooks:
reveal, brewing start, chip placement, player stop, evaluation start/completion,
or round end. Placement hooks distinguish a bag draw, ingredient selection and
fortune placement. `RoundEventContext` supplies player observations and controlled
effects, including choices whose supply requirements are checked dynamically.
Add a card to the deck through `RuleSet.SetOne(cards)`.

Keep rule objects stateless. If a new rule needs persistent round state, add a
session-owned capability with a clear reset point rather than storing mutable
fields in a shared rule object. Extend a context when a legitimate new effect
needs it, and test that capability without either front end.

The standard Set 1 deck contains all 24 fortune cards, drawn without replacement.
Pass an explicit empty deck for isolated rule fixtures. BrewingRestartState owns
Second Chance’s snapshot and one-time restart decision; the event’s pre-placement
hook protects its opening draws. Placed chips track whether their ingredient
effect is enabled, allowing Strong Ingredient to suppress immediate and deferred
actions while retaining physical position and white value.

### Opponent policy

`IPlayerPolicy` receives only a `MatchView` and legal actions. It cannot access the
session's random generator or inspect the next draw. `NormalPolicy` declines a
draw if any remaining chip could explode the pot. It uses its flask only when
doing so restores a safe draw and satisfies the round-dependent conservation
threshold. `BalancedPolicy` is a compatibility wrapper around this policy.

Difficulty changes belong in policy implementations. Legal actions, ingredient
effects and scoring remain Core responsibilities regardless of the player type.

## Unity

`MatchPresenter` owns the session and adapts Unity lifecycle/input to its public
API. It renders observations, submits exact legal actions and paces the AI through
a coroutine. Views such as `PotView`, `ActionPanelView` and `ReferenceModal` handle
layout and interaction. They do not reproduce rule calculations.

`QuackiesArtCatalog` maps source artwork to sprites and board coordinates.
`InitialSceneBuilder` authors the saved scene and serialized bindings so that
`Quackies/Build and Play Initial Scene` can recreate it. The editor builder and
importer are editor-only code. Crops live in sprite metadata; original art image
bytes stay unchanged.

Unity references a compiled Core DLL in `Assets/Plugins`. After a Core checkpoint,
run `tools/sync-unity-core.sh` outside Play mode and let Unity finish importing.
When workers have uncommitted Core edits, build an isolated committed snapshot
instead; a published Unity checkpoint must identify the Core version it contains.

## CLI

`Quackies.Cli` displays match observations, lists legal actions and runs the same
Normal opponent. It lets the human advance the round after reviewing rewards.
Keep new match settings and player choices accessible here as they reach Unity.

## Verification and remaining migration

Core tests cover rules, invalid/stale actions, immutable observations, shared
supply and deterministic complete matches. Unity checks cover serialized
bindings, layout, hit testing, action callbacks and runtime errors. Editor checks,
an iOS export and a physical-device test are separate evidence.

The early `QuackiesGame` prototype and its small-track tests still exist for
compatibility. `MatchSession` is the current game engine. Consolidating that legacy
surface is a remaining milestone; new gameplay must not extend two engines.
