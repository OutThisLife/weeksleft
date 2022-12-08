/* eslint-disable @typescript-eslint/ban-ts-comment */
import { useAspect } from '@react-three/drei'
import type { RawShaderMaterialProps } from '@react-three/fiber'
import { useFrame } from '@react-three/fiber'
import { useControls } from 'leva'
import * as React from 'react'
import * as THREE from 'three'
import fragmentShader from './frag.fs'
import vertexShader from './vert.vs'

export default function Scene() {
  const controls = useControls({
    seed: { max: 1, min: 0, step: 0.01, value: Math.random() }
  })

  const scale = useAspect(1920, 1080)
  const ref = React.useRef<THREE.RawShaderMaterial>(null!)

  const material = React.useMemo<RawShaderMaterialProps>(
    () => ({
      depthTest: false,
      fragmentShader,
      side: THREE.FrontSide,
      transparent: false,
      uniforms: {
        uFrame: new THREE.Uniform(1),
        uMouse: new THREE.Uniform(new THREE.Vector2(1, 1)),
        uResolution: new THREE.Uniform(new THREE.Vector4(1, 1, 1, 2)),
        uSeed: new THREE.Uniform(Math.random()),
        uTime: new THREE.Uniform(0)
      },
      vertexShader
    }),
    []
  )

  useFrame(
    ({
      clock,
      mouse: { x = 0, y = 0 },
      size: { height, width },
      viewport: { dpr }
    }) => {
      const w = width * dpr
      const h = height * dpr

      const m = ref.current

      if (m instanceof THREE.RawShaderMaterial) {
        m.uniforms.uFrame.value = clock.getDelta()
        m.uniforms.uMouse.value.copy(new THREE.Vector2(x / dpr, y / dpr))
        m.uniforms.uResolution.value.copy(new THREE.Vector4(w, h, w / h, dpr))
        m.uniforms.uTime.value = clock.getElapsedTime()
        m.uniforms.uSeed.value = controls.seed
      }
    }
  )

  return (
    <mesh key={Math.random()} {...{ scale }}>
      <planeBufferGeometry />
      <rawShaderMaterial {...{ ref, ...material }} />
    </mesh>
  )
}
