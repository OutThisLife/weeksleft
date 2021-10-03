import { dirname } from 'path'

export default async (code: string, id: string): Promise<string> => {
  const { compile } = await import('glslify')
  const { default: transforms } = await import('./transforms')
  const { default: minify } = await import('./minify')

  return `export default ${JSON.stringify(
    minify(
      compile(code, {
        basedir: dirname(id.split('?').shift()),
        transform: await Promise.all(transforms.map(async t => t))
      })
    )
  )}`
}
