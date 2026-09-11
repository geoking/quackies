#!/usr/bin/env node
/**
 * Static V3 board typesetter. This is deliberately image-only: it reads the
 * audited Core values but changes neither Unity nor game rules.
 *
 * Usage:
 *   NODE_PATH=/.../node_modules node render-board.mjs \
 *     --background board-art-v3.png --output three-biome-board-v3.jpg
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
    // Codex's bundled runtime keeps the renderer reproducible when NODE_PATH is absent.
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
const backgroundPath = resolve(here, option('--background', 'board-art-v3.png'));
const outputPath = resolve(here, option('--output', 'three-biome-board-v3.jpg'));
const qaPath = resolve(here, option('--qa', 'typeset-qa.json'));
const outputExtension = extname(outputPath).toLowerCase();
const outputFormat = outputExtension === '.jpg' || outputExtension === '.jpeg' ? 'jpeg' : outputExtension === '.png' ? 'png' : null;
if (!outputFormat) throw new Error(`Unsupported output extension "${outputExtension}". Use .jpg, .jpeg, or .png.`);

const [layout, track] = await Promise.all([
  readFile(resolve(here, 'layout.json'), 'utf8').then(JSON.parse),
  readFile(resolve(here, 'track-data.json'), 'utf8').then(JSON.parse)
]);

const requiredRests = [5, 13, 20, 28, 34, 40, 46, 52];
const number = (value) => String(value);
const escape = (value) => String(value).replace(/[&<>"']/g, (char) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&apos;' })[char]);
const rect = (x, y, width, height) => ({ x: x - width / 2, y: y - height / 2, width, height });
const overlaps = (a, b) => a.x < b.x + b.width && a.x + a.width > b.x && a.y < b.y + b.height && a.y + a.height > b.y;
const within = (box, area) => box.x >= area.x && box.y >= area.y && box.x + box.width <= area.x + area.width && box.y + box.height <= area.y + area.height;

function validate() {
  const errors = [];
  const spaces = layout.spaces;
  const coreSpaces = track.spaces;
  const expectedIndices = Array.from({ length: 53 }, (_, offset) => offset + 1);
  const layoutIndices = spaces.map((space) => space.index);
  const uniqueIndices = new Set(layoutIndices);
  const exactIndexSet = expectedIndices.every((index) => uniqueIndices.has(index)) && uniqueIndices.size === 53;
  if (!exactIndexSet) errors.push('Layout must contain each printed index 1–53 exactly once.');
  if (layout.start.index !== 0) errors.push('The permanent duck start must remain index 0.');

  const coreIndices = coreSpaces.map((space) => space.index);
  const contiguousCore = coreIndices.length === 54 && coreIndices.every((index, expected) => index === expected);
  if (!contiguousCore) errors.push('Track data must remain contiguous from Core index 0 through 53.');

  const rewardMismatches = [];
  for (const space of spaces) {
    const core = coreSpaces.find((entry) => entry.index === space.index);
    if (!core || !Number.isInteger(core.coins) || !Number.isInteger(core.points)) rewardMismatches.push(space.index);
  }
  if (rewardMismatches.length) errors.push(`Missing or invalid reward values for: ${rewardMismatches.join(', ')}.`);

  const restSet = new Set(layout.rests);
  const exactRests = layout.rests.length === 8 && requiredRests.every((index) => restSet.has(index)) && restSet.size === 8;
  const sourceRubySet = new Set(track.rubyPositions);
  const restsAreSourceRubies = layout.rests.every((index) => sourceRubySet.has(index));
  if (!exactRests) errors.push('Rest pads must be the confirmed eight indices.');
  if (!restsAreSourceRubies) errors.push('Each visual rest pad must remain tied to an original ruby position.');

  const tileHeight = layout.tile.wellHeight + layout.tile.rewardHeight;
  const boxes = spaces.map((space) => {
    const scale = space.scale ?? 1;
    return {
      index: space.index,
      scale,
      box: rect(space.x, space.y + (layout.tile.rewardHeight * scale) / 2, layout.tile.wellWidth * scale, tileHeight * scale)
    };
  });
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

  return {
    renderer: 'static image-only SVG overlay',
    referenceCanvas: layout.referenceCanvas,
    requestedOutput: layout.renderCanvas,
    background: backgroundPath,
    output: outputPath,
    outputFormat,
    checks: {
      printedNumbers: { expected: 53, actual: layoutIndices.length, unique: uniqueIndices.size, exact: exactIndexSet },
      coreIndices: { expected: '0–53 contiguous', actual: `${coreIndices[0]}–${coreIndices.at(-1)}`, exact: contiguousCore },
      encounterPositions: { expected: '1–52', actual: '1–52', exact: spaces.filter((space) => space.index <= 52).length === 52 },
      terminalScoring: { expected: 53, actual: spaces.find((space) => space.index === 53)?.role ?? 'ordinary', exact: spaces.find((space) => space.index === 53)?.role === 'scoring-only' },
      rewards: { source: 'track-data.json', spacesChecked: spaces.length, mismatches: rewardMismatches, exact: rewardMismatches.length === 0 },
      rests: { expected: requiredRests, actual: layout.rests, count: layout.rests.length, originalRubySubset: restsAreSourceRubies, exact: exactRests && restsAreSourceRubies },
      tileBodies: { regular: { width: layout.tile.wellWidth, height: tileHeight }, outOfBounds, overlaps: overlapsFound, exact: outOfBounds.length === 0 && overlapsFound.length === 0 },
      legend: { area: layout.legendArea, separateFromBoard: !overlaps(layout.boardArea, layout.legendArea) }
    },
    errors,
    passed: errors.length === 0
  };
}

const qa = validate();
await writeFile(qaPath, `${JSON.stringify(qa, null, 2)}\n`);
if (!qa.passed) {
  console.error(qa.errors.join('\n'));
  process.exitCode = 1;
}

function leaf(cx, cy, angle, color) {
  return `<ellipse cx="${number(cx)}" cy="${number(cy)}" rx="4.8" ry="2.4" fill="${color}" stroke="#315b38" stroke-width="0.7" transform="rotate(${angle} ${number(cx)} ${number(cy)})"/>`;
}

function feather(cx, cy) {
  return `<path d="M ${cx - 1} ${cy + 7} C ${cx + 2} ${cy + 2}, ${cx + 8} ${cy - 2}, ${cx + 6} ${cy - 9} C ${cx + 1} ${cy - 7}, ${cx - 5} ${cy - 1}, ${cx - 1} ${cy + 7} Z M ${cx - 1} ${cy + 7} L ${cx + 5} ${cy - 6}" fill="#f8e6a4" stroke="#754a28" stroke-width="1.2" stroke-linecap="round" stroke-linejoin="round"/>`;
}

function restWreath(space) {
  const colors = ['#547d38', '#78a34d', '#416d36', '#9eb95a', '#5b8f42', '#7fa84d', '#3f6d3b', '#9cc05b'];
  const leaves = colors.map((color, index) => {
    const angle = index * 45 - 15;
    const radians = angle * Math.PI / 180;
    return leaf(space.x + Math.cos(radians) * 47, space.y + Math.sin(radians) * 33, angle + 90, color);
  }).join('');
  const medalX = space.x + 35;
  const medalY = space.y - 23;
  return `<g class="rest-marker" aria-label="Rest on tile ${space.index}">
    <path d="M ${space.x - 45} ${space.y + 17} C ${space.x - 49} ${space.y - 14}, ${space.x - 23} ${space.y - 37}, ${space.x + 11} ${space.y - 34} C ${space.x + 42} ${space.y - 30}, ${space.x + 51} ${space.y - 4}, ${space.x + 42} ${space.y + 25}" fill="none" stroke="#456f3c" stroke-width="2.2" stroke-linecap="round"/>
    ${leaves}
    <circle cx="${medalX}" cy="${medalY}" r="11" fill="#2f7180" stroke="#573c28" stroke-width="2"/>
    <circle cx="${medalX}" cy="${medalY}" r="8.3" fill="#7fb9bc" stroke="#f3d995" stroke-width="1.2"/>
    ${feather(medalX - 2, medalY + 2)}
  </g>`;
}

function rewardTab(space, x, y) {
  const terminal = space.index === 53;
  const fill = terminal ? '#f0cf72' : '#fff0c7';
  const stroke = terminal ? '#754b24' : '#70492e';
  const top = y + layout.tile.wellHeight / 2;
  const bottom = top + layout.tile.rewardHeight;
  const middle = (top + bottom) / 2;
  return `<g class="reward" aria-label="Tile ${space.index}: ${space.coins} pond pennies and ${space.points} twigs">
    <path d="M ${x - 42} ${top + 3} Q ${x - 41} ${top} ${x - 37} ${top} H ${x + 37} Q ${x + 41} ${top} ${x + 42} ${top + 3} L ${x + 39} ${bottom - 3} Q ${x + 38} ${bottom} ${x + 33} ${bottom} H ${x - 33} Q ${x - 38} ${bottom} ${x - 39} ${bottom - 3} Z" fill="${fill}" stroke="${stroke}" stroke-width="2" stroke-linejoin="round"/>
    <text x="${x}" y="${middle + 5}" class="reward-text">P${escape(space.coins)}   T${escape(space.points)}</text>
  </g>`;
}

function tile(space) {
  const { x, y } = space;
  const scale = space.scale ?? 1;
  const transform = scale === 1 ? '' : ` transform="translate(${x} ${y}) scale(${scale}) translate(${-x} ${-y})"`;
  const w = layout.tile.wellWidth / 2;
  const h = layout.tile.wellHeight / 2;
  const terminal = space.index === 53;
  const isRest = layout.rests.includes(space.index);
  const face = terminal ? '#f3d886' : isRest ? '#dce7b7' : '#f6dfaa';
  const inner = terminal ? '#ead07f' : isRest ? '#cddda7' : '#eed39d';
  const body = `<path d="M ${x - w + 7} ${y - h} Q ${x - w + 20} ${y - h - 2} ${x - 4} ${y - h} Q ${x + 22} ${y - h - 3} ${x + w - 4} ${y - h + 8} L ${x + w} ${y + 8} Q ${x + w - 3} ${y + h - 7} ${x + 25} ${y + h} Q ${x} ${y + h + 3} ${x - 23} ${y + h} Q ${x - w + 2} ${y + h - 3} ${x - w} ${y + 8} L ${x - w + 2} ${y - 10} Q ${x - w - 1} ${y - h + 3} ${x - w + 7} ${y - h} Z" fill="${face}" stroke="#5c3b28" stroke-width="3" stroke-linejoin="round"/>
    <path d="M ${x - w + 10} ${y - h + 5} Q ${x - 4} ${y - h + 1} ${x + w - 9} ${y - h + 7} L ${x + w - 5} ${y + h - 10} Q ${x} ${y + h - 3} ${x - w + 7} ${y + h - 9} Z" fill="${inner}" opacity="0.58"/>`;
  const badge = `<g class="index-badge" aria-label="Space ${space.index}">
    <rect x="${x - 53}" y="${y - 39}" width="24" height="20" rx="8" fill="#643e2a" stroke="#f4dba0" stroke-width="1.5"/>
    <text x="${x - 41}" y="${y - 24}" class="index-text">${space.index}</text>
  </g>`;
  const scoreMark = terminal ? `<g class="score-flag" aria-label="Terminal scoring space">
    <path d="M ${x + 16} ${y - 37} h 30 l -4 9 4 9 h -30 z" fill="#c88635" stroke="#704124" stroke-width="1.4" stroke-linejoin="round"/>
    <text x="${x + 30}" y="${y - 24.5}" class="score-text">SCORE</text>
  </g>` : '';
  return `<g class="tile tile-${space.index}"${transform}>
    ${body}
    ${isRest ? restWreath(space) : ''}
    ${scoreMark}
    ${badge}
    ${rewardTab(space, x, y)}
  </g>`;
}

function startMarker() {
  const { x, y, label } = layout.start;
  return `<g class="start" aria-label="Index 0 permanent duck start">
    <path d="M ${x - 35} ${y - 17} Q ${x - 14} ${y - 20} ${x + 8} ${y - 18} Q ${x + 33} ${y - 20} ${x + 38} ${y - 8} L ${x + 36} ${y + 13} Q ${x + 20} ${y + 20} ${x - 4} ${y + 18} Q ${x - 28} ${y + 20} ${x - 38} ${y + 11} L ${x - 37} ${y - 7} Q ${x - 39} ${y - 15} ${x - 35} ${y - 17} Z" fill="#f6dfaa" stroke="#60402d" stroke-width="3" stroke-linejoin="round"/>
    <path d="M ${x - 29} ${y - 11} Q ${x} ${y - 16} ${x + 30} ${y - 9} L ${x + 31} ${y + 7} Q ${x} ${y + 13} ${x - 30} ${y + 7} Z" fill="#eed39d" opacity="0.62"/>
    <rect x="${x - 48}" y="${y - 51}" width="31" height="19" rx="8" fill="#643e2a" stroke="#f4dba0" stroke-width="1.5"/>
    <text x="${x - 32.5}" y="${y - 37}" class="index-text">0</text>
    <text x="${x}" y="${y + 56}" class="start-text">${escape(label)}</text>
  </g>`;
}

function legend() {
  return `<g class="legend" aria-label="Board legend">
    <path d="M 94 975 q 4 -9 16 -9 h 23 q 9 0 11 9 l -2 12 h -46 z" fill="#f6dfaa" stroke="#60402d" stroke-width="1.6"/>
    <text x="153" y="985" class="legend-text">Tile space</text>
    <circle cx="315" cy="975" r="8" fill="#e6b63f" stroke="#784b20" stroke-width="1.4"/><text x="329" y="985" class="legend-text">P = Pond pennies</text>
    <path d="M 538 983 l 10 -15 M 543 978 l 6 4 M 545 974 l 6 4" stroke="#72502e" stroke-width="2" stroke-linecap="round"/><text x="559" y="985" class="legend-text">T = Twigs</text>
    <circle cx="709" cy="975" r="11" fill="#2f7180" stroke="#573c28" stroke-width="1.6"/>${feather(707, 977)}<text x="727" y="985" class="legend-text">Rest</text>
    <text x="865" y="985" class="legend-emphasis">Score the next empty space.</text>
  </g>`;
}

function svg() {
  const { width, height } = layout.renderCanvas;
  return `<svg xmlns="http://www.w3.org/2000/svg" width="${width}" height="${height}" viewBox="0 0 ${layout.referenceCanvas.width} ${layout.referenceCanvas.height}">
  <style>
    text { font-family: "Avenir Next", "Trebuchet MS", sans-serif; paint-order: stroke; }
    .index-text { fill: #fff4d2; font-size: 14px; font-weight: 800; text-anchor: middle; }
    .reward-text { fill: #573a29; font-size: 14px; font-weight: 800; text-anchor: middle; letter-spacing: -.3px; }
    .score-text { fill: #fff1bb; font-size: 6.6px; font-weight: 900; text-anchor: middle; letter-spacing: .15px; }
    .start-text { fill: #543725; stroke: #faebbf; stroke-width: 2px; font-size: 11px; font-weight: 900; text-anchor: middle; letter-spacing: .8px; }
    .legend-text { fill: #503825; stroke: #f9e9bc; stroke-width: 2px; font-size: 13px; font-weight: 800; }
    .legend-emphasis { fill: #503825; stroke: #f9e9bc; stroke-width: 2px; font-size: 13px; font-weight: 900; }
  </style>
  ${startMarker()}
  ${layout.spaces.map((entry) => tile({ ...entry, ...track.spaces.find((space) => space.index === entry.index) })).join('\n')}
  ${legend()}
</svg>`;
}

if (!checkOnly && qa.passed) {
  if (!existsSync(backgroundPath)) {
    console.error(`Background artwork not found: ${backgroundPath}`);
    process.exitCode = 1;
  } else {
    const baseInfo = await sharp(backgroundPath).metadata();
    const outputInfo = layout.renderCanvas;
    const overlay = Buffer.from(svg());
    const composition = sharp(backgroundPath)
      .resize(outputInfo.width, outputInfo.height, { fit: 'fill' })
      .composite([{ input: overlay, top: 0, left: 0 }]);
    if (outputFormat === 'jpeg') {
      await composition.jpeg({ quality: 98, chromaSubsampling: '4:4:4' }).toFile(outputPath);
    } else {
      await composition.png({ compressionLevel: 9, palette: false }).toFile(outputPath);
    }
    qa.render = {
      sourceDimensions: { width: baseInfo.width, height: baseInfo.height },
      outputDimensions: outputInfo,
      format: outputFormat,
      backgroundPreservedAs: 'single background layer with SVG typesetting overlay'
    };
    await writeFile(qaPath, `${JSON.stringify(qa, null, 2)}\n`);
    console.log(`Rendered ${outputPath}`);
  }
}

console.log(`QA ${qa.passed ? 'passed' : 'failed'}: ${qaPath}`);
