# Future Quackies milestones

The active priority is now the gradual duck migration. See
[its plan](duck-migration/PLAN.md) and [review status](duck-migration/STATUS.md).
M0 was accepted and M1's visual-test milestone is complete. Pause for feedback
on the Unity style preview before M2 terminology work. The items below
remain backlog and are not authorization to start additional work now.

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

Continue small reviewed source, test and Unity checkpoints; push each and record
validation in PROGRESS.md. Preserve the supplied artwork and keep rules outside
Unity presentation code. No new goal, main merge or release is started here.
