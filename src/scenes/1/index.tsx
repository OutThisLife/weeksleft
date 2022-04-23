import { useAspect } from '@react-three/drei'
import * as React from 'react'
import * as THREE from 'three'
import type { FrameCB } from '~/components'
import { Scene, Shader } from '~/components'
import fragmentShader from './frag.fs'
import vertexShader from './vert.vs'

export default function Index() {
  const scale = useAspect(1920, 1080)
  const [st, toggle] = React.useState(() => false)

  const camera = React.useMemo(
    () => new THREE.PerspectiveCamera(75, 1, 0.01, 1e3),
    []
  )

  const material = React.useMemo(
    () => ({
      fragmentShader,
      transparent: true,
      uniforms: { uProgress: new THREE.Uniform(0) },
      vertexShader
    }),
    []
  )

  const onFrame: FrameCB = e => {
    if (e.el?.material instanceof THREE.RawShaderMaterial) {
      e.el.material.uniforms.uProgress.value = THREE.MathUtils.lerp(
        e.el.material.uniforms.uProgress.value,
        +st,
        0.1
      )

      e.el.material.uniformsNeedUpdate = true
    }
  }

  return (
    <>
      <Scene position={[0, 0, 1]} {...{ camera, scale }}>
        <Shader key={Math.random()} {...{ material, onFrame }}>
          <sphereBufferGeometry args={[1, 100, 100]} />
        </Shader>
      </Scene>

      <mesh onPointerDown={() => toggle(s => !s)} position={[-1, 1, 2]}>
        <boxGeometry />
        <meshNormalMaterial wireframe={st} />
      </mesh>
    </>
  )
}
