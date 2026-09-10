# Duck art brief

## First experiment: M1, after the M0 review

Generate three separate original raster assets and show them in a separate
Unity scene. The available `image_gen.imagegen` tool is the intended generator;
it is callable in this session, but no generation or quota test is part of M0.
Read the imagegen skill when starting generation. Do not silently use a separate
billed image API. Keep generation bounded to the three assets below.

Happy cartoon wetlands: warm cream duck, orange bill/feet, rounded silhouettes,
clear outlines, soft shading, mint/teal water and fresh green plants. Use a direct
overhead board with a slightly angled character illustration if that makes its
face more expressive. Keep viewpoint, outline weight and light direction
consistent. Exhaustion is sleepy or comically muddy, never distressed.

| Proposed asset | Purpose and composition | Suggested source / background |
| --- | --- | --- |
| `duck_playmat_v1.png` | Calm colourful table/playmat surroundings; quiet central area for the code-built trail and panels | Approximately 2304 × 1536 landscape or nearest supported size; opaque, crop-safe edges |
| `duck_happy_v1.png` | Readable starting-marker duck, facing toward the trail; simple silhouette and visible face | 1024 × 1024 square source; transparent surrounding area |
| `encounter_seeds_v1.png` | One clear cluster of seeds, recognisable inside a small encounter token | 1024 × 1024 square source; transparent surrounding area |

These are source-size targets, not assumptions about generator output support.
Unity's real test viewport is **1133 × 744** (same aspect as 2266 × 1488).
Check icons at actual intended use: roughly 28–36 logical units on the full
trail, with a larger sample beside it. Adjust the layout if the duck and values
cannot be distinguished; do not shorten the complete board to hide the problem.

No generated text, exact track, numbers, token values, rewards, buttons or logos.
Unity constructs the full 0–53 path, labels, markers and buttons. Start with a
6 × 9 winding layout with rounded bends, then refine its spacing in the Editor.
Generated scenery should surround that route, not impose baked-in rule spaces.

The duck remains at the start while representative seed encounters lie ahead.
Show a separate resting-preview highlight and placeholder Twigs, Spend today:
Pond pennies, and Feathers labels. M1 demonstrates style and scale, not working
rule changes. A representative Explore button may illustrate interaction state;
it must not imply the separate style scene is already the full playable game.

## Repository placement and provenance

Proposed Unity import roots, relative to `unity/Quackies.Unity`:

- `Assets/Art/DuckTheme/Backgrounds/`
- `Assets/Art/DuckTheme/Characters/`
- `Assets/Art/DuckTheme/Encounters/`

Store prompts, version, output path, actual dimensions and alpha checks in
`docs/duck-migration/ASSET_MANIFEST.md` when assets are generated. Keep `.meta`
files stable on later replacements. Preserve `Assets/Art/raw` and all current
catalog references. Original game art is reference for rule data only; do not
feed it into the generator to imitate its illustration or board composition.

M1's proposed additions are a `DuckStyleTestSceneBuilder.cs` under
`Assets/Editor` and `Assets/Scenes/DuckStyleTestScene.unity`. Reuse suitable layout
helpers and safe-area fitting. Keep the existing scene builder and build list.
Capture the actual GameView target texture at native dimensions; the connector's
`Screen.width/height` can instead reflect the focused Editor GUI surface.

## Later assets, after the style review

Essential M3/M4 work: obstacle, tailwind, signpost, splash, nesting reeds,
companion duck and wildflower icons; feather, twig and Pond penny icons; base
scored nest; shelter marker; one reusable lily-pad crossing and landing marker;
simple happy/worn-out duck result poses; water flask and Lucky find treatment;
original Pond happenings presentation. Numeric strengths remain UI overlays.

Keep the scored nest distinct from the daily shelter/resting spot. Nest growth
stages, elaborate animations and additional decorative variants are later polish.
Shelter art does not approve or implement a new shelter reward rule.

## M1 review evidence

- Three assets imported and attributed in the manifest; alpha/framing verified.
- Complete winding placeholder trail readable at the native iPad mini aspect.
- Duck, seed, numeric overlay, representative text and control inspected at size.
- Duck/start, placed route, resting preview and persistent nest score distinguishable.
- Unity compiles; existing playable scene and source art remain intact.
- Show the actual Unity screenshot and pause for the user's reaction before M2.
