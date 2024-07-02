import { defineConfig } from '@rsbuild/core'
import { pluginVue } from '@rsbuild/plugin-vue'

export default defineConfig({
  plugins: [pluginVue()],
  source: {
      entry: {
          'DialMove': './src/pages/DialMove/main.js',
          'DialRotate': './src/pages/DialRotate/main.js',
          'DialZoom': './src/pages/DialZoom/main.js',
          'ModelChange': './src/pages/ModelChange/main.js',
          'TriggerHotkey': './src/pages/TriggerHotkey/main.js',
          'MoveModel': './src/pages/MoveModel/main.js',
          'ScaleModel': './src/pages/ScaleModel/main.js',
          'TempZoom': './src/pages/TempZoom/main.js',
          'NoSettings': './src/pages/NoSettings/main.js',
      }
  }
})