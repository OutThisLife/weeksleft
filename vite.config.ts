import reactRefreshPlugin from '@vitejs/plugin-react-refresh'
import { resolve } from 'path'
import { defineConfig } from 'vite'
import glslify from './glslify'

export default defineConfig({
  base: '/',
  build: {
    outDir: resolve(__dirname, 'dist')
  },
  plugins: [reactRefreshPlugin(), glslify()],
  root: resolve(__dirname, 'src')
})
