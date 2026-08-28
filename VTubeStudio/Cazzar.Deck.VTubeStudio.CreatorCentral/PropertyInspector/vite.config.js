import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { dirname, resolve } from 'node:path'
import { fileURLToPath } from 'node:url'

const here = dirname(fileURLToPath(import.meta.url))

export default defineConfig({
  plugins: [vue()],
  base: './',
  resolve: {
    alias: { '@shared': resolve(here, '../../Cazzar.Deck.VTubeStudio.Views/src') },
  },
  build: {
    target: 'esnext',
    outDir: resolve(here, '../../../dist/dev.cazzar.creatorcentral.vtubestudio/PropertyInspector'),
    emptyOutDir: true,
  },
})
