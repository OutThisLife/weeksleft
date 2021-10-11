import { basename, resolve } from 'path'
import type { Plugin } from 'vite'

const isShader = (s: string): boolean =>
  /(\.(fs|vs|frag|vert|glsl)|\?glslify)(\?raw)?$/i.test(s)

export default (): Plugin => ({
  async handleHotUpdate({ file, modules, server }) {
    if (isShader(file)) {
      const [m] = server.moduleGraph.getModulesByFile(
        resolve(__dirname, '../../src/app.tsx')
      )

      const entry = [...m.importedModules.values()].find(i => isShader(i.file))

      if (
        entry?.file !== file &&
        new RegExp(`${basename(file)}`, 'gm').test(entry?.transformResult?.code)
      ) {
        return [entry]
      }

      return modules
    }

    return null
  },

  name: 'glslify-plugin',

  async transform(code, id) {
    if (!isShader(id)) {
      return { code }
    }

    const { default: compile } = await import('./compile')

    return {
      code: await compile(code, id),
      map: null
    }
  }
})
