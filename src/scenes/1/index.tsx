import { useAspect } from '@react-three/drei'
import type { RawShaderMaterialProps } from '@react-three/fiber'
import { useLoader } from '@react-three/fiber'
import * as React from 'react'
import * as THREE from 'three'
import { Shader } from '~/components'
import fragmentShader from './frag.fs'
import vertexShader from './vert.vs'

export default function Index() {
  const scale = useAspect(1920, 1080)

  const [tex0, tex1] = useLoader(THREE.TextureLoader, [
    '/flowers.jpg',
    '/flowers2.jpg'
  ])

  const material = React.useMemo<RawShaderMaterialProps>(
    () => ({
      depthTest: false,
      fragmentShader,
      uniforms: {
        tex0: new THREE.Uniform(tex0),
        tex1: new THREE.Uniform(tex1)
      },
      vertexShader
    }),
    [tex0, tex1]
  )

  return <Shader key={fragmentShader + vertexShader} {...{ material, scale }} />
}
