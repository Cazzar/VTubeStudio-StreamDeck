import js from '@eslint/js'
import vue from 'eslint-plugin-vue'

export default [
  js.configs.recommended,
  ...vue.configs['flat/recommended'],
  {
    languageOptions: {
      globals: { window: 'readonly', console: 'readonly', WebSocket: 'readonly', queueMicrotask: 'readonly' },
    },
  },
  { ignores: ['dist/', '.vite-entries/', '*.html'] },
]
