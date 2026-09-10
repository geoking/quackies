# Initial playable validation — 10 September 2026

## Code and scope

- Unity scene and AI continuation fix: `b2861ad`.
- Successful iOS-export serialization: `a48e0c8`.
- Reusable Editor interaction harness: `da025ec`.
- Complete base game, ingredient Set 1, human versus Normal AI, all 24 fortunes
  enabled by default. Test tubes and the separate AI-history pane are deferred.

## Rules and automated validation

The unchanged Core/CLI solution last passed its build and all 129 tests on
10 September. Tests include focused fortune, final-scoring, state-ownership and
invalid-action cases, 192 single-card complete-match simulations (24 cards × 8
seeds), and 32 mixed-deck complete-match simulations. These are Core results;
they do not validate Unity input or rendering. The rules sweep and documented
interpretations are in [RULES_AUDIT.md](../../../../docs/RULES_AUDIT.md).

## Live Editor match

[Full report](full-match-report.txt): terminal state `complete`, round 9,
phase `Finished`, Human 43 VP / AI 42 VP. Nine distinct fortune captions matched
Core; encountered choice labels preserved their full text without overflow
(longest 31 characters). The settings toggle applied through restart, active
fortune and opponent-pot references opened and returned, dice results displayed
and dismissed, and the final scoreboard opened and returned.

The human actions used EventSystem pointer callbacks and the Core Normal policy;
the opponent used the live presenter coroutine. This caught an AI continuation
bug after a 48-action batch; the corrected run completed. It is callback and
integration evidence, not native mouse hit testing or physical touch evidence.
No new console errors were reported after the validation cursor (4).

## iPad mini presentation

The [viewport check](viewport-check.json) reads the live GameView capture target
(`PlayModeView.m_TargetTexture`) at exactly **1133 × 744**, the same aspect ratio
as 2266 × 1488. The Canvas pixel rect and fitted tabletop match that size; all
four corners of Draw, Stop and Flask lie inside it. The [native initial-scene
capture](initial-scene-native.png) uses that actual target size, with no resizing,
and was visually reviewed for readable controls, fortune artwork and pot layout.

`Screen.width/height` returned 2182 × 1402 while the connector was executing in
Editor GUI context. Inspection of the installed capture command established that
it reads the separate GameView target texture, then scales to the requested output
size. The Screen numbers therefore do not describe this capture's native size.

The [active-fortune table](active-fortune-resized.png) and [final scoreboard](final-scoreboard-resized.png) from the completed match were also visually reviewed.
Their output size is 1133 × 744; the native texture dimensions were not recorded
at those earlier capture instants. Their conservative filenames are retained;
exact-aspect evidence comes from the later native capture and viewport report.

## iOS export

Unity 6000.6.0f1 exported the project successfully at 06:37 UTC to the local
`unity/Quackies.Unity/Builds/iOS` directory: 0 errors and 5 warnings. Only
`Assets/Scenes/QuackiesInitialScene.unity` was included. The generated Xcode
project exists, has `TARGETED_DEVICE_FAMILY = 2` (iPad), an iOS 15.0 deployment
target, and contains one scene (`Data/level0`). Its Info.plist declares landscape
orientation. Generated export files stay ignored in Git. The extracted
[Unity build summary](ios-build-summary.json) preserves the result and all five
warnings: the Pipeline connector is disabled in player builds, one TextMesh Pro
shader pragma is deprecated, and three large TextMesh Pro methods received
separate IL2CPP source files. None prevented export.

Xcode compilation, signing, installation and physical-device testing were not
performed. Export success does not establish device performance or compatibility.

## Acceptance review

The agreed initial playable milestone is complete: the full Set 1 rules and
fortune deck, Normal AI and starting-ruby settings, dice results, fixed controls,
board inspection and final scoring are integrated in the repeatable scene.
Core/CLI checks, the nine-round live Unity run, native iPad-aspect review,
compilation and iOS export have recorded evidence. Test tubes, AI-history UI and
physical-device validation remain future work. No main-branch merge or release
was performed.
