import * as React from 'react'
import * as THREE from 'three'
import { Shader } from '~/components'
import fragmentShader from './frag.fs'
import vertexShader from './vert.vs'

export default function Sphere() {
  const material = React.useMemo(
    () => ({
      fragmentShader,
      uniforms: { tCube: new THREE.Uniform(new THREE.CubeTexture()) },
      vertexShader
    }),
    []
  )

  return (
    <Shader position={[0.5, 0.4, -1]} {...{ material }}>
      <sphereBufferGeometry args={[0.7, 100, 100]} />
    </Shader>
  )
}
