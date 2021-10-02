import { readFileSync } from 'fs'
import glResolve from 'glsl-resolve'
import glString from 'glsl-token-string'
import glTokenize from 'glsl-tokenizer'
import { compile } from 'glslify'
import { basename, dirname, resolve } from 'path'
import type { Plugin } from 'vite'

const isGLSL = (s: string): boolean =>
  /(\.(fs|vs|frag|vert|glsl)|\?glslify)(\?raw)?$/i.test(s)

export default (): Plugin => ({
  async handleHotUpdate({ file, modules, server }) {
    if (isGLSL(file)) {
      const [m] = server.moduleGraph.getModulesByFile(
        resolve(__dirname, 'src/app.tsx')
      )

      const entry = [...m.importedModules.values()].find(i => isGLSL(i.file))

      if (
        entry?.file !== file &&
        new RegExp(`${basename(file)}`, 'gm').test(entry?.transformResult?.code)
      ) {
        return [entry]
      }

      return modules
    }

    return []
  },

  name: 'glslify-plugin',

  async transform(code, id) {
    if (!isGLSL(id)) {
      return { code }
    }

    const glslifyImport = (
      file: string,
      src: string,
      opts: Record<string, any>,
      done: null | CallableFunction
    ): string => {
      const basedir = dirname(file)

      let total = 0

      const tokens = (glTokenize(src) as GLSLToken[]).map(k => {
        if (k.type !== 'preprocessor') {
          return k
        }

        const [, ...matches] =
          /#pragma glslify:\s?import\s(\{.*\})?.*?(['"./A-z]+)$/gm.exec(
            k.data
          ) ?? []

        if (!matches.length) {
          return k
        }

        try {
          total++

          const modules = matches.find(v => v?.includes('{'))

          const f = matches
            .find(v => v?.includes('.'))
            .split(/'|"/)
            .join('')

          const prefix = `\n// ${f}\n`

          if (typeof done === 'function') {
            const resolved = glResolve.sync(f, { basedir })

            glslifyImport(
              resolved,
              readFileSync(resolved, 'utf8'),
              opts,
              (err, contents) => {
                if (err) {
                  throw err
                }

                k.data = `${prefix}${contents}`

                if (--total) {
                  return k
                }

                done(null, glString(tokens))

                return null
              }
            )
          } else {
            total--

            const resolved = glResolve.sync(f, { basedir })

            const contents = glslifyImport(
              resolved,
              readFileSync(resolved, 'utf8'),
              opts,
              null
            )

            if (modules) {
              k.data = modules
                .trim()
                .replace(/(^{\s?|\s}$)/g, '')
                .split(',')
                .reduce((acc, m) => {
                  const rgx = new RegExp(`(.*(?<=${m})[\\s\\S]*?})`, 'gm')
                  const [, func] = rgx.exec(contents) ?? []

                  return [...acc, `${prefix}${func}`]
                }, [])
                .filter(v => v)
                .join('\n')
            } else {
              k.data = `${prefix}${contents}`
            }
          }
        } catch (err) {
          if (typeof done === 'function') {
            done(err)
          }
        }

        return k
      })

      return typeof done === 'function' ? done(null, src) : glString(tokens)
    }

    const out = compile(code, {
      basedir: dirname(id.split('?').shift()),
      transform: [glslifyImport, 'glslify-hex']
    })

    return {
      code: `export default ${JSON.stringify(out)}`,
      map: null
    }
  }
})

interface GLSLToken {
  type: string
  data: string
}
