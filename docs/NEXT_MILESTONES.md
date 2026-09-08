# Next playable milestones

The full target remains all nine rounds of base-game Set 1, human versus AI.
Complete one bounded step, validate it, and publish it before starting the next.
The lead owns Git; workers report ready files and pause writes during staging.

Latest priority: the live scoreboard and full-screen opponent pot, then stable
draw/stop/flask controls with artwork and the active fortune view. Continue to
verify action/text visibility and show only a fixed starting-bag reference. Normal AI now
declines explosion risks and conserves its flask. Add settings and a full AI log,
and implement the optional test-tube rules before any new ingredient-book sets.
Keep the CLI aligned with the same match settings, actions and observations.

1. **Repair the current table.** Author a background camera. Align the human and
   opponent markers with their actual printed spaces. Keep the tabletop fitted
   after changes in Game view size. Verify the saved scene at iPad mini resolution.
2. **Make shopping usable.** Reach shopping through the real draw/stop flow, show
   affordable chips with price and effect, support scrolling or a readable shop
   panel, and verify coins, stock and next-round inventory after a purchase.
3. **Make both boards inspectable.** Add a score-board button with live human/AI
   counters and a round marker. Tap the AI pot to enlarge it; provide a clearly
   labelled Back button. Keep these views tied to the same match observations.
4. **Complete the fortune deck.** Add cards in small groups using event lifecycle
   hooks and per-match state. Commit each compiling Core group, then its focused
   tests. Continue until all 24 supplied cards and their timing are covered.
5. **Prove complete matches.** Exercise all nine rounds including shopping,
   explosions, ingredient and fortune choices, final conversion and ties. Test
   the actual Unity controls, restart and scene rebuild; inspect iPad layout and
   attempt an iOS export. Report device verification separately.

For each step, explain the changed behavior in the commit body and record the
validation in PROGRESS.md. Preserve all original art image bytes. Keep future
ingredient sets and cards outside the Unity presentation layer.
Keep HANDOFF.md current at checkpoints so the user can continue after a usage
interruption; prepare it before the last 1% whenever limits permit.
