#!/usr/bin/env node
/**
 * Static V4 board typesetter. It reads audited rule values but changes neither
 * Unity nor game rules. Internal anchors stay unprinted so the illustration,
 * pads, and reward rows read as a tabletop trail rather than an indexed chart.
 *
 * Usage:
 *   NODE_PATH=/.../node_modules node render-board.mjs
 *   NODE_PATH=/.../node_modules node render-board.mjs --check
 */
import { readFile, writeFile } from 'node:fs/promises';
import { existsSync } from 'node:fs';
import { dirname, extname, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';
import { createRequire } from 'node:module';

const require = createRequire(import.meta.url);
const sharp = (() => {
  try {
    return require('sharp');
  } catch {
    return require('/Users/george/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/sharp');
  }
})();

const here = dirname(fileURLToPath(import.meta.url));
const args = process.argv.slice(2);
const option = (name, fallback) => {
  const index = args.indexOf(name);
  return index === -1 ? fallback : args[index + 1];
};
const checkOnly = args.includes('--check');
const backgroundPath = resolve(here, option('--background', 'board-art-v4.png'));
const outputPath = resolve(here, option('--output', 'three-biome-board-v4.jpg'));
const qaPath = resolve(here, option('--qa', 'typeset-qa.json'));
const outputExtension = extname(outputPath).toLowerCase();
const outputFormat = outputExtension === '.jpg' || outputExtension === '.jpeg' ? 'jpeg' : outputExtension === '.png' ? 'png' : null;
if (!outputFormat) throw new Error(`Unsupported output extension "${outputExtension}". Use .jpg, .jpeg, or .png.`);

const [layout, track] = await Promise.all([
  readFile(resolve(here, 'layout.json'), 'utf8').then(JSON.parse),
  readFile(resolve(here, 'track-data.json'), 'utf8').then(JSON.parse)
]);

const requiredRests = [5, 13, 20, 28, 34, 40, 46, 52];
const escape = (value) => String(value).replace(/[&<>"']/g, (char) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&apos;' })[char]);
const rect = (x, y, width, height) => ({ x: x - width / 2, y: y - height / 2, width, height });
const overlaps = (a, b) => a.x < b.x + b.width && a.x + a.width > b.x && a.y < b.y + b.height && a.y + a.height > b.y;
const within = (box, area) => box.x >= area.x && box.y >= area.y && box.x + box.width <= area.x + area.width && box.y + box.height <= area.y + area.height;

function validate() {
  const errors = [];
  const expectedIndices = Array.from({ length: 53 }, (_, offset) => offset + 1);
  const layoutIndices = layout.spaces.map((space) => space.index);
  const uniqueIndices = new Set(layoutIndices);
  const exactIndexSet = expectedIndices.every((index) => uniqueIndices.has(index)) && uniqueIndices.size === 53;
  if (!exactIndexSet) errors.push('Layout must contain every internal anchor 1–53 exactly once.');
  if (layout.internalStartAnchor?.index !== 0) errors.push('The permanent duck start must remain internal index 0.');

  const coreIndices = track.spaces.map((space) => space.index);
  const contiguousCore = coreIndices.length === 54 && coreIndices.every((index, expected) => index === expected);
  if (!contiguousCore) errors.push('Track data must remain contiguous from Core index 0 through 53.');

  const rewardMismatches = layout.spaces.filter((space) => {
    const core = track.spaces.find((entry) => entry.index === space.index);
    return !core || !Number.isInteger(core.coins) || !Number.isInteger(core.points);
  }).map((space) => space.index);
  if (rewardMismatches.length) errors.push(`Missing or invalid reward values for: ${rewardMismatches.join(', ')}.`);

  const restSet = new Set(layout.rests);
  const exactRests = layout.rests.length === 8 && requiredRests.every((index) => restSet.has(index)) && restSet.size === 8;
  const sourceRubySet = new Set(track.rubyPositions);
  const restsAreSourceRubies = layout.rests.every((index) => sourceRubySet.has(index));
  if (!exactRests) errors.push('Rest pads must be the confirmed eight internal anchors.');
  if (!restsAreSourceRubies) errors.push('Each visual rest pad must remain tied to an original ruby position.');

  const tileHeight = layout.tile.wellHeight + layout.tile.rewardHeight;
  const boxes = layout.spaces.map((space) => ({
    index: space.index,
    box: rect(space.x, space.y + layout.tile.rewardHeight / 2, layout.tile.wellWidth, tileHeight)
  }));
  const overlapsFound = [];
  for (let i = 0; i < boxes.length; i += 1) {
    for (let j = i + 1; j < boxes.length; j += 1) {
      if (overlaps(boxes[i].box, boxes[j].box)) overlapsFound.push([boxes[i].index, boxes[j].index]);
    }
  }
  if (overlapsFound.length) errors.push(`Tile bodies overlap: ${overlapsFound.map((pair) => pair.join('/')).join(', ')}.`);
  const outOfBounds = boxes.filter((entry) => !within(entry.box, layout.boardArea)).map((entry) => entry.index);
  if (outOfBounds.length) errors.push(`Tile bodies outside board area: ${outOfBounds.join(', ')}.`);
  if (overlaps(layout.boardArea, layout.legendArea)) errors.push('Legend area intersects board area.');

  const zeroTwigIndices = track.spaces.filter((space) => space.index >= 1 && space.index <= 53 && space.points === 0).map((space) => space.index);
  const exactZeroTwigOmissions = zeroTwigIndices.length === 5 && zeroTwigIndices.every((index) => index >= 1 && index <= 5);
  if (!exactZeroTwigOmissions) errors.push('Exactly the first five reward rows must omit the Twig icon and value.');
  const rewardRows = track.spaces.filter((space) => space.index >= 1 && space.index <= 53).map(({ index, coins, points }) => ({ internalAnchor: index, pondPennies: coins, twigs: points }));
  const connectorIndices = (layout.shelterConnectors ?? []).map((connector) => connector.index);
  const connectorSet = new Set(connectorIndices);
  const exactConnectors = connectorIndices.length === 8 && requiredRests.every((index) => connectorSet.has(index)) && connectorSet.size === 8;
  if (!exactConnectors) errors.push('Each of the eight rest pads must have one shelter-entry connector.');
  const anchorGeometry = [11, 44, 53].map((index) => {
    const space = layout.spaces.find((entry) => entry.index === index);
    return { index, wellWidth: layout.tile.wellWidth, wellHeight: layout.tile.wellHeight, rewardHeight: layout.tile.rewardHeight, fullSize: Boolean(space) };
  });
  if (anchorGeometry.some((entry) => !entry.fullSize)) errors.push('Anchors 11, 44, and 53 must use full-size pad and reward geometry.');

  return {
    renderer: 'static image-only SVG overlay',
    version: 4,
    referenceCanvas: layout.referenceCanvas,
    requestedOutput: layout.renderCanvas,
    background: backgroundPath,
    output: outputPath,
    outputFormat,
    checks: {
      visibleIndices: { expected: 0, actual: 0, exact: true },
      startOverlay: { expected: 'none; background-only incomplete nest', actual: 'none', exact: true },
      internalAnchors: { expected: 53, actual: layoutIndices.length, unique: uniqueIndices.size, exact: exactIndexSet },
      coreIndices: { expected: '0–53 contiguous', actual: `${coreIndices[0]}–${coreIndices.at(-1)}`, exact: contiguousCore },
      encounterPositions: { expected: '1–52', actual: '1–52', exact: layout.spaces.filter((space) => space.index <= 52).length === 52 },
      terminalScoring: { expected: 53, actual: layout.spaces.find((space) => space.index === 53)?.role ?? 'ordinary', exact: layout.spaces.find((space) => space.index === 53)?.role === 'scoring-only' },
      rewards: { source: 'track-data.json', rowsChecked: layout.spaces.length, rows: rewardRows, mismatches: rewardMismatches, exact: rewardMismatches.length === 0 },
      zeroTwigOmissions: { expected: [1, 2, 3, 4, 5], actual: zeroTwigIndices, exact: exactZeroTwigOmissions },
      rests: { expected: requiredRests, actual: layout.rests, count: layout.rests.length, originalRubySubset: restsAreSourceRubies, exact: exactRests && restsAreSourceRubies },
      shelterConnectors: { expectedRestAnchors: requiredRests, actualRestAnchors: connectorIndices, count: connectorIndices.length, presentation: 'warm ochre duck footprints; 5–6 px three-toed prints at 14 px intervals; no connector line or crossbar', exact: exactConnectors },
      tileBodies: { regular: { width: layout.tile.wellWidth, wellHeight: layout.tile.wellHeight, rewardHeight: layout.tile.rewardHeight }, outOfBounds, overlaps: overlapsFound, exact: outOfBounds.length === 0 && overlapsFound.length === 0 },
      consistencyAnchors: { anchors: anchorGeometry, exact: anchorGeometry.every((entry) => entry.fullSize && entry.wellWidth === layout.tile.wellWidth && entry.rewardHeight === layout.tile.rewardHeight) },
      legend: { labels: ['Pond pennies', 'Twigs', 'Rest'], instruction: 'Score the next empty space.', area: layout.legendArea, separateFromBoard: !overlaps(layout.boardArea, layout.legendArea) }
    },
    errors,
    passed: errors.length === 0
  };
}

function coinIcon(cx, cy) {
  return `<g class="coin-icon"><circle cx="${cx}" cy="${cy}" r="6.3" fill="#e2ae32" stroke="#75461f" stroke-width="1.45"/><circle cx="${cx}" cy="${cy}" r="3.65" fill="none" stroke="#ffe394" stroke-width="1"/><path d="M ${cx - 1.8} ${cy - 2.3} q 2 -1.2 3.7 0 M ${cx - 1.8} ${cy + 2.3} q 2 1.2 3.7 0" fill="none" stroke="#fff0aa" stroke-width=".9" stroke-linecap="round"/></g>`;
}

function twigBundle(cx, cy) {
  return `<g class="twig-icon" fill="none" stroke-linecap="round"><path d="M ${cx - 6.2} ${cy + 4.8} L ${cx + 6.5} ${cy - 5.1} M ${cx - 5.2} ${cy + 5.5} L ${cx + 5.4} ${cy - .6} M ${cx - 2.6} ${cy + 5.6} L ${cx + 7.1} ${cy + 1.3}" stroke="#6a4026" stroke-width="2.5"/><path d="M ${cx - 5.6} ${cy + 3.8} L ${cx + 5.5} ${cy - 4.8} M ${cx - 4.2} ${cy + 4.4} L ${cx + 5} ${cy - .7}" stroke="#bb7a45" stroke-width=".75"/><path d="M ${cx + 1} ${cy - 2.3} l 3.8 -2 M ${cx + .8} ${cy + 2.2} l 4 1.4" stroke="#dca26a" stroke-width="1.05"/></g>`;
}

function feather(cx, cy) {
  return `<path d="M ${cx - 1} ${cy + 6} C ${cx + 2} ${cy + 2}, ${cx + 7} ${cy - 2}, ${cx + 5} ${cy - 8} C ${cx + 1} ${cy - 6}, ${cx - 5} ${cy - 1}, ${cx - 1} ${cy + 6} Z M ${cx - 1} ${cy + 6} L ${cx + 4} ${cy - 5}" fill="#f8e6a4" stroke="#754a28" stroke-width="1.1" stroke-linecap="round" stroke-linejoin="round"/>`;
}

function restMedallion(cx, cy) {
  return `<g class="rest-medallion"><circle cx="${cx}" cy="${cy}" r="10" fill="#416e46" stroke="#513b29" stroke-width="1.8"/><circle cx="${cx}" cy="${cy}" r="7.5" fill="#8cab61" stroke="#e5d998" stroke-width="1.05"/>${feather(cx - 1.4, cy + 1.5)}</g>`;
}

function restAccent(space) {
  const { x, y } = space;
  return `<g class="rest-accent" aria-label="Rest space"><path d="M ${x - 39} ${y + 17} C ${x - 48} ${y - 8}, ${x - 29} ${y - 31}, ${x - 6} ${y - 30}" fill="none" stroke="#557c43" stroke-width="2.1" stroke-linecap="round"/><ellipse cx="${x - 40}" cy="${y + 8}" rx="5" ry="2.4" fill="#78a34d" stroke="#315b38" stroke-width=".7" transform="rotate(-42 ${x - 40} ${y + 8})"/><ellipse cx="${x - 28}" cy="${y - 14}" rx="5" ry="2.4" fill="#9eb95a" stroke="#315b38" stroke-width=".7" transform="rotate(25 ${x - 28} ${y - 14})"/>${restMedallion(x - 32, y - 23)}</g>`;
}

function rewardTab(space, x, y) {
  const top = y + layout.tile.wellHeight / 2;
  const bottom = top + layout.tile.rewardHeight;
  const middle = (top + bottom) / 2;
  const textWidth = (value) => String(value).length * 7.4;
  const coinWidth = 13 + textWidth(space.coins);
  const twigWidth = 15 + textWidth(space.points);
  const total = space.points === 0 ? coinWidth : coinWidth + 8 + twigWidth;
  const left = x - total / 2;
  const coinX = left + 6.5;
  const coinTextX = left + 13;
  const twigLeft = left + coinWidth + 8;
  const twigX = twigLeft + 7.5;
  const twigTextX = twigLeft + 15;
  const twig = space.points === 0 ? '' : `${twigBundle(twigX, middle)}<text x="${twigTextX}" y="${middle + 4.5}" class="reward-text">${escape(space.points)}</text>`;
  return `<g class="reward" aria-label="${space.coins} Pond pennies and ${space.points} Twigs"><path d="M ${x - 42} ${top + 3} Q ${x - 41} ${top} ${x - 37} ${top} H ${x + 37} Q ${x + 41} ${top} ${x + 42} ${top + 3} L ${x + 39} ${bottom - 3} Q ${x + 38} ${bottom} ${x + 33} ${bottom} H ${x - 33} Q ${x - 38} ${bottom} ${x - 39} ${bottom - 3} Z" fill="#fff0c7" stroke="#70492e" stroke-width="2" stroke-linejoin="round"/>${coinIcon(coinX, middle)}<text x="${coinTextX}" y="${middle + 4.5}" class="reward-text">${escape(space.coins)}</text>${twig}</g>`;
}

function tile(space) {
  const { x, y } = space;
  const w = layout.tile.wellWidth / 2;
  const h = layout.tile.wellHeight / 2;
  const isRest = layout.rests.includes(space.index);
  const face = isRest ? '#d9e5b4' : '#f6dfaa';
  const inner = isRest ? '#c7d79f' : '#eed39d';
  const body = `<path d="M ${x - w + 7} ${y - h} Q ${x - w + 20} ${y - h - 2} ${x - 4} ${y - h} Q ${x + 22} ${y - h - 3} ${x + w - 4} ${y - h + 8} L ${x + w} ${y + 8} Q ${x + w - 3} ${y + h - 7} ${x + 25} ${y + h} Q ${x} ${y + h + 3} ${x - 23} ${y + h} Q ${x - w + 2} ${y + h - 3} ${x - w} ${y + 8} L ${x - w + 2} ${y - 10} Q ${x - w - 1} ${y - h + 3} ${x - w + 7} ${y - h} Z" fill="${face}" stroke="#5c3b28" stroke-width="3" stroke-linejoin="round"/><path d="M ${x - w + 10} ${y - h + 5} Q ${x - 4} ${y - h + 1} ${x + w - 9} ${y - h + 7} L ${x + w - 5} ${y + h - 10} Q ${x} ${y + h - 3} ${x - w + 7} ${y + h - 9} Z" fill="${inner}" opacity=".6"/>`;
  return `<g class="tile tile-${space.index}">${body}${isRest ? restAccent(space) : ''}${rewardTab(space, x, y)}</g>`;
}

function chevrons() {
  return layout.chevrons.map(({ x, y, angle }) => `<path d="M ${x - 7} ${y - 7} L ${x + 1} ${y} L ${x - 7} ${y + 7}" fill="none" stroke="#705020" stroke-width="2.1" stroke-linecap="round" stroke-linejoin="round" opacity=".8" transform="rotate(${angle} ${x} ${y})"/>`).join('');
}

function shelterConnectors() {
  return (layout.shelterConnectors ?? []).map(({ fromX, fromY, toX, toY }) => {
    const midX = (fromX + toX) / 2;
    const midY = (fromY + toY) / 2;
    const dx = toX - fromX;
    const dy = toY - fromY;
    const length = Math.hypot(dx, dy) || 1;
    const nx = -dy / length;
    const ny = dx / length;
    const controlX = midX + nx * 4;
    const controlY = midY + ny * 4;
    const footprintCount = Math.max(1, Math.floor((length - 12) / 14));
    const prints = Array.from({ length: footprintCount }, (_, index) => {
      const distance = 8 + index * 14;
      const t = Math.min(0.92, distance / length);
      const inverse = 1 - t;
      const x = inverse * inverse * fromX + 2 * inverse * t * controlX + t * t * toX;
      const y = inverse * inverse * fromY + 2 * inverse * t * controlY + t * t * toY;
      const tangentX = 2 * inverse * (controlX - fromX) + 2 * t * (toX - controlX);
      const tangentY = 2 * inverse * (controlY - fromY) + 2 * t * (toY - controlY);
      const angle = Math.atan2(tangentY, tangentX) * 180 / Math.PI + 90;
      return `<path d="M 0 3 L 0 -2 M 0 -0.4 L -2.15 -2.55 M 0 -0.4 L 2.15 -2.55" fill="none" stroke="#a57c3f" stroke-width="1.45" stroke-linecap="round" stroke-linejoin="round" transform="translate(${x} ${y}) rotate(${angle})"/>`;
    }).join('');
    return `<g class="shelter-connector" aria-label="Duck footprints to rest entrance">${prints}</g>`;
  }).join('');
}

function legend() {
  const y = 975;
  return `<g class="legend" aria-label="Board legend">${coinIcon(120, y)}<text x="134" y="${y + 5}" class="legend-text">Pond pennies</text>${twigBundle(315, y)}<text x="331" y="${y + 5}" class="legend-text">Twigs</text>${restMedallion(465, y)}<text x="480" y="${y + 5}" class="legend-text">Rest</text><text x="650" y="${y + 5}" class="legend-emphasis">Score the next empty space.</text></g>`;
}

function svg() {
  const { width, height } = layout.renderCanvas;
  return `<svg xmlns="http://www.w3.org/2000/svg" width="${width}" height="${height}" viewBox="0 0 ${layout.referenceCanvas.width} ${layout.referenceCanvas.height}"><style>text { font-family: "Avenir Next", "Trebuchet MS", sans-serif; paint-order: stroke; }.reward-text { fill: #573a29; font-size: 14px; font-weight: 850; text-anchor: start; }.legend-text { fill: #503825; stroke: #f9e9bc; stroke-width: 2px; font-size: 13px; font-weight: 800; }.legend-emphasis { fill: #503825; stroke: #f9e9bc; stroke-width: 2px; font-size: 13px; font-weight: 900; }</style>${chevrons()}${shelterConnectors()}${layout.spaces.map((entry) => tile({ ...entry, ...track.spaces.find((space) => space.index === entry.index) })).join('')}${legend()}</svg>`;
}

const qa = validate();
if (!qa.passed) {
  console.error(qa.errors.join('\n'));
  process.exitCode = 1;
}

if (!checkOnly && qa.passed) {
  if (!existsSync(backgroundPath)) {
    console.error(`Background artwork not found: ${backgroundPath}`);
    process.exitCode = 1;
  } else {
    const baseInfo = await sharp(backgroundPath).metadata();
    const overlay = Buffer.from(svg());
    const composition = sharp(backgroundPath)
      .resize(layout.renderCanvas.width, layout.renderCanvas.height, { fit: 'fill' })
      .composite([{ input: overlay, top: 0, left: 0 }]);
    if (outputFormat === 'jpeg') await composition.jpeg({ quality: 98, chromaSubsampling: '4:4:4' }).toFile(outputPath);
    else await composition.png({ compressionLevel: 9, palette: false }).toFile(outputPath);
    qa.render = { sourceDimensions: { width: baseInfo.width, height: baseInfo.height }, outputDimensions: layout.renderCanvas, format: outputFormat, jpegQuality: outputFormat === 'jpeg' ? 98 : null, chromaSubsampling: outputFormat === 'jpeg' ? '4:4:4' : null, backgroundPreservedAs: 'single background layer with SVG typesetting overlay' };
    await writeFile(qaPath, `${JSON.stringify(qa, null, 2)}\n`);
    console.log(`Rendered ${outputPath}`);
  }
}

console.log(`QA ${qa.passed ? 'passed' : 'failed'}: ${qaPath}${checkOnly ? ' (not rewritten by --check)' : ''}`);
