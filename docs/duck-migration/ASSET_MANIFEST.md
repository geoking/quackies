# M1 generated asset manifest

Created 10 September 2026 with the built-in **image_gen.imagegen** tool, one
generation per asset. No separately billed API, manual raster editing or original
game-art reference image was used. The three native PNG outputs were copied
unchanged into Unity; the generator's originals remain in the Codex image folder.

The exact prompt set is saved in [m1-image-prompts.json](m1-image-prompts.json).
It specifies happy cartoon wetlands, dark teal outlines, warm cream/orange
characters, simple silhouettes and no baked-in text, numbers or game track.
Native dimensions differ from the suggested prompt sizes and are recorded below.

| Asset | Saved path relative to `unity/Quackies.Unity` | Native output | Use |
| --- | --- | --- | --- |
| Playmat v1 | `Assets/Art/DuckTheme/Backgrounds/duck_playmat_v1.png` | 1536 × 1024 RGB | Quiet mint surface with reeds/pools around the perimeter |
| Happy duck v1 | `Assets/Art/DuckTheme/Characters/duck_happy_v1.png` | 1254 × 1254 RGBA | Starting marker and larger style reference |
| Seeds v1 | `Assets/Art/DuckTheme/Encounters/encounter_seeds_v1.png` | 1254 × 1254 RGBA | Encounter icon with values supplied separately by Unity |

## Inspection and provenance

[asset-inspection.json](evidence/m1/asset-inspection.json) records source paths,
workspace paths, dimensions, SHA-256 hashes and alpha inspection. The duck has
1,009,425 fully transparent pixels; the seeds have 875,980. Both contain partial
alpha along their rendering and boundaries. The playmat is intentionally opaque.
No checkerboard was baked into either character/icon image.

All three were visually inspected at source size: the duck is complete and happy,
the three seeds have a clear silhouette, and the playmat leaves a quiet centre.
Their appearance at actual Unity size has also been inspected in the
[native M1 capture](evidence/m1/duck-style-native.png), with the complete trail,
small encounter overlays and larger samples. [Scene QA](evidence/m1/README.md)
records the checks. The user's style acceptance is still pending.

Generation sources:

- Duck: `exec-88aea203-55b0-417a-8b21-f49045be6ae4.png`
- Playmat: `exec-ae96b09b-14b0-4bf5-bc8f-5b392287392d.png`
- Seeds: `exec-ebd6b1ba-86c4-47b2-a499-9c224e00e24e.png`

These source files are under
`/Users/george/.codex/generated_images/01a08046-4f79-7510-8ec7-8077ec3057b5/`.
Only the copied Unity assets are required by the project. Keep their `.meta`
identities when replacing a version later.

## Scope

Only these three assets were generated. Trail geometry, labels, encounter
strengths and resting highlights are authored in code. Nest, Twigs, Pond pennies
and Feathers use representative placeholder displays in M1. Additional encounter
art, polished nest growth and a shelter-rule experiment are later milestones.
