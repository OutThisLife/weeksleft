import { readFileSync } from 'fs'
import { dirname } from 'path'

export default async () => {
  const { default: glResolve } = await import('glsl-resolve')
  const { default: glTokenize } = await import('glsl-tokenizer')
  const { default: glString } = await import('glsl-token-string')

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
        /#pragma glslify:\s?import\s(\{.*\})?.*?(['"./A-z]+)$/gm.exec(k.data) ??
        []

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
              .replace(/(^{\s?|\s}$)/g, '')
              .split(',')
              .map(m => m.trim())
              .reduce((acc, m) => {
                const rgx = new RegExp(`(.*(?<=${m})[\\s\\S]*?})`, 'gm')
                const [, ...refs] = rgx.exec(contents) ?? []

                return [...acc, `${prefix}${refs.join('\n')}`]
              }, [])
              .filter(v => v)
              .join('\n')
          } else {
            k.data = `${prefix}${contents}`
          }
        }
      } catch (err) {
        console.error(err)

        if (typeof done === 'function') {
          done(err)
        }
      }

      return k
    })

    return typeof done === 'function' ? done(null, src) : glString(tokens)
  }

  return glslifyImport
}

interface GLSLToken {
  type: string
  data: string
}
