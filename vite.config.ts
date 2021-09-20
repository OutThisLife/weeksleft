import reactRefreshPlugin from '@vitejs/plugin-react-refresh'
import { compile } from 'glslify'
import { dirname, resolve } from 'path'
import { defineConfig } from 'vite'

const glslify = () => ({
  name: 'glslify-plugin',

  transform(code, id) {
    if (!/(\.(fs|vs|frag|vert|glsl)|\?glslify)(\?raw)?$/i.test(id)) {
      return
    }

    return {
      code: `export default ${JSON.stringify(
        compile(`${code.replace(/glslify :/gm, 'glslify:')}`, {
          basedir: dirname(id.split('?').shift()),
          transforms: []
        })
      )}`,
      map: null
    }
  }
})

export default defineConfig({
  base: '/',
  build: {
    outDir: resolve(__dirname, 'dist')
  },
  plugins: [reactRefreshPlugin(), glslify()],
  root: resolve(__dirname, 'src')
})
