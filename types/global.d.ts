declare module 'postprocessing'

declare module '*?glslify'
declare module '*?gl'
declare module '*.vs'
declare module '*.fs'

declare module 'glslify' {
  interface GLSL<T extends string> {
    (first: string[], ...interpolations: any[]): T
  }

  export default glsl as GLSL
}
