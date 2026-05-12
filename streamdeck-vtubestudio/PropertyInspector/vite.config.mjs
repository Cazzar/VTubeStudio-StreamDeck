import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { resolve, dirname } from 'path'
import { readFileSync, writeFileSync } from 'fs'
import { fileURLToPath } from 'url'

const __dirname = dirname(fileURLToPath(import.meta.url))

const pages = [
  'DialMove',
  'DialRotate',
  'DialZoom',
  'ModelChange',
  'TriggerHotkey',
  'MoveModel',
  'ScaleModel',
  'TempZoom',
  'NoSettings',
  'ExpressionToggle',
]

// Generate per-page HTML entry files from the single template.
// These are gitignored build artifacts — regenerated on every vite invocation.
const template = readFileSync(resolve(__dirname, 'src/page.html'), 'utf-8')
for (const page of pages) {
  writeFileSync(
    resolve(__dirname, `${page}.html`),
    template
      .replace(/%PAGE_TITLE%/g, page)
      .replace(/%PAGE_ENTRY%/g, `/src/pages/${page}/main.js`),
  )
}

export default defineConfig({
  plugins: [vue()],
  base: './',
  build: {
    target: 'esnext',
    rollupOptions: {
      input: Object.fromEntries(
        pages.map(page => [page, resolve(__dirname, `${page}.html`)])
      ),
    },
  },
})
