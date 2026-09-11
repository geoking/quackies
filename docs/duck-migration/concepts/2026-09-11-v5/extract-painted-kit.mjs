#!/usr/bin/env node

// Deterministically remove only the connected neutral-gray checker matte from
// painted-kit-native.png. RGB bytes are copied unchanged into the RGBA output.
import fs from 'node:fs';
import crypto from 'node:crypto';
import path from 'node:path';
import { createRequire } from 'node:module';
const require = createRequire(import.meta.url);
const sharp = require('/Users/george/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/sharp');

const here = path.dirname(new URL(import.meta.url).pathname);
const inputPath = path.join(here, 'painted-kit-native.png');
const outputPath = path.join(here, 'painted-kit.png');
const inspectionPath = path.join(here, 'painted-kit-inspection.json');
const CELL = 418;
const GRID = 3;

// The checker is neutral and gray. Keep this threshold conservative: it
// accepts both observed checker tones while rejecting colored artwork and
// low-value dark ink outlines. Flood-fill connectivity provides the matte
// decision; isolated neutral artwork pixels are therefore retained.
const MATTE_MIN = 115;
const MATTE_MAX = 215;
const MATTE_SPREAD = 18;

function sha256(bytes) {
  return crypto.createHash('sha256').update(bytes).digest('hex');
}

function isMatte(data, index) {
  const r = data[index];
  const g = data[index + 1];
  const b = data[index + 2];
  const min = Math.min(r, g, b);
  const max = Math.max(r, g, b);
  return min >= MATTE_MIN && max <= MATTE_MAX && max - min <= MATTE_SPREAD;
}

function floodMatte(rgb, width, height) {
  const total = width * height;
  const transparent = new Uint8Array(total);
  const queue = new Int32Array(total);
  let head = 0;
  let tail = 0;
  const enqueue = (x, y) => {
    const p = y * width + x;
    if (transparent[p]) return;
    if (!isMatte(rgb, p * 3)) return;
    transparent[p] = 1;
    queue[tail++] = p;
  };
  // Start at every atlas and cell boundary. Cell boundaries are included so
  // each tile's matte can be removed even if an adjacent tile is fully filled.
  for (let y = 0; y < height; y++) {
    for (let x = 0; x < width; x++) {
      if (x === 0 || y === 0 || x === width - 1 || y === height - 1 ||
          x % CELL === 0 || x % CELL === CELL - 1 ||
          y % CELL === 0 || y % CELL === CELL - 1) enqueue(x, y);
    }
  }
  while (head < tail) {
    const p = queue[head++];
    const x = p % width;
    const y = (p - x) / width;
    for (let dy = -1; dy <= 1; dy++) for (let dx = -1; dx <= 1; dx++) {
      if ((dx || dy) && x + dx >= 0 && x + dx < width && y + dy >= 0 && y + dy < height) enqueue(x + dx, y + dy);
    }
  }
  return transparent;
}

function retainLargestRegionComponents(alpha, width, height) {
  const rowEdges = [0, 385, 835, height];
  const colEdges = [0, 418, 836, width];
  const keep = new Uint8Array(width * height);
  const seen = new Uint8Array(width * height);
  const queue = new Int32Array(width * height);
  const regions = [];
  for (let row = 0; row < 3; row++) for (let col = 0; col < 3; col++) {
    const x0 = colEdges[col], x1 = colEdges[col + 1];
    const y0 = rowEdges[row], y1 = rowEdges[row + 1];
    let largest = [];
    let componentCount = 0;
    for (let y = y0; y < y1; y++) for (let x = x0; x < x1; x++) {
      const start = y * width + x;
      if (!alpha[start] || seen[start]) continue;
      componentCount++;
      let head = 0, tail = 0;
      const component = [];
      queue[tail++] = start;
      seen[start] = 1;
      while (head < tail) {
        const p = queue[head++];
        component.push(p);
        const px = p % width;
        const py = (p - px) / width;
        for (let dy = -1; dy <= 1; dy++) for (let dx = -1; dx <= 1; dx++) {
          if (!(dx || dy)) continue;
          const nx = px + dx, ny = py + dy;
          if (nx < x0 || nx >= x1 || ny < y0 || ny >= y1) continue;
          const np = ny * width + nx;
          if (alpha[np] && !seen[np]) { seen[np] = 1; queue[tail++] = np; }
        }
      }
      if (component.length > largest.length) largest = component;
    }
    for (const p of largest) keep[p] = 1;
    regions.push({ grid: `${row},${col}`, componentCount, retainedPixels: largest.length });
  }
  return { keep, regions, rowEdges, colEdges };
}

function componentBounds(alpha, width, x0, y0, regionWidth, regionHeight) {
  let left = regionWidth, top = regionHeight, right = -1, bottom = -1, count = 0;
  for (let y = 0; y < regionHeight; y++) for (let x = 0; x < regionWidth; x++) {
    if (alpha[(y0 + y) * width + x0 + x] === 0) continue;
    count++;
    if (x < left) left = x; if (x > right) right = x;
    if (y < top) top = y; if (y > bottom) bottom = y;
  }
  return { x: x0 + left, y: y0 + top, width: right - left + 1, height: bottom - top + 1, opaquePixels: count };
}

const inputBytes = fs.readFileSync(inputPath);
const { data: rgb, info } = await sharp(inputBytes).raw().toBuffer({ resolveWithObject: true });
if (info.width !== CELL * GRID || info.height !== CELL * GRID || info.channels !== 3) {
  throw new Error(`Expected ${CELL * GRID}x${CELL * GRID} RGB input, got ${info.width}x${info.height}x${info.channels}`);
}
const matte = floodMatte(rgb, info.width, info.height);
const rgba = Buffer.alloc(info.width * info.height * 4);
let transparentPixels = 0;
const alpha = new Uint8Array(info.width * info.height);
for (let p = 0; p < info.width * info.height; p++) {
  alpha[p] = matte[p] ? 0 : 255;
}
const componentCleanup = retainLargestRegionComponents(alpha, info.width, info.height);
for (let p = 0; p < info.width * info.height; p++) {
  alpha[p] = componentCleanup.keep[p] ? 255 : 0;
  rgba[p * 4] = rgb[p * 3];
  rgba[p * 4 + 1] = rgb[p * 3 + 1];
  rgba[p * 4 + 2] = rgb[p * 3 + 2];
  rgba[p * 4 + 3] = alpha[p];
  transparentPixels += alpha[p] === 0;
}
await sharp(rgba, { raw: { width: info.width, height: info.height, channels: 4 } }).png().toFile(outputPath);

const outputBytes = fs.readFileSync(outputPath);
const decoded = await sharp(outputBytes).raw().toBuffer({ resolveWithObject: true });
let alphaZero = 0, alphaFull = 0, rgbMismatches = 0;
for (let p = 0; p < info.width * info.height; p++) {
  const i = p * 4;
  if (decoded.data[i + 3] === 0) alphaZero++;
  if (decoded.data[i + 3] === 255) alphaFull++;
  if (decoded.data[i] !== rgb[p * 3] || decoded.data[i + 1] !== rgb[p * 3 + 1] || decoded.data[i + 2] !== rgb[p * 3 + 2]) rgbMismatches++;
}
const cells = [];
const alphaDecoded = decoded.data.filter((_, i) => i % 4 === 3);
for (let row = 0; row < GRID; row++) for (let col = 0; col < GRID; col++) {
  const x0 = componentCleanup.colEdges[col], x1 = componentCleanup.colEdges[col + 1];
  const y0 = componentCleanup.rowEdges[row], y1 = componentCleanup.rowEdges[row + 1];
  cells.push({ grid: `${row},${col}`, bounds: componentBounds(alphaDecoded, info.width, x0, y0, x1 - x0, y1 - y0) });
}
const retainedCenters = [
  ['creamFace', 210, 140],
  ['whiteFeather', 340, 550],
  ['coin', 210, 1045],
  ['twigs', 627, 1045],
  ['whiteFeatherBottomRight', 1060, 1000]
].map(([name, x, y]) => {
  const i = (y * info.width + x) * 4;
  return { name, x, y, rgb: [...decoded.data.subarray(i, i + 3)], alpha: decoded.data[i + 3], opaque: decoded.data[i + 3] === 255 };
});
const inspection = {
  input: { path: path.basename(inputPath), sha256: sha256(inputBytes), width: info.width, height: info.height, channels: 3 },
  output: { path: path.basename(outputPath), sha256: sha256(outputBytes), width: info.width, height: info.height, channels: 4, cropBounds: { x: 0, y: 0, width: info.width, height: info.height } },
  matteRule: { minChannel: MATTE_MIN, maxChannel: MATTE_MAX, maxRgbSpread: MATTE_SPREAD, connectivity: '8-neighbor from atlas and per-cell boundaries', selectedPixels: transparentPixels },
  componentCleanup: { rowEdges: componentCleanup.rowEdges, colEdges: componentCleanup.colEdges, regions: componentCleanup.regions },
  alpha: { zero: alphaZero, full: alphaFull, other: info.width * info.height - alphaZero - alphaFull, zeroFraction: alphaZero / (info.width * info.height) },
  preservedRgb: { mismatches: rgbMismatches, verified: rgbMismatches === 0 },
  retainedOpaqueCenters: retainedCenters,
  nontransparentComponentsByGrid: cells,
  caveats: ['Checker matte pixels are removed only when boundary-connected under the neutral-gray threshold.', 'No redraw, resampling, recoloring, or RGB alteration was performed.', 'Output retains the full atlas crop; component bounds are atlas coordinates.']
};
fs.writeFileSync(inspectionPath, `${JSON.stringify(inspection, null, 2)}\n`);
console.log(JSON.stringify(inspection, null, 2));
