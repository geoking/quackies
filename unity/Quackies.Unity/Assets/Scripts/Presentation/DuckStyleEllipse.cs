using UnityEngine;
using UnityEngine.UI;

namespace Quackies.Unity.Presentation
{
    /// <summary>Draws a clean, scale-independent filled ellipse for the M1 trail geometry.</summary>
    public sealed class DuckStyleEllipse : Image
    {
        [SerializeField] private Color borderColor = new Color(.04f, .29f, .31f, .55f);
        [SerializeField] [Min(0f)] private float borderThickness = 1.5f;

        public void Configure(Color fill, Color border, float thickness)
        {
            color = fill;
            borderColor = border;
            borderThickness = Mathf.Max(0f, thickness);
            SetVerticesDirty();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetAllDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vertices)
        {
            vertices.Clear();
            var rect = GetPixelAdjustedRect();
            var halfWidth = rect.width * .5f;
            var halfHeight = rect.height * .5f;
            if (halfWidth <= 0f || halfHeight <= 0f) return;

            var centre = rect.center;

            const int segments = 40;
            var innerWidth = Mathf.Max(0f, halfWidth - borderThickness);
            var innerHeight = Mathf.Max(0f, halfHeight - borderThickness);
            vertices.AddVert(centre, color, Vector2.zero);

            for (var index = 0; index < segments; index++)
            {
                var radians = Mathf.PI * 2f * index / segments;
                var direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
                vertices.AddVert(centre + Vector2.Scale(direction, new Vector2(innerWidth, innerHeight)), color, Vector2.zero);
                vertices.AddVert(centre + Vector2.Scale(direction, new Vector2(halfWidth, halfHeight)), borderColor, Vector2.zero);
            }

            for (var index = 0; index < segments; index++)
            {
                var next = (index + 1) % segments;
                var inner = 1 + index * 2;
                var outer = inner + 1;
                var nextInner = 1 + next * 2;
                var nextOuter = nextInner + 1;
                vertices.AddTriangle(0, inner, nextInner);
                vertices.AddTriangle(inner, outer, nextInner);
                vertices.AddTriangle(outer, nextOuter, nextInner);
            }
        }
    }
}
