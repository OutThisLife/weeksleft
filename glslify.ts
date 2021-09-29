import { readFileSync } from 'fs'
import glResolve from 'glsl-resolve'
import glString from 'glsl-token-string'
import glTokenize from 'glsl-tokenizer'
import { compile } from 'glslify'
import { dirname } from 'path'

const glslifyImport = (file, src, opts, done) => {
  const basedir = dirname(file)

  let total = 0

  const tokens = glTokenize(src).map(k => {
    if (k.type !== 'preprocessor') {
      return k
    }

    const [, ...matches] =
      /#pragma glslify:\s?import\s(\{.*\})?.*?(['".\/A-z]+)$/gm.exec(k.data) ??
      []

    if (!matches.length) {
      return k
    }

    try {
      total++

      const modules = matches.find(v => v?.includes('{'))
      const file = matches
        .find(v => v?.includes('.'))
        .split(/'|"/)
        .join('')

      if (typeof done === 'function') {
        const resolved = glResolve.sync(file, { basedir })

        glslifyImport(
          resolved,
          readFileSync(resolved, 'utf8'),
          opts,
          (err, contents) => {
            if (err) {
              throw err
            }

            k.data = contents

            if (--total) {
              return k
            }

            done(null, glString(tokens))
          }
        )
      } else {
        total--

        const resolved = glResolve.sync(file, { basedir })
        let contents = glslifyImport(
          resolved,
          readFileSync(resolved, 'utf8'),
          opts,
          null
        )

        k.data = contents

        if (modules) {
          k.contents = modules
            .trim()
            .replace(/(^{\s?|\s}$)/g, '')
            .split(',')
            .reduce((acc, m) => {
              const rgx = new RegExp(`(.*(?<=${m})[\\s\\S]*?\})`, 'gm')

              const [, func] = rgx.exec(contents) ?? []
              acc.push(func)
              return acc
            }, [])
            .filter(v => v)
            .join('\n')
        }
      }
    } catch (err) {
      if (typeof done === 'function') {
        done(err)
      }
    } finally {
      return k
    }
  })

  if (!total) {
    return typeof done === 'function' ? done(null, src) : glString(tokens)
  }
}

export default () => ({
  name: 'glslify-plugin',

  transform(code, id) {
    if (!/(\.(fs|vs|frag|vert|glsl)|\?glslify)(\?raw)?$/i.test(id)) {
      return
    }

    return {
      code: `export default ${JSON.stringify(
        compile(code.replace(/^#version 330/, '#version 300 es'), {
          basedir: dirname(id.split('?').shift()),
          transform: [glslifyImport, 'glslify-hex']
        })
      )}`,
      map: null
    }
  }
})
