# M4.5 — AI quality, balance and playability review

Authorized 15 September 2026 at the user's request, **before M5 Unity integration**.
M4 remains a completed Core/CLI implementation milestone. This new gate addresses
the quality of play and the balance evidence that its correctness tests did not
establish. Implementation is now active; [the working record](README.md) tracks
actual changes, checks and remaining evidence separately from the plan.
B1/B2 implementation is complete. B3 automated analysis and a readable
[review report](REPORT.md) are complete; human play feedback remains pending.
B4 awaits review of the exact isolated price proposal before promotion.

Use the existing ten-Day, 43-space game and approved rules as the baseline.
Improve and assess play through Core/CLI first, then review any proposed balance
changes before binding the game into Unity. M6 will confirm the resulting balance
in the connected game alongside readability, performance and export checks.

## Why this comes now

The [M4 policy review](../m4/POLICY_REVIEW.md) identifies limited look-ahead and
greedy shopping. In the saved seed-42 match, both seats use Normal: each settles
after one Seed on Day 1; 18 of 20 daily rests are in wetlands, two in meadow and
none in wasteland. Fifteen rests are at haven 4 or 10. These observations justify
investigation; one match does not establish typical rates or defective rewards.

The aim is meaningful push-your-luck decisions, useful bag-building choices,
credible recovery from a poor start and an oasis that feels worth pursuing.
Leads should matter without making the outcome feel settled too early. The AI
should pursue winning decisions; travelling furthest is not itself its objective.

The user clarified the balance target during B3: **Reeds-focused and
movement-focused bags should both be credible ways to win; reaching the oasis
is optional.** The two approaches should be fairly evenly balanced, with neither
becoming an automatic winning choice. Assess comparable strategic strength
across opponents and builds, rather than forcing one isolated matchup to exactly
50/50. This target does not by itself approve any specific price or rule change.

## Ordered checkpoints

| Checkpoint | Work and reviewable result |
| --- | --- |
| B1 — Establish the baseline | Add a repeatable CLI evaluation runner over actual Core sessions and issued actions. Capture full-game metrics, decision reasons and representative saved matches for the existing Normal and simple cautious/adventurous reference strategies. Separate faults in rules from weak decisions and balance hypotheses. |
| B2 — Improve Normal | Evaluate useful sequences of draws toward later rewards, including the option to settle along the way. Improve shopping by comparing affordable combinations within the current Night's budget and purchase slots. Retain sound high-risk stops. Add decision-quality regressions and compare with the saved baseline on unchanged rules. |
| B3 — Assess the game | Run multiple strategies, seeds and seat assignments; measure pace, comeback rates, haven/region/oasis use, purchases, events and exhaustion. Review representative human-versus-Normal CLI games and prepare a concise report of findings and proposed changes, including a recommendation to retain values where evidence supports that. |
| B4 — Reassess and stabilise | Review results with the user. If balance changes are agreed, implement small coherent adjustments, rerun the affected comparisons and full regressions, update canonical rules/data and migration notes, and record the resulting baseline. Stop for milestone review before M5. |

## Evaluation design

Keep game outcomes authoritative in Core; the runner supplies actions and
collects observations. Policies use only information available to their duck:
own bag composition and revealed previews, public opponent progress and the
shared event. They cannot inspect save data, hidden bag order, opponent previews
or an unrevealed Day 10 commitment. Planning randomness must be separate from
the game's RNG, reproducible and bounded. Do not create another rules engine
inside the AI or evaluation scripts.

Use a 24-seed pilot to verify instrumentation and planning cost, then a declared
set of 100 seeds per selected strategy matchup, playing both seat assignments.
Include baseline-versus-candidate, equal-policy and mixed cautious/adventurous
opponents, plus targeted movement-heavy and Reeds-heavy buying comparisons.
Keep those categories separate in the results. Record ordinary-Day scheduling
and check whether apparent policy strength is actually a seat/order advantage.
All comparisons must preserve independent public Days 1–9 and private Day 10.

After development, validate the important comparisons on a fresh declared set
of 300 seeds. Expand a comparison only if uncertainty or a suspected rare failure
justifies it. Report sample sizes, ties and uncertainty; sparse lead bands or
rare endpoint results remain inconclusive. Paired seats from one
seed are related samples, not independent evidence. Save rule/source versions,
policies, seed sets, commands and failing or surprising traces under this folder.
Small focused regressions accompany fixes; run the full suite at the milestone.

Normal's improvement must include situations where leaving an early haven is
worthwhile, low-risk onward travel from an early ordinary space, late-Day score
pressure, exact previews and meaningful purchase trade-offs. Also retain cases
where settling is correct. Do not replace the old bias with forced exploration
or hard-coded minimum finishing positions. Record planning time and work limits
so desktop decision quality does not silently create an unsuitable iPad budget.
Compare wins as well as travel against the old Normal on held-out seeds, and
inspect complete per-duck Night shopping bundles by token type, not just offer
variant counts. Include a case where the best individual purchase blocks a
better two- or three-chip bundle.

## Questions and measures

| Question | Evidence to collect |
| --- | --- |
| Does each Day offer meaningful adventure? | Draw counts, one-draw settlements with their reasons, finishing positions, haven hits, all three regions, wear-outs and progression by Day. Inspect both typical and extreme traces. |
| Can a trailing duck recover? | Leader-to-winner and comeback rates after Days 3, 5, 7 and 9, grouped by Twig deficit and strategy strength. Report lead changes, final margins, Dawn gifts, accumulated Feathers and subsequent starts. Large late leads should be harder to overturn than small early ones; no forced 50/50 target. |
| Does catch-up create unwanted incentives? | Compare safe-haven and Dawn trail growth, recovery after wear-out, repeated gifts and Most Rested advantages. Test deliberate weak early play to see whether farming Dawn gifts becomes more rewarding than playing well. Use matched scenarios to investigate causes; correlation alone does not isolate Dawn's effect. |
| How realistic is the oasis? | Safe and worn-out arrivals at space 43 separately, per duck-Day and per match, first arrival Day, repeat arrivals, associated bag builds and overshoots. Distinguish a legal reachability example from typical observed frequency. Zero arrivals in a sample does not prove impossibility. Review whether it is an attainable late-game ambition, rather than assume every match should reach it. |
| Do different purchases and events matter? | Choice frequency, affordability, unspent Sleep, shopping combinations, movement versus Reeds investment, Companion/Mud interaction, Day 5 Goose pressure, safe-haven value and event-dependent results. Popularity alone is not proof that an item is strong. |
| Does it feel fun and understandable? | Short observed or user-played CLI matches: understandable decisions, tension before drawing, reasons to choose different purchases, recovery after bad luck, engaging late Days and desire to play again. Record feedback separately from simulated outcomes; text-only play cannot validate the finished Unity experience. |

## Decision and completion gate

Deliver a readable baseline-versus-improved comparison, representative game
stories, oasis/comeback distributions and a prioritised issue list. Label each
finding as a confirmed implementation bug, AI limitation, balance concern or
human-play feedback. Record severity, reproduction and confidence, including
questions the available sample cannot answer.

Prefer improving AI planning before attributing its behaviour to prices or
rewards. For any balance proposal, show the problem, exact old/new values or
rule, expected trade-off and supporting comparison. Diagnostic variants stay
separate from the approved profile. Do not silently change the ten-Day length,
43-space route, token powers, shop prices, Dawn thresholds or Feather semantics.
If any of those change, rerun the affected scoring, save-version and starting-
position analysis, including the current effective-start bound of 42.

Close M4.5 only after the report and agreed corrections are checked, known
significant concerns have a reviewed disposition, and the user has reviewed
playability evidence and remaining limits. Missing human feedback remains
explicitly pending; automated wins and passing tests cannot certify fun.
Keep small source/test checkpoints and regular GitHub pushes. M5 then binds
the resulting committed profile and reconciles any approved data changes with
the accepted art/layout. No Unity changes, DLL sync or new artwork occur in
this milestone.
