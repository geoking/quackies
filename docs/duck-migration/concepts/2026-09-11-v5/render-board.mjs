#!/usr/bin/env node
/** Static V5 board typesetter: bitmap-painted pads/icons, SVG text only. */
import { readFile, writeFile } from 'node:fs/promises';
import { existsSync } from 'node:fs';
import { dirname, extname, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';
import { createRequire } from 'node:module';

const require = createRequire(import.meta.url);
const sharp = (() => { try { return require('sharp'); } catch { return require('/Users/george/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/sharp'); } })();
const here = dirname(fileURLToPath(import.meta.url));
const args = process.argv.slice(2);
const option = (name, fallback) => { const index = args.indexOf(name); return index === -1 ? fallback : args[index + 1]; };
const checkOnly = args.includes('--check');
const backgroundPath = resolve(here, option('--background', 'board-art-v5.png'));
const outputPath = resolve(here, option('--output', 'three-biome-board-v5.jpg'));
const qaPath = resolve(here, option('--qa', 'typeset-qa.json'));
const outputExtension = extname(outputPath).toLowerCase();
const outputFormat = outputExtension === '.jpg' || outputExtension === '.jpeg' ? 'jpeg' : outputExtension === '.png' ? 'png' : null;
if (!outputFormat) throw new Error(`Unsupported output extension "${outputExtension}".`);

const [layout, track] = await Promise.all([
  readFile(resolve(here, 'layout.json'), 'utf8').then(JSON.parse),
  readFile(resolve(here, 'track-data.json'), 'utf8').then(JSON.parse)
]);
const atlasMetadataPath = resolve(here, layout.atlas.metadata);
if (!existsSync(atlasMetadataPath)) throw new Error(`Tile atlas metadata not found: ${atlasMetadataPath}`);
const atlasMetadata = JSON.parse(await readFile(atlasMetadataPath, 'utf8'));
const atlasPath = resolve(here, atlasMetadata.atlas ?? layout.atlas.file);
const requiredRests = [5, 13, 20, 28, 34, 40, 46, 52];
const scale = layout.renderCanvas.width / layout.referenceCanvas.width;
const escape = (value) => String(value).replace(/[&<>"']/g, (char) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&apos;' })[char]);
const rect = (x, y, width, height) => ({ x: x - width / 2, y: y - height / 2, width, height });
const overlaps = (a, b) => a.x < b.x + b.width && a.x + a.width > b.x && a.y < b.y + b.height && a.y + a.height > b.y;
const within = (box, area) => box.x >= area.x && box.y >= area.y && box.x + box.width <= area.x + area.width && box.y + box.height <= area.y + area.height;
const cropNames = ['regularA', 'regularB', 'regularC', 'restA', 'restB', 'restC', 'coin', 'twig', 'restIcon'];
const rewardFont = 'Marker Felt, Chalkboard SE, Comic Sans MS, sans-serif';
const rewardFontSize = 11;
const rewardIcon = { coin: { width: 9, height: 9 }, twig: { width: 11, height: 9.6 } };
const capsuleInset = { horizontal: 6, vertical: 3 };
async function measureGlyph(value) {
  const origin = { x: 40, y: 36 };
  const source = `<svg width="120" height="70" xmlns="http://www.w3.org/2000/svg"><text x="${origin.x}" y="${origin.y}" style="font-family:${rewardFont};font-size:${rewardFontSize}px;font-weight:800;fill:#593d26">${escape(value)}</text></svg>`;
  const { data, info } = await sharp(Buffer.from(source)).ensureAlpha().raw().toBuffer({ resolveWithObject: true });
  let minX = info.width; let minY = info.height; let maxX = -1; let maxY = -1;
  for (let y = 0; y < info.height; y += 1) for (let x = 0; x < info.width; x += 1) if (data[(y * info.width + x) * info.channels + 3] > 0) { minX = Math.min(minX, x); minY = Math.min(minY, y); maxX = Math.max(maxX, x); maxY = Math.max(maxY, y); }
  if (maxX < 0) throw new Error(`Could not measure reward glyphs for ${value}.`);
  return { value: String(value), width: maxX - minX + 1, height: maxY - minY + 1, leftBearing: minX - origin.x, topBearing: minY - origin.y };
}
const rewardValues = [...new Set(track.spaces.filter((space) => space.index >= 1).flatMap((space) => [space.coins, ...(space.points === 0 ? [] : [space.points])]).map(String))];
const rewardMetrics = new Map((await Promise.all(rewardValues.map(measureGlyph))).map((metric) => [metric.value, metric]));
function capsuleBox(space) {
  const isRest = layout.rests.includes(space.index);
  const width = isRest ? layout.tile.restSpriteWidth : layout.tile.wellWidth;
  const top = isRest ? space.y - layout.tile.restSpriteTopOffset : space.y - layout.tile.wellHeight / 2;
  const capsule = layout.tile.capsules[isRest ? 'rest' : 'regular'];
  return { x: space.x - width / 2 + capsule.left, y: top + capsule.top, width: capsule.width, height: capsule.height };
}
function rewardLayout(space) {
  const capsule = capsuleBox(space);
  const inner = { x: capsule.x + capsuleInset.horizontal, y: capsule.y + capsuleInset.vertical, width: capsule.width - capsuleInset.horizontal * 2, height: capsule.height - capsuleInset.vertical * 2 };
  const coinMetric = rewardMetrics.get(String(space.coins));
  const twigMetric = space.points === 0 ? null : rewardMetrics.get(String(space.points));
  const coinPart = rewardIcon.coin.width + 1 + coinMetric.width;
  const twigPart = twigMetric ? rewardIcon.twig.width + 1 + twigMetric.width : 0;
  const total = coinPart + (twigMetric ? 2 + twigPart : 0);
  const left = inner.x + (inner.width - total) / 2;
  const centerY = inner.y + inner.height / 2;
  const number = (metric, visualLeft) => ({ text: metric.value, x: visualLeft - metric.leftBearing, baseline: centerY - metric.topBearing - metric.height / 2, visual: { x: visualLeft, y: centerY - metric.height / 2, width: metric.width, height: metric.height } });
  const icons = [{ name: 'coin', x: left, y: centerY - rewardIcon.coin.height / 2, width: rewardIcon.coin.width, height: rewardIcon.coin.height }];
  const numbers = [number(coinMetric, left + rewardIcon.coin.width + 1)];
  if (twigMetric) {
    const twigLeft = left + coinPart + 2;
    icons.push({ name: 'twig', x: twigLeft, y: centerY - rewardIcon.twig.height / 2, width: rewardIcon.twig.width, height: rewardIcon.twig.height });
    numbers.push(number(twigMetric, twigLeft + rewardIcon.twig.width + 1));
  }
  const boxes = [...icons, ...numbers.map((entry) => entry.visual)];
  const minX = Math.min(...boxes.map((box) => box.x)); const maxX = Math.max(...boxes.map((box) => box.x + box.width));
  const minY = Math.min(...boxes.map((box) => box.y)); const maxY = Math.max(...boxes.map((box) => box.y + box.height));
  return { capsule, inner, icons, numbers, content: { x: minX, y: minY, width: maxX - minX, height: maxY - minY }, totalWidth: total };
}

function validate() {
  const errors = [];
  const expectedIndices = Array.from({ length: 53 }, (_, offset) => offset + 1);
  const indices = layout.spaces.map((space) => space.index);
  const unique = new Set(indices);
  const exactIndexSet = expectedIndices.every((index) => unique.has(index)) && unique.size === 53;
  if (!exactIndexSet) errors.push('Layout must contain every internal anchor 1–53 exactly once.');
  if (layout.internalStartAnchor?.index !== 0) errors.push('Internal start anchor must remain index 0.');
  const coreIndices = track.spaces.map((space) => space.index);
  const contiguousCore = coreIndices.length === 54 && coreIndices.every((index, expected) => index === expected);
  if (!contiguousCore) errors.push('Track data must remain contiguous from Core index 0 through 53.');
  const rewards = layout.spaces.map((space) => track.spaces.find((entry) => entry.index === space.index));
  const badRewards = rewards.filter((entry) => !entry || !Number.isInteger(entry.coins) || !Number.isInteger(entry.points)).map((entry) => entry?.index ?? 'missing');
  if (badRewards.length) errors.push(`Invalid reward rows: ${badRewards.join(', ')}.`);
  const restSet = new Set(layout.rests);
  const sourceRubySet = new Set(track.rubyPositions);
  const exactRests = layout.rests.length === 8 && requiredRests.every((index) => restSet.has(index)) && restSet.size === 8 && layout.rests.every((index) => sourceRubySet.has(index));
  if (!exactRests) errors.push('Rest pads must be the confirmed eight source-ruby anchors.');
  const totalHeight = layout.tile.wellHeight + layout.tile.rewardHeight;
  const bodies = layout.spaces.map((space) => ({ index: space.index, box: rect(space.x, space.y + layout.tile.rewardHeight / 2, layout.tile.wellWidth, totalHeight) }));
  const bodyOverlaps = [];
  for (let i = 0; i < bodies.length; i += 1) for (let j = i + 1; j < bodies.length; j += 1) if (overlaps(bodies[i].box, bodies[j].box)) bodyOverlaps.push([bodies[i].index, bodies[j].index]);
  const outOfBounds = bodies.filter(({ box }) => !within(box, layout.boardArea)).map(({ index }) => index);
  if (bodyOverlaps.length) errors.push(`Tile bodies overlap: ${bodyOverlaps.map((pair) => pair.join('/')).join(', ')}.`);
  if (outOfBounds.length) errors.push(`Tile bodies outside board area: ${outOfBounds.join(', ')}.`);
  const zeroTwig = track.spaces.filter((space) => space.index >= 1 && space.index <= 53 && space.points === 0).map((space) => space.index);
  const exactZeroTwig = zeroTwig.length === 5 && zeroTwig.every((index) => index >= 1 && index <= 5);
  if (!exactZeroTwig) errors.push('Exactly anchors 1–5 must omit the Twig icon and amount.');
  const connectors = layout.shelterConnectors.map(({ index }) => index);
  const exactConnectors = connectors.length === 8 && requiredRests.every((index) => connectors.includes(index));
  if (!exactConnectors) errors.push('Each rest pad needs one footprint path to its shelter.');
  const rewardContent = layout.spaces.map((space) => {
    const row = rewardLayout({ ...space, ...track.spaces.find((entry) => entry.index === space.index) });
    return { index: space.index, content: row.content, capsule: row.capsule, innerCapsule: row.inner, totalWidth: row.totalWidth };
  });
  const rewardOutsideCapsules = rewardContent.filter(({ content, innerCapsule }) => !within(content, innerCapsule)).map(({ index }) => index);
  if (rewardOutsideCapsules.length) errors.push(`Reward icon-and-text content escapes painted capsules: ${rewardOutsideCapsules.join(', ')}.`);
  const atlasCrops = atlasMetadata.crops ?? {};
  const missingCrops = cropNames.filter((name) => !atlasCrops[name]);
  const malformedCrops = cropNames.filter((name) => { const crop = atlasCrops[name]; return crop && (!Number.isInteger(crop.left) || !Number.isInteger(crop.top) || !Number.isInteger(crop.width) || !Number.isInteger(crop.height) || crop.width <= 0 || crop.height <= 0); });
  if (missingCrops.length) errors.push(`Tile atlas missing crops: ${missingCrops.join(', ')}.`);
  if (malformedCrops.length) errors.push(`Tile atlas has invalid crops: ${malformedCrops.join(', ')}.`);
  const rewardRows = rewards.map(({ index, coins, points }) => ({ internalAnchor: index, pondPennies: coins, twigs: points }));
  return {
    renderer: 'static image-only bitmap atlas with SVG text/arrows/footprints', version: 5,
    referenceCanvas: layout.referenceCanvas, requestedOutput: layout.renderCanvas,
    background: backgroundPath, output: outputPath, outputFormat,
    checks: {
      visibleIndices: { expected: 0, actual: 0, exact: true },
      startOverlay: { expected: 'none; nest remains background-only', actual: 'none', exact: true },
      internalAnchors: { expected: 53, actual: indices.length, unique: unique.size, exact: exactIndexSet },
      coreIndices: { expected: '0–53 contiguous', actual: `${coreIndices[0]}–${coreIndices.at(-1)}`, exact: contiguousCore },
      terminalScoring: { expected: 53, actual: layout.spaces.find((space) => space.index === 53)?.role ?? 'ordinary', exact: layout.spaces.find((space) => space.index === 53)?.role === 'scoring-only' },
      rewards: { source: 'track-data.json', rows: rewardRows, exact: badRewards.length === 0 },
      rewardContentBounds: { font: { family: rewardFont, size: rewardFontSize, measuredGlyphs: [...rewardMetrics.values()] }, innerInset: capsuleInset, checked: rewardContent.map(({ index, content, capsule, innerCapsule, totalWidth }) => ({ internalAnchor: index, content, capsule, innerCapsule, totalWidth, withinInnerCapsule: within(content, innerCapsule) })), outsideCapsules: rewardOutsideCapsules, exact: rewardOutsideCapsules.length === 0 },
      zeroTwigOmissions: { expected: [1, 2, 3, 4, 5], actual: zeroTwig, exact: exactZeroTwig },
      rests: { expected: requiredRests, actual: layout.rests, count: layout.rests.length, originalRubySubset: layout.rests.every((index) => sourceRubySet.has(index)), exact: exactRests },
      footprintConnectors: { expected: requiredRests, actual: connectors, count: connectors.length, exact: exactConnectors },
      tileBodies: { regular: { width: layout.tile.wellWidth, wellHeight: layout.tile.wellHeight, rewardHeight: layout.tile.rewardHeight }, outOfBounds, overlaps: bodyOverlaps, exact: !outOfBounds.length && !bodyOverlaps.length },
      atlas: { metadata: atlasMetadataPath, atlas: atlasPath, requiredCrops: cropNames, missingCrops, malformedCrops, exact: !missingCrops.length && !malformedCrops.length }
    }, errors, passed: errors.length === 0
  };
}

const qa = validate();
let atlasInfo = null;
if (!existsSync(atlasPath)) {
  qa.errors.push(`Tile atlas not found: ${atlasPath}`);
} else {
  atlasInfo = await sharp(atlasPath).metadata();
  const cropBounds = cropNames.filter((name) => {
    const crop = atlasMetadata.crops?.[name];
    return crop && (crop.left + crop.width > atlasInfo.width || crop.top + crop.height > atlasInfo.height);
  });
  qa.checks.atlas.sourceDimensions = { width: atlasInfo.width, height: atlasInfo.height };
  qa.checks.atlas.hasAlpha = Boolean(atlasInfo.hasAlpha);
  qa.checks.atlas.outOfBoundsCrops = cropBounds;
  if (!atlasInfo.hasAlpha) qa.errors.push('Tile atlas must retain a transparent alpha channel.');
  if (cropBounds.length) qa.errors.push(`Atlas crops outside source bounds: ${cropBounds.join(', ')}.`);
}
qa.passed = qa.errors.length === 0;
if (!qa.passed) { console.error(qa.errors.join('\n')); process.exitCode = 1; }

function textSvg() {
  const rewardTexts = [];
  const iconPlacements = [];
  const addIcon = (name, x, y, width, height) => iconPlacements.push({ name, x, y, width, height });
  for (const layoutSpace of layout.spaces) {
    const space = { ...layoutSpace, ...track.spaces.find((entry) => entry.index === layoutSpace.index) };
    const row = rewardLayout(space);
    row.icons.forEach((icon) => addIcon(icon.name, icon.x, icon.y, icon.width, icon.height));
    row.numbers.forEach((number) => rewardTexts.push(`<text x="${number.x}" y="${number.baseline}" class="reward-text">${escape(number.text)}</text>`));
  }
  addIcon('regularA', 60, 950, 54, 50);
  addIcon('coin', 244, 954, 36, 36);
  addIcon('twig', 506, 955, 42, 34);
  addIcon('restIcon', 800, 952, 42, 42);
  const separator = (x) => `<text x="${x}" y="985" class="separator">|</text>`;
  const legendMarkup = `<text x="128" y="985" class="legend-text">TILE HERE</text>${separator(224)}<text x="290" y="985" class="legend-text">Pond pennies</text>${separator(483)}<text x="558" y="985" class="legend-text">Twigs</text>${separator(770)}<text x="852" y="985" class="legend-text">REST</text>${separator(1010)}<path d="M 1052 970 h 61 m -11 -9 l 13 9 -13 9" class="legend-arrow"/><text x="1130" y="985" class="legend-text">Score the next empty space</text>`;
  const bridgeChevrons = layout.chevrons.map(({ x, y, angle }) => `<path d="M ${x - 7} ${y - 7} L ${x + 1} ${y} L ${x - 7} ${y + 7}" class="bridge-arrow" transform="rotate(${angle} ${x} ${y})"/>`).join('');
  return { svg: `<svg xmlns="http://www.w3.org/2000/svg" width="${layout.renderCanvas.width}" height="${layout.renderCanvas.height}" viewBox="0 0 ${layout.referenceCanvas.width} ${layout.referenceCanvas.height}"><style>text { font-family: "Marker Felt", "Chalkboard SE", "Comic Sans MS", sans-serif; paint-order: stroke; }.reward-text { fill:#593d26; font-size:${rewardFontSize}px; font-weight:800; }.legend-text { fill:#4f3622; stroke:#faedc9; stroke-width:1.8px; font-size:18px; font-weight:900; }.separator { fill:#795633; font-size:27px; font-weight:500; }.legend-arrow { fill:none; stroke:#583b24; stroke-width:3px; stroke-linecap:round; stroke-linejoin:round; stroke-dasharray:9 7; }.bridge-arrow { fill:none; stroke:#705020; stroke-width:2.1px; stroke-linecap:round; stroke-linejoin:round; opacity:.82; }</style>${bridgeChevrons}${rewardTexts.join('')}${legendMarkup}</svg>`, iconPlacements };
}

function quadraticPoint(points, t) {
  if (points.length === 2) return [points[0][0] + (points[1][0] - points[0][0]) * t, points[0][1] + (points[1][1] - points[0][1]) * t];
  const [a, b, c] = points; const inverse = 1 - t;
  return [inverse * inverse * a[0] + 2 * inverse * t * b[0] + t * t * c[0], inverse * inverse * a[1] + 2 * inverse * t * b[1] + t * t * c[1]];
}
function footprintSvg() {
  return layout.shelterConnectors.map(({ path }) => {
    const spans = path.slice(1).reduce((sum, point, index) => sum + Math.hypot(point[0] - path[index][0], point[1] - path[index][1]), 0);
    const count = Math.max(1, Math.floor((spans - 10) / 14));
    return Array.from({ length: count }, (_, index) => {
      const t = Math.min(.92, (8 + index * 14) / spans);
      const [x, y] = quadraticPoint(path, t);
      const [beforeX, beforeY] = quadraticPoint(path, Math.max(0, t - .015));
      const [afterX, afterY] = quadraticPoint(path, Math.min(1, t + .015));
      const angle = Math.atan2(afterY - beforeY, afterX - beforeX) * 180 / Math.PI + 90;
      return `<path d="M 0 3 L 0 -2 M 0 -.4 L -2.15 -2.55 M 0 -.4 L 2.15 -2.55" fill="none" stroke="#a57c3f" stroke-width="1.45" stroke-linecap="round" stroke-linejoin="round" transform="translate(${x} ${y}) rotate(${angle})"/>`;
    }).join('');
  }).join('');
}

async function sprite(name, width, height) {
  const crop = atlasMetadata.crops[name];
  return sharp(atlasPath).extract(crop).resize(Math.round(width * scale), Math.round(height * scale), { fit: 'fill' }).png().toBuffer();
}

if (!checkOnly && qa.passed) {
  if (!existsSync(backgroundPath)) { console.error(`Background artwork not found: ${backgroundPath}`); process.exitCode = 1; }
  else if (!existsSync(atlasPath)) { console.error(`Tile atlas not found: ${atlasPath}`); process.exitCode = 1; }
  else {
    const composites = [{ input: Buffer.from(`<svg xmlns="http://www.w3.org/2000/svg" width="${layout.renderCanvas.width}" height="${layout.renderCanvas.height}" viewBox="0 0 ${layout.referenceCanvas.width} ${layout.referenceCanvas.height}">${footprintSvg()}</svg>`), left: 0, top: 0 }];
    const paintOrder = [...layout.spaces.filter((space) => layout.rests.includes(space.index)), ...layout.spaces.filter((space) => !layout.rests.includes(space.index))];
    for (const space of paintOrder) {
      const name = `${layout.rests.includes(space.index) ? 'rest' : 'regular'}${['A', 'B', 'C'][(space.index - 1) % 3]}`;
      const isRest = layout.rests.includes(space.index);
      const width = isRest ? layout.tile.restSpriteWidth : layout.tile.wellWidth;
      const height = isRest ? layout.tile.restSpriteHeight : layout.tile.wellHeight + layout.tile.rewardHeight;
      const top = isRest ? space.y - layout.tile.restSpriteTopOffset : space.y - layout.tile.wellHeight / 2;
      composites.push({ input: await sprite(name, width, height), left: Math.round((space.x - width / 2) * scale), top: Math.round(top * scale) });
    }
    const { svg, iconPlacements } = textSvg();
    for (const placement of iconPlacements) composites.push({ input: await sprite(placement.name, placement.width, placement.height), left: Math.round(placement.x * scale), top: Math.round(placement.y * scale) });
    composites.push({ input: Buffer.from(svg), left: 0, top: 0 });
    const sourceInfo = await sharp(backgroundPath).metadata();
    const composition = sharp(backgroundPath).resize(layout.renderCanvas.width, layout.renderCanvas.height, { fit: 'fill' }).composite(composites);
    if (outputFormat === 'jpeg') await composition.jpeg({ quality: 98, chromaSubsampling: '4:4:4' }).toFile(outputPath);
    else await composition.png({ compressionLevel: 9, palette: false }).toFile(outputPath);
    qa.render = { sourceDimensions: { width: sourceInfo.width, height: sourceInfo.height }, atlasDimensions: { width: atlasInfo.width, height: atlasInfo.height }, outputDimensions: layout.renderCanvas, format: outputFormat, jpegQuality: outputFormat === 'jpeg' ? 98 : null, chromaSubsampling: outputFormat === 'jpeg' ? '4:4:4' : null, backgroundPreservedAs: 'unmodified selected background plus bitmap atlas and SVG text overlay' };
    await writeFile(qaPath, `${JSON.stringify(qa, null, 2)}\n`);
    console.log(`Rendered ${outputPath}`);
  }
}
console.log(`QA ${qa.passed ? 'passed' : 'failed'}: ${qaPath}${checkOnly ? ' (not rewritten by --check)' : ''}`);
