# Duck art production brief

Updated 12 September 2026. The accepted board, token philosophy and overlay
requirements are defined once in [PLAN.md](PLAN.md). Use that decision record
for visual direction and [STATUS.md](STATUS.md) for the current gate.

The canonical base is [the user-selected exec-9d44cb08 image](concepts/2026-09-12-approved/board-art-approved.png),
with [selection and provenance](concepts/2026-09-12-approved/README.md).
V2 player ducks and the V3 seed are approved; V5 components supply the painted
well/icon style. Earlier layouts and the V1 style test are historical references.
No art generation or Unity import is authorized by this documentation update.

## Production method after work resumes

- Keep the selected scenery separate from placement wells, reward rows, rest
  markers, tokens and legend. Reuse painted components where suitable; author
  exact text and values from verified data. Preserve editable overlay positions
  so alignment can be reviewed against the actual painted route.
- Use the common token size budget from PLAN.md. Check silhouettes, category
  colour, contrast, strength overlays, alpha edges and occupied-well reward
  readability at the actual human-board and opponent-inspection sizes.
- Inspect native outputs before declaring them usable. Record prompts or source,
  version, dimensions, file hash, alpha findings and intended use. A resolution
  requested in a prompt is not evidence of delivered resolution.
- Keep native source files and reproducible crop/overlay metadata outside Unity
  until import is authorized. Preserve `Assets/Art/raw`, catalog references and
  stable `.meta` identities when replacing assets.
- Use original artwork. The user's duck reference inspires a cartoon character;
  do not trace it. Original game assets provide rule/component references, not
  artwork to imitate through image generation.

Possible later import roots, relative to `unity/Quackies.Unity`:

- `Assets/Art/DuckTheme/Backgrounds/`
- `Assets/Art/DuckTheme/Characters/`
- `Assets/Art/DuckTheme/Encounters/`

## Remaining asset work

After authorization, prepare the other encounter categories, resource icons,
scored nest, shelter marker, lily-pad crossing/landing pad and Water flask.
Develop the **Most Rested Duck reward** presentation and **World Events** with
original event art and explanations mapped to existing mechanics. Keep player
duck identities distinct from Companion duck encounters.

Essential journey/result poses belong to the playable presentation. Elaborate
nest-growth stages, decorative variants and animation polish can follow.
Numeric strengths remain overlays. Artwork never silently changes a rule.

The [asset manifest](ASSET_MANIFEST.md) retains historical V1 provenance; each
subsequent concept folder records its own sources. The selected image's
production resolution and exact token fit remain outstanding as listed in PLAN.md.
