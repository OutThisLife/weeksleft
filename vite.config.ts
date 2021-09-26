import reactRefreshPlugin from '@vitejs/plugin-react-refresh'
import { resolve } from 'path'
import { defineConfig } from 'vite'
import glslify from './glslify'

export default defineConfig({
  base: '/',
  build: {
    outDir: resolve(__dirname, 'dist')
  },
  plugins: [glslify(), reactRefreshPlugin()],
  root: resolve(__dirname, 'src')
})
