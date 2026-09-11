# Future Quackies milestones

The active priority is now the gradual duck migration. See
[its plan](duck-migration/PLAN.md) and [review status](duck-migration/STATUS.md).
M0 was accepted. M1's first Unity style preview passed technical checks but was
rejected visually on 11 September. The user approved the V2 ducks/biome style;
the V3 image review refines a single orange seed tile and a precisely labelled
board with eight assigned rests and unchanged Penny/Twig values. Pause after
image inspection. The items below remain backlog
and are not authorization to start additional work now.

The agreed initial playable milestone is complete. All 24 Set 1 fortunes are
enabled by default; 129 Core tests, a nine-round Unity interaction run, native
iPad-aspect inspection and iOS export provide the recorded validation. See
[the evidence](../tools/validation/evidence/2026-09-10/README.md).

Future work, requiring a new selected scope:

1. **Test-tube rules.** Add the second droplet, every printed reward and the choice
   of which droplet advances, with the full board displayed when enabled.
2. **AI-history pane.** Present the existing match history in a separate scrollable
   view, including draws, flask use, stops, explosions and purchases.
3. **Physical iPad validation.** Compile and sign the exported Xcode project, then
   measure touch behavior, performance and memory on the intended iPad mini.
4. **Compatibility cleanup.** Consolidate the retained prototype API when its
   callers can migrate; continue implementing new gameplay through MatchSession.

5. **Biome reward experiment.** Extend the old shelter experiment only after an
   explicit rules specification: preserve the unchanged baseline and compare
   proposed first, middle and final biome reward profiles through playtesting.
   The current art direction does not approve numeric rewards, timing, AI or
   balance changes.

Continue small reviewed source, test and Unity checkpoints; push each and record
validation in PROGRESS.md. Preserve the supplied artwork and keep rules outside
Unity presentation code. No new goal, main merge or release is started here.
