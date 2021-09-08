import reactRefresh from '@vitejs/plugin-react-refresh'
import { resolve } from 'path'
import { defineConfig } from 'vite'
import tsconfigPaths from 'vite-tsconfig-paths'

export default defineConfig({
  base: '/',
  build: {
    outDir: resolve(__dirname, 'dist')
  },
  plugins: [reactRefresh(), tsconfigPaths()],
  root: resolve(__dirname, 'src')
})