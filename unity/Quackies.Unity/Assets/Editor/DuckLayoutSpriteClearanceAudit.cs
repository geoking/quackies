using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quackies.Unity.DuckLayout;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackies.Unity.Editor
{
    /// <summary>
    /// Read-only clearance audit for the real cropped encounter meshes. It deliberately does not
    /// use the token RectTransform as a collision shape: transparent sprite area is not artwork.
    /// </summary>
    public static class DuckLayoutSpriteClearanceAudit
    {
        private const float BoardPixelClearance = .75f;

        public static string AuditOpenScene()
        {
            Canvas.ForceUpdateCanvases();
            var issues = new List<string>();
            var controller = UnityEngine.Object.FindObjectOfType<DuckLayoutProofPreview>();
            if (controller == null)
            {
                issues.Add("Missing DuckLayoutProofPreview controller.");
                return Report(0, 0, 0, issues);
            }

            var fixtures = (controller.TokenPresentations ?? Array.Empty<DuckLayoutProofPreview.TokenPresentation>())
                .Where(token => token != null && token.sprite != null).ToArray();
            if (fixtures.Length != 16 || fixtures.Select(token => token.sprite).Distinct().Count() != 16)
                issues.Add("Expected 16 distinct non-null encounter fixture sprites.");

            var spaces = (controller.Spaces ?? Array.Empty<DuckLayoutSpaceView>())
                .Where(space => space != null).ToArray();
            var placements = new List<TokenPlacement>();
            foreach (var space in spaces.Where(space => space.Space < controller.BoardSpaceCount))
            {
                var tokenImage = space.GetComponentsInChildren<Image>(true)
                    .FirstOrDefault(image => image.name == "Encounter overlay");
                if (tokenImage == null)
                {
                    issues.Add(SpaceName(space) + " is missing its encounter token image.");
                    continue;
                }
                if (tokenImage.type != Image.Type.Simple || !tokenImage.preserveAspect)
                    issues.Add(SpaceName(space) + " encounter token must use Simple Image with preserveAspect enabled.");
                RequireNativeMeshWhenActive(tokenImage, SpaceName(space) + " encounter token", issues);
                placements.Add(new TokenPlacement(space, tokenImage));
            }

            if (placements.Count != controller.BoardSpaceCount - 1)
                issues.Add("Expected " + (controller.BoardSpaceCount - 1) + " nonendpoint token placements, found " + placements.Count + ".");
            if (fixtures.Length != 16 || placements.Count == 0)
                return Report(placements.Count, 0, 0, issues);

            var clearance = WorldClearance(placements[0].image);
            var rewards = RewardTargets(spaces);
            foreach (var placement in placements)
                placement.SetUnionBounds(fixtures.Select(fixture => fixture.sprite));

            var rewardMeshCandidates = 0;
            var chipMeshCandidates = 0;
            var duckMeshCandidates = 0;
            var featherMeshCandidates = 0;

            foreach (var placement in placements)
            {
                foreach (var reward in rewards)
                {
                    if (!Overlaps(placement.unionBounds, Expand(reward.bounds, clearance))) continue;
                    foreach (var fixture in fixtures)
                    {
                        var mesh = placement.MeshFor(fixture.sprite);
                        rewardMeshCandidates++;
                        if (Intersects(mesh, reward.bounds, clearance))
                            issues.Add(SpaceName(placement.space) + " / " + fixture.title + " chip overlaps " + reward.description + ".");
                    }
                }
            }

            // The union bounds discard distant route pairs before comparing the real mesh triangles.
            // Each ordered fixture pair is considered because their cropped shapes differ by location.
            for (var left = 0; left < placements.Count; left++)
            {
                for (var right = left + 1; right < placements.Count; right++)
                {
                    var first = placements[left];
                    var second = placements[right];
                    if (!Overlaps(first.unionBounds, Expand(second.unionBounds, clearance))) continue;
                    for (var a = 0; a < fixtures.Length; a++)
                    {
                        var firstMesh = first.MeshFor(fixtures[a].sprite);
                        for (var b = 0; b < fixtures.Length; b++)
                        {
                            var secondMesh = second.MeshFor(fixtures[b].sprite);
                            chipMeshCandidates++;
                            if (Intersects(firstMesh, secondMesh, clearance))
                            {
                                issues.Add(SpaceName(first.space) + " / " + fixtures[a].title + " chip overlaps "
                                    + SpaceName(second.space) + " / " + fixtures[b].title + " chip.");
                            }
                        }
                    }
                }
            }

            var duck = UnityEngine.Object.FindObjectsOfType<Image>(true)
                .FirstOrDefault(image => image.name == "Duck resting at space 32");
            var duckSprite = ActiveSprite(duck);
            if (duck == null || duckSprite == null)
            {
                issues.Add("Missing a sprite-backed Duck resting at space 32.");
            }
            else
            {
                RequireNativeMeshWhenActive(duck, "Duck 32", issues);
                var duckMesh = SimulatedSpriteMesh.Create(duck, duckSprite);
                foreach (var reward in rewards)
                {
                    if (!Overlaps(duckMesh.bounds, Expand(reward.bounds, clearance))) continue;
                    duckMeshCandidates++;
                    if (Intersects(duckMesh, reward.bounds, clearance))
                        issues.Add("Duck 32 overlaps " + reward.description + ".");
                }
                foreach (var placement in placements.Where(placement => placement.space.Space != 32))
                {
                    if (!Overlaps(duckMesh.bounds, Expand(placement.unionBounds, clearance))) continue;
                    foreach (var fixture in fixtures)
                    {
                        var chipMesh = placement.MeshFor(fixture.sprite);
                        duckMeshCandidates++;
                        if (Intersects(duckMesh, chipMesh, clearance))
                            issues.Add("Duck 32 overlaps " + SpaceName(placement.space) + " / " + fixture.title + " chip.");
                    }
                }
            }

            var oasisFeathers = UnityEngine.Object.FindObjectsOfType<Image>(true)
                .Where(image => image.name.StartsWith("Oasis Ground Feather ", StringComparison.Ordinal) && ActiveSprite(image) != null).ToArray();
            if (oasisFeathers.Length != 2)
                issues.Add("Expected two sprite-backed Oasis Ground Feather images, found " + oasisFeathers.Length + ".");
            foreach (var feather in oasisFeathers)
            {
                RequireNativeMeshWhenActive(feather, feather.name, issues);
                var featherMesh = SimulatedSpriteMesh.Create(feather, ActiveSprite(feather));
                foreach (var reward in rewards)
                {
                    if (!Overlaps(featherMesh.bounds, Expand(reward.bounds, clearance))) continue;
                    featherMeshCandidates++;
                    if (Intersects(featherMesh, reward.bounds, clearance))
                        issues.Add(feather.name + " overlaps " + reward.description + ".");
                }
                foreach (var placement in placements)
                {
                    if (!Overlaps(featherMesh.bounds, Expand(placement.unionBounds, clearance))) continue;
                    foreach (var fixture in fixtures)
                    {
                        var chipMesh = placement.MeshFor(fixture.sprite);
                        featherMeshCandidates++;
                        if (Intersects(featherMesh, chipMesh, clearance))
                            issues.Add(feather.name + " overlaps " + SpaceName(placement.space) + " / " + fixture.title + " chip.");
                    }
                }
            }

            return Report(placements.Count, rewardMeshCandidates,
                chipMeshCandidates + duckMeshCandidates + featherMeshCandidates, issues);
        }

        /// <summary>Checks that the audit's simulated native mesh still matches the mesh CanvasRenderer drew for active artwork.</summary>
        public static string VerifyRenderedMeshes()
        {
            Canvas.ForceUpdateCanvases();
            var issues = new List<string>();
            var checkedMeshes = 0;
            var images = UnityEngine.Object.FindObjectsOfType<Image>(true);
            foreach (var encounter in images.Where(image => image.name == "Encounter overlay" && IsActiveSpriteBacked(image)))
                VerifyRenderedMesh(encounter, "Encounter overlay", issues, ref checkedMeshes);
            foreach (var duck in images.Where(image => image.name == "Duck resting at space 32" && IsActiveSpriteBacked(image)))
                VerifyRenderedMesh(duck, "Duck 32", issues, ref checkedMeshes);
            foreach (var feather in images.Where(image => image.name.StartsWith("Oasis Ground Feather ", StringComparison.Ordinal)
                && IsActiveSpriteBacked(image)))
                VerifyRenderedMesh(feather, feather.name, issues, ref checkedMeshes);

            var report = new StringBuilder();
            report.AppendLine("# Duck Layout Rendered Mesh Verification");
            report.AppendLine("Status: " + (issues.Count == 0 ? "PASS" : "FAIL"));
            report.AppendLine("Checked active native meshes: " + checkedMeshes + ".");
            foreach (var issue in issues.Distinct()) report.AppendLine("- " + issue);
            return report.ToString();
        }

        private static void VerifyRenderedMesh(Image image, string description, List<string> issues, ref int checkedMeshes)
        {
            var sprite = ActiveSprite(image);
            if (image.type != Image.Type.Simple || !image.useSpriteMesh || sprite == null)
            {
                issues.Add(description + " is not an active Simple native-sprite Image.");
                return;
            }
            var rendered = image.canvasRenderer == null ? null : image.canvasRenderer.GetMesh();
            if (rendered == null)
            {
                issues.Add(description + " has no CanvasRenderer mesh.");
                return;
            }
            checkedMeshes++;
            var simulated = SimulatedSpriteMesh.Create(image, sprite);
            var actualVertices = rendered.vertices;
            if (actualVertices.Length != simulated.vertices.Length)
            {
                issues.Add(description + " mesh vertex count differs: " + actualVertices.Length + " rendered, "
                    + simulated.vertices.Length + " simulated.");
            }
            else
            {
                for (var index = 0; index < actualVertices.Length; index++)
                {
                    var renderedWorld = image.rectTransform.TransformPoint(actualVertices[index]);
                    if (Vector2.Distance(renderedWorld, simulated.vertices[index]) <= .01f) continue;
                    issues.Add(description + " mesh vertex " + index + " differs from Image.GenerateSprite geometry.");
                    break;
                }
            }
            var actualTriangles = rendered.triangles;
            if (actualTriangles.Length != simulated.triangles.Length)
            {
                issues.Add(description + " mesh triangle-index count differs: " + actualTriangles.Length + " rendered, "
                    + simulated.triangles.Length + " simulated.");
                return;
            }
            for (var index = 0; index < actualTriangles.Length; index++)
            {
                if (actualTriangles[index] == simulated.triangles[index]) continue;
                issues.Add(description + " mesh triangle index " + index + " differs from native sprite triangles.");
                break;
            }
        }

        private static List<RewardTarget> RewardTargets(IEnumerable<DuckLayoutSpaceView> spaces)
        {
            var targets = new List<RewardTarget>();
            foreach (var space in spaces)
            {
                foreach (var number in space.RewardNumbers)
                {
                    if (number == null) continue;
                    foreach (var glyph in GlyphRects(number))
                    {
                        targets.Add(new RewardTarget(glyph, SpaceName(space) + " / " + number.name + " glyph"));
                    }
                }
                foreach (var area in space.RewardArtAreas)
                {
                    if (area != null)
                        targets.Add(new RewardTarget(WorldRect(area), SpaceName(space) + " / " + area.name));
                }
            }
            return targets;
        }

        private static IEnumerable<Rect> GlyphRects(TMP_Text text)
        {
            text.ForceMeshUpdate(true, true);
            foreach (var character in text.textInfo.characterInfo.Take(text.textInfo.characterCount))
            {
                if (!character.isVisible) continue;
                var bottomLeft = text.rectTransform.TransformPoint(character.bottomLeft);
                var bottomRight = text.rectTransform.TransformPoint(character.bottomRight);
                var topLeft = text.rectTransform.TransformPoint(character.topLeft);
                var topRight = text.rectTransform.TransformPoint(character.topRight);
                var glyphBounds = Rect.MinMaxRect(
                    Mathf.Min(bottomLeft.x, bottomRight.x, topLeft.x, topRight.x),
                    Mathf.Min(bottomLeft.y, bottomRight.y, topLeft.y, topRight.y),
                    Mathf.Max(bottomLeft.x, bottomRight.x, topLeft.x, topRight.x),
                    Mathf.Max(bottomLeft.y, bottomRight.y, topLeft.y, topRight.y));
                yield return Expand(glyphBounds, .65f * Mathf.Abs(text.rectTransform.lossyScale.x));
            }
        }

        private static float WorldClearance(Image tokenImage)
        {
            var board = UnityEngine.Object.FindObjectsOfType<Image>(true)
                .FirstOrDefault(image => image.name == "Approved Board Art" && image.sprite != null);
            if (board == null) return BoardPixelClearance * Mathf.Max(tokenImage.rectTransform.lossyScale.x, tokenImage.rectTransform.lossyScale.y);
            var boardBounds = WorldRect(board.rectTransform);
            var boardPixels = board.sprite.rect.size;
            if (boardPixels.x <= 0f || boardPixels.y <= 0f) return BoardPixelClearance;
            return BoardPixelClearance * Mathf.Max(boardBounds.width / boardPixels.x, boardBounds.height / boardPixels.y);
        }

        private static string Report(int placements, int rewardMeshCandidates, int chipMeshCandidates, IEnumerable<string> issues)
        {
            var allIssues = issues.Distinct().ToArray();
            var report = new StringBuilder();
            report.AppendLine("# Duck Layout Sprite Clearance Audit");
            report.AppendLine("Status: " + (allIssues.Length == 0 ? "PASS" : "FAIL"));
            report.AppendLine("Simulated placements: 16 × " + placements + " = " + (16 * placements) + ".");
            report.AppendLine("Broad-phase candidates sent to exact triangle SAT: " + rewardMeshCandidates + " reward meshes; "
                + chipMeshCandidates + " chip/duck/feather meshes.");
            if (allIssues.Length > 0)
            {
                report.AppendLine("Issues (" + allIssues.Length + "):");
                foreach (var issue in allIssues) report.AppendLine("- " + issue);
            }
            return report.ToString();
        }

        private static string SpaceName(DuckLayoutSpaceView space) => space.StableId + " (space " + space.Space + ")";

        private static Sprite ActiveSprite(Image image) => image == null ? null : image.overrideSprite;

        private static bool IsActiveSpriteBacked(Image image) => image != null && image.isActiveAndEnabled && ActiveSprite(image) != null;

        private static void RequireNativeMeshWhenActive(Image image, string description, List<string> issues)
        {
            if (IsActiveSpriteBacked(image) && !image.useSpriteMesh)
                issues.Add(description + " is active with a sprite but useSpriteMesh is disabled.");
        }

        private static Rect WorldRect(RectTransform transform)
        {
            var corners = new Vector3[4];
            transform.GetWorldCorners(corners);
            return Rect.MinMaxRect(corners.Min(corner => corner.x), corners.Min(corner => corner.y),
                corners.Max(corner => corner.x), corners.Max(corner => corner.y));
        }

        private static Rect Expand(Rect rect, float amount) => Rect.MinMaxRect(rect.xMin - amount, rect.yMin - amount,
            rect.xMax + amount, rect.yMax + amount);

        private static bool Overlaps(Rect first, Rect second) => first.xMin <= second.xMax && first.xMax >= second.xMin
            && first.yMin <= second.yMax && first.yMax >= second.yMin;

        private static bool Intersects(SimulatedSpriteMesh mesh, Rect rect, float clearance)
        {
            var expanded = Expand(rect, clearance);
            if (!Overlaps(mesh.bounds, expanded)) return false;
            for (var index = 0; index < mesh.triangles.Length; index += 3)
            {
                if (TriangleIntersectsRect(mesh.vertices[mesh.triangles[index]], mesh.vertices[mesh.triangles[index + 1]],
                    mesh.vertices[mesh.triangles[index + 2]], expanded)) return true;
            }
            return false;
        }

        private static bool Intersects(SimulatedSpriteMesh first, SimulatedSpriteMesh second, float clearance)
        {
            if (!Overlaps(first.bounds, Expand(second.bounds, clearance))) return false;
            for (var a = 0; a < first.triangles.Length; a += 3)
            {
                var firstA = first.vertices[first.triangles[a]];
                var firstB = first.vertices[first.triangles[a + 1]];
                var firstC = first.vertices[first.triangles[a + 2]];
                var firstBounds = Rect.MinMaxRect(Mathf.Min(firstA.x, firstB.x, firstC.x), Mathf.Min(firstA.y, firstB.y, firstC.y),
                    Mathf.Max(firstA.x, firstB.x, firstC.x), Mathf.Max(firstA.y, firstB.y, firstC.y));
                for (var b = 0; b < second.triangles.Length; b += 3)
                {
                    var secondA = second.vertices[second.triangles[b]];
                    var secondB = second.vertices[second.triangles[b + 1]];
                    var secondC = second.vertices[second.triangles[b + 2]];
                    if (!Overlaps(firstBounds, Expand(Rect.MinMaxRect(Mathf.Min(secondA.x, secondB.x, secondC.x),
                        Mathf.Min(secondA.y, secondB.y, secondC.y), Mathf.Max(secondA.x, secondB.x, secondC.x),
                        Mathf.Max(secondA.y, secondB.y, secondC.y)), clearance))) continue;
                    if (TrianglesIntersect(firstA, firstB, firstC, secondA, secondB, secondC, clearance)) return true;
                }
            }
            return false;
        }

        private static bool TriangleIntersectsRect(Vector2 a, Vector2 b, Vector2 c, Rect rect)
        {
            return OverlapsOnAxis(a, b, c, rect, Vector2.right)
                && OverlapsOnAxis(a, b, c, rect, Vector2.up)
                && OverlapsOnAxis(a, b, c, rect, Perpendicular(b - a))
                && OverlapsOnAxis(a, b, c, rect, Perpendicular(c - b))
                && OverlapsOnAxis(a, b, c, rect, Perpendicular(a - c));
        }

        private static bool TrianglesIntersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d, Vector2 e, Vector2 f, float clearance)
        {
            var axes = new[] { Perpendicular(b - a), Perpendicular(c - b), Perpendicular(a - c),
                Perpendicular(e - d), Perpendicular(f - e), Perpendicular(d - f) };
            foreach (var axis in axes)
            {
                if (axis.sqrMagnitude < .0001f) continue;
                Project(a, b, c, axis, out var firstMin, out var firstMax);
                Project(d, e, f, axis, out var secondMin, out var secondMax);
                var axisLength = Mathf.Sqrt(axis.sqrMagnitude);
                if (firstMax + clearance * axisLength < secondMin || secondMax + clearance * axisLength < firstMin) return false;
            }
            return true;
        }

        private static bool OverlapsOnAxis(Vector2 a, Vector2 b, Vector2 c, Rect rect, Vector2 axis)
        {
            if (axis.sqrMagnitude < .0001f) return true;
            Project(a, b, c, axis, out var triangleMin, out var triangleMax);
            var rectangleMin = Mathf.Min(Vector2.Dot(new Vector2(rect.xMin, rect.yMin), axis), Vector2.Dot(new Vector2(rect.xMin, rect.yMax), axis),
                Vector2.Dot(new Vector2(rect.xMax, rect.yMin), axis), Vector2.Dot(new Vector2(rect.xMax, rect.yMax), axis));
            var rectangleMax = Mathf.Max(Vector2.Dot(new Vector2(rect.xMin, rect.yMin), axis), Vector2.Dot(new Vector2(rect.xMin, rect.yMax), axis),
                Vector2.Dot(new Vector2(rect.xMax, rect.yMin), axis), Vector2.Dot(new Vector2(rect.xMax, rect.yMax), axis));
            return triangleMax >= rectangleMin && rectangleMax >= triangleMin;
        }

        private static void Project(Vector2 a, Vector2 b, Vector2 c, Vector2 axis, out float min, out float max)
        {
            min = max = Vector2.Dot(a, axis);
            var bProjection = Vector2.Dot(b, axis);
            var cProjection = Vector2.Dot(c, axis);
            min = Mathf.Min(min, bProjection, cProjection);
            max = Mathf.Max(max, bProjection, cProjection);
        }

        private static Vector2 Perpendicular(Vector2 vector) => new Vector2(-vector.y, vector.x);

        private sealed class TokenPlacement
        {
            private readonly Dictionary<Sprite, SimulatedSpriteMesh> meshes = new Dictionary<Sprite, SimulatedSpriteMesh>();
            public readonly DuckLayoutSpaceView space;
            public readonly Image image;
            public Rect unionBounds;

            public TokenPlacement(DuckLayoutSpaceView spaceView, Image tokenImage)
            {
                space = spaceView;
                image = tokenImage;
            }

            public void SetUnionBounds(IEnumerable<Sprite> sprites)
            {
                var hasBounds = false;
                foreach (var sprite in sprites)
                {
                    var bounds = SimulatedSpriteMesh.BoundsFor(image, sprite);
                    unionBounds = hasBounds ? Union(unionBounds, bounds) : bounds;
                    hasBounds = true;
                }
            }

            public SimulatedSpriteMesh MeshFor(Sprite sprite)
            {
                if (!meshes.TryGetValue(sprite, out var mesh))
                {
                    mesh = SimulatedSpriteMesh.Create(image, sprite);
                    meshes.Add(sprite, mesh);
                }
                return mesh;
            }
        }

        private readonly struct RewardTarget
        {
            public readonly Rect bounds;
            public readonly string description;
            public RewardTarget(Rect targetBounds, string targetDescription) { bounds = targetBounds; description = targetDescription; }
        }

        private readonly struct SimulatedSpriteMesh
        {
            public readonly Vector2[] vertices;
            public readonly ushort[] triangles;
            public readonly Rect bounds;

            private SimulatedSpriteMesh(Vector2[] worldVertices, ushort[] spriteTriangles)
            {
                vertices = worldVertices;
                triangles = spriteTriangles;
                bounds = Rect.MinMaxRect(worldVertices.Min(point => point.x), worldVertices.Min(point => point.y),
                    worldVertices.Max(point => point.x), worldVertices.Max(point => point.y));
            }

            public static Rect BoundsFor(Image image, Sprite sprite) => CreateVertices(image, sprite, false).bounds;

            public static SimulatedSpriteMesh Create(Image image, Sprite sprite)
            {
                var mapped = CreateVertices(image, sprite, true);
                return new SimulatedSpriteMesh(mapped.vertices, sprite.triangles);
            }

            private static (Vector2[] vertices, Rect bounds) CreateVertices(Image image, Sprite sprite, bool retainVertices)
            {
                if (image == null || sprite == null || sprite.vertices == null || sprite.vertices.Length == 0)
                    throw new InvalidOperationException("Sprite clearance audit needs a non-empty image sprite mesh.");
                if (sprite.triangles == null || sprite.triangles.Length == 0 || sprite.triangles.Length % 3 != 0)
                    throw new InvalidOperationException("Sprite clearance audit needs complete sprite triangles.");
                var spriteSize = sprite.rect.size;
                var spriteBounds = sprite.bounds.size;
                if (spriteSize.x <= 0f || spriteSize.y <= 0f || spriteBounds.x == 0f || spriteBounds.y == 0f)
                    throw new InvalidOperationException("Sprite clearance audit found an invalid sprite size.");
                // This is Image.GenerateSprite's native-mesh path: its actual pixel-adjusted rect,
                // optional preserved aspect ratio, and the sprite-pivot / RectTransform-pivot offset.
                var rect = image.GetPixelAdjustedRect();
                if (image.preserveAspect) PreserveSpriteAspectRatio(ref rect, spriteSize, image.rectTransform.pivot);
                var drawSize = rect.size;
                var drawOffset = (image.rectTransform.pivot - sprite.pivot / spriteSize) * drawSize;
                var vertices = retainVertices ? new Vector2[sprite.vertices.Length] : null;
                var bounds = new Rect();
                for (var index = 0; index < sprite.vertices.Length; index++)
                {
                    var vertex = sprite.vertices[index];
                    var local = new Vector3((vertex.x / spriteBounds.x) * drawSize.x - drawOffset.x,
                        (vertex.y / spriteBounds.y) * drawSize.y - drawOffset.y);
                    var world = image.rectTransform.TransformPoint(local);
                    if (retainVertices) vertices[index] = world;
                    if (index == 0) bounds = Rect.MinMaxRect(world.x, world.y, world.x, world.y);
                    else bounds = Union(bounds, Rect.MinMaxRect(world.x, world.y, world.x, world.y));
                }
                return (vertices, bounds);
            }
        }

        private static void PreserveSpriteAspectRatio(ref Rect rect, Vector2 spriteSize, Vector2 rectPivot)
        {
            var spriteRatio = spriteSize.x / spriteSize.y;
            var rectRatio = rect.width / rect.height;
            if (spriteRatio > rectRatio)
            {
                var oldHeight = rect.height;
                rect.height = rect.width / spriteRatio;
                rect.y += (oldHeight - rect.height) * rectPivot.y;
            }
            else
            {
                var oldWidth = rect.width;
                rect.width = rect.height * spriteRatio;
                rect.x += (oldWidth - rect.width) * rectPivot.x;
            }
        }

        private static Rect Union(Rect first, Rect second) => Rect.MinMaxRect(Mathf.Min(first.xMin, second.xMin), Mathf.Min(first.yMin, second.yMin),
            Mathf.Max(first.xMax, second.xMax), Mathf.Max(first.yMax, second.yMax));
    }
}
