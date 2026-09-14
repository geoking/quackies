"""Measure native Sprite Editor polygons; never alter the generated PNG pixels.

The generated sheets have neutral checkerboard outside the coloured tile art.
Read colour/dark-outline pixels in each of the five fixed sheet cells and trace
their radial silhouette, as used by the earlier decorative-tile crop manifests.
Requires Pillow and NumPy. Run from the repository root after copying the sheets.
"""
import json
from pathlib import Path

import numpy as np
from PIL import Image, ImageDraw

ART = Path("unity/Quackies.Unity/Assets/Art/DuckLayout")


def measure_sheet(path):
    image = Image.open(path)
    assert image.size == (1536, 1024), (path, image.size)
    rgb = np.asarray(image.convert("RGB"), dtype=np.int16)
    colour = rgb.max(axis=2) - rgb.min(axis=2) > 18
    dark_edge = rgb.max(axis=2) < 128
    foreground = colour | dark_edge
    entries = []
    for index, (column, row) in enumerate([(0, 0), (1, 0), (2, 0), (0, 1), (1, 1)]):
        cell_x, cell_y = column * 512, (0 if row == 0 else 490)
        cell = foreground[cell_y:(490 if row == 0 else 1024), cell_x:cell_x + 512]
        ys, xs = np.nonzero(cell)
        x, y = int(xs.min()), int(ys.min())
        width, height = int(xs.max()) - x + 1, int(ys.max()) - y + 1
        assert 430 < width < 510 and 350 < height < 490, (path, index, width, height)
        mask = cell[y:y + height, x:x + width]
        cx, cy = (width - 1) / 2, (height - 1) / 2
        radii = np.arange(0, np.hypot(width, height), .25)
        polygon = []
        for angle in np.linspace(0, 2 * np.pi, 360, endpoint=False):
            dx, dy = np.cos(angle), np.sin(angle)
            ray_x, ray_y = np.rint(cx + dx * radii).astype(int), np.rint(cy + dy * radii).astype(int)
            inside = (ray_x >= 0) & (ray_x < width) & (ray_y >= 0) & (ray_y < height)
            hits = np.flatnonzero(mask[ray_y[inside], ray_x[inside]])
            assert len(hits), (path, index, angle)
            radius = radii[inside][hits[-1]] + 1
            polygon.append((round(float(np.clip(cx + dx * radius, 0, width - 1)), 2),
                            round(float(np.clip(cy + dy * radius, 0, height - 1)), 2)))
        # An in-memory diagnostic mask validates the polygon; no raster file is written.
        coverage = Image.new("1", (width, height))
        ImageDraw.Draw(coverage).polygon(polygon, fill=1)
        covered = np.asarray(coverage)
        fraction = float(np.count_nonzero(covered & mask) / np.count_nonzero(mask))
        assert fraction > .99, (path, index, fraction)
        entries.append(dict(asset=path.name, index=index, x=cell_x + x, y=cell_y + y,
                            width=width, height=height,
                            outline=[dict(x=px, y=py) for px, py in polygon]))
        print(path.name, index, (cell_x + x, cell_y + y, width, height), "coverage", round(fraction, 5))
    return entries


if __name__ == "__main__":
    entries = []
    for biome in ["wetlands", "meadow", "wasteland"]:
        entries.extend(measure_sheet(ART / ("tile-scattered-" + biome + ".png")))
    (ART / "tile-crops.json").write_text(json.dumps(dict(entries=entries), indent=2) + "\n")
