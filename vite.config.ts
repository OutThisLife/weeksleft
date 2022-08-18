import react from '@vitejs/plugin-react'
import { resolve } from 'path'
import { defineConfig } from 'vite'
import { glslify } from './plugins'

export default defineConfig({
  base: '/',
  build: { outDir: resolve(__dirname, 'dist') },
  plugins: [glslify(), react()],
  publicDir: resolve(__dirname, 'public'),
  resolve: {
    alias: {
      '~': resolve(__dirname, 'src')
    }
  },
  root: resolve(__dirname, 'src')
})
