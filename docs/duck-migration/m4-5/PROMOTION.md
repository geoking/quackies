# M4.5 approved price promotion

The user approved the exact price proposal on 15 September 2026. Source
checkpoint `c636390` applies it; `a60ec91` adds and updates its tests. New games
use Tailwind prices **4/8/12** and Reeds prices **8/14/20**. Other prices, token
powers, calendar, board rewards and Dawn thresholds are unchanged.

## Save behavior and architecture

New matches capture **rules revision 2**. Existing revision 1 saves still load
with their original Tailwind 5/10/15 and Reeds 6/11/16 prices. Capture/restore
preserves the match's immutable revision, paid Sleep, current purchases and
future catalogue. Save format 1 and the `quackies.duck.v1` product profile are
unchanged. Unsupported rules revisions are rejected.

The runtime selects one immutable catalogue at creation or restoration.
Normal evaluates earlier purchases and candidate Night bundles using that
match's observed offers. This prevents either retroactive repricing or planning
an old game's bundle with the new game's costs. Board/encounter behavior remains
shared. Core remains independent of Unity.

## Verification

- **84 focused tests pass** across definitions, Day/Dream, policy, saves and
  evaluation accounting. [Log](evidence/promotion/focused.txt).
- **332/332 full-suite tests pass**, including retained original-game coverage.
  [Log](evidence/promotion/full-suite.txt). Full Release build: **zero warnings
  and errors**. [Log](evidence/promotion/full-build.txt).
- Source/tests were pushed as separate checkpoints as requested. `c636390` is
  explicitly a source checkpoint: its old price-specific test expectations need
  the immediately following `a60ec91` test update. The combined tree passes.
- The actual legacy seed-20 final-Day decision was captured from frozen
  `32b5233` and saved under `tests/Quackies.Core.Tests/Fixtures/Duck/`. Restoring
  it still chooses the possible recovery at AI 32 versus Human 40, rather than
  banking haven 10. This preserves the regression when new prices change which
  bags a fresh game would build. It does not promise a successful recovery.
- **144/144 promoted games exactly match** the frozen E proposal's deterministic
  per-Day telemetry and final result: 24 previously validated seeds per each of
  the Normal/Reeds/movement comparisons, both seats. Timing and intentional
  rules-revision metadata differ; actual game data does not. This verifies
  integration, not another independent balance estimate. [Manifest](evidence/promotion/manifest.json).
- New-price CLI seed 42 finishes all ten Days at **AI 41 Twigs / 18 Sleep versus
  Human 40 / 16**, with both seats using Normal. Finished-save Continue preserves
  that result without repeating awards. [Transcript](evidence/promotion/seed42-cli.txt.gz),
  [Continue](evidence/promotion/seed42-continue.txt.gz),
  [save](evidence/promotion/seed42-save.json.gz).
- Continuing the earlier revision 1 seed 42 save preserves its original **56–46**
  result and revision 1. Focused tests additionally cover a legacy mid-Dream
  purchase/restore, old legal prices, paid Sleep and consistent AI bundles.
  [Legacy CLI Continue](evidence/promotion/legacy-seed42-continue.txt.gz).

- Final audit also rechecked the three remaining important comparisons under
  applied prices: Normal/selfplay, weak-opening/Normal and Normal/M4 algorithm,
  300 seeds each and both seats (**1,800 additional complete matches**). Normal
  scores 78.4% against the M4 algorithm (467 wins, 126 losses, 7 draws); weak
  openings score 37.6% against Normal (222 wins, 371 losses, 7 draws). In selfplay,
  a 3–6-Twig Day 5 deficit recovers 54.9% across 91 distinct seeds, interval 44.0–64.8%.
  These findings support retaining Dawn thresholds; they do not prove a causal
  catch-up effect or the absence of every possible exploit. [Final aggregates](evidence/promotion/final-summary.json),
  [readable summary](evidence/promotion/final-summary.md),
  [catch-up protocol](evidence/promotion/catchup-protocol.json),
  [AI-quality protocol](evidence/promotion/baseline-protocol.json).

The initial focused run identified an obsolete accounting-test fixture: the
old cautious/baseline pair no longer purchased Reeds at higher prices. Its
accounting assertions are retained using a Reeds-focused opponent, which
exercises the intended immediate-Twig case. Old explicit affordability
expectations were updated to the approved prices. No scoring rule was weakened.

## Starting position and numerical audit

The regenerated [bounded audit](../v1/balance-audit.json) still proves effective
start **42**, below endpoint 43. The proof grants the strongest possible Night 1
movement purchase regardless of affordability, so reducing its price cannot
raise the granted movement maximum. Feathers remain bounded by 16 from previous
havens, 25 from Dawn and one temporary Most Rested step. This is a legal upper
bound, not a likely path; the observed final revision 2 comparison maximum was 31.

The same audit now uses the approved prices for its limited opening affordability
and purchase comparisons. Reeds 1 and Tailwind 4 each cost 8 and are affordable in
34.347% of its fixed-stop opening outcomes. That simplified model omits optional
early settlement and several effects; complete-game results remain in the
[review report](REPORT.md). The historical excluded starting-three-Feathers
witness is preserved as history and is not replayed under the new contract.

## Milestone status

Price approval and implementation are complete. The user reviewed the results
and game examples and explicitly accepted closing M4.5, with hands-on gameplay
feel testing deferred to Unity. Automated results do not certify fun. No Unity
change, DLL sync or device testing occurred; M5 waits for a separate command.
[Completion audit](COMPLETION_AUDIT.md).
