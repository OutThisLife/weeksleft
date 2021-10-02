import react from '@vitejs/plugin-react'
import { resolve } from 'path'
import { defineConfig } from 'vite'
import glslify from './plugin-glslify'

export default defineConfig({
  base: '/',
  build: {
    outDir: resolve(__dirname, 'dist')
  },
  plugins: [react(), glslify()],
  root: resolve(__dirname, 'src')
})
