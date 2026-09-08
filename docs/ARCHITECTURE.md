# Quackies Architecture

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

The default fortune deck is currently empty while the complete 24-card deck is
implemented. Partial batches are opt-in test fixtures. Their existence must not
be described as complete base-game coverage.

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
