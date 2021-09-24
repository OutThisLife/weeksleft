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

    const [, str] = /#pragma glslify:\s*import\(([^\)]+)\)/.exec(k.data) ?? []

    if (!str) {
      return k
    }

    try {
      total++

      const v = str.trim().replace(/^'|'$/g, '').replace(/^"|"$/g, '')

      if (typeof done === 'function') {
        const resolved = glResolve.sync(v, { basedir })

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

        const resolved = glResolve.sync(v, { basedir })

        k.data = glslifyImport(
          resolved,
          readFileSync(resolved, 'utf8'),
          opts,
          null
        )
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
        compile(code, {
          basedir: dirname(id.split('?').shift()),
          transform: [glslifyImport, 'glslify-hex']
        })
      )}`,
      map: null
    }
  }
})
