const fs = require('fs');
const path = require('path');

const projectRoot = path.resolve(__dirname, '..');
const nodeCss = path.join(projectRoot, 'node_modules', '@mdi', 'font', 'css', 'materialdesignicons.min.css');
const nodeFontsDir = path.join(projectRoot, 'node_modules', '@mdi', 'font', 'fonts');
const outFontsDir = path.join(projectRoot, 'wwwroot', 'fonts');
const outCss = path.join(projectRoot, 'Assets', 'assets', 'styles', 'materialdesignicons-local.css');

function ensureDir(d) {
  if (!fs.existsSync(d)) fs.mkdirSync(d, { recursive: true });
}

ensureDir(outFontsDir);

// Copy known font files
const fonts = [
  'materialdesignicons-webfont.eot',
  'materialdesignicons-webfont.woff2',
  'materialdesignicons-webfont.woff',
  'materialdesignicons-webfont.ttf'
];

for (const f of fonts) {
  const src = path.join(nodeFontsDir, f);
  const dst = path.join(outFontsDir, f);
  try {
    if (fs.existsSync(src)) {
      fs.copyFileSync(src, dst);
      console.log('copied', f);
    } else {
      console.warn('missing source font', src);
    }
  } catch (e) {
    console.error('failed to copy', src, e.message);
  }
}

// Generate local CSS by reading the node module CSS and rewriting font URLs to /fonts/
try {
  if (fs.existsSync(nodeCss)) {
    let css = fs.readFileSync(nodeCss, 'utf8');
    // Replace relative font paths (../fonts/...) with absolute /fonts/
    css = css.replace(/url\((?:"|')?\.\.\/fonts\/(materialdesignicons-[^"')]+)(?:"|')?\)/g, "url('/fonts/$1')");
    // Also replace any other fonts path occurrences
    css = css.replace(/url\((?:"|')?fonts\/(materialdesignicons-[^"')]+)(?:"|')?\)/g, "url('/fonts/$1')");
    ensureDir(path.dirname(outCss));
    fs.writeFileSync(outCss, css, 'utf8');
    console.log('wrote', outCss);
  } else {
    console.warn('source CSS not found, creating minimal @font-face fallback');
    const fallback = "@font-face{font-family:Material Design Icons;src:url('/fonts/materialdesignicons-webfont.eot');src:url('/fonts/materialdesignicons-webfont.eot?#iefix') format('embedded-opentype'),url('/fonts/materialdesignicons-webfont.woff2') format('woff2'),url('/fonts/materialdesignicons-webfont.woff') format('woff'),url('/fonts/materialdesignicons-webfont.ttf') format('truetype');font-weight:400;font-style:normal;}";
    fs.writeFileSync(outCss, fallback, 'utf8');
  }
} catch (e) {
  console.error('error generating local CSS', e.message);
}

// Done
console.log('mdi font copy complete');
