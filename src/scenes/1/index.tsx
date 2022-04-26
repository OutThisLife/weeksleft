import { useAspect } from '@react-three/drei'
import type { RawShaderMaterialProps } from '@react-three/fiber'
import { useFrame } from '@react-three/fiber'
import * as React from 'react'
import * as THREE from 'three'
import fragmentShader from './frag.fs'
import vertexShader from './vert.vs'

export default function Index() {
  const scale = useAspect(1920, 1080)

  const ref = React.useRef<
    THREE.Mesh<THREE.BufferGeometry, THREE.RawShaderMaterial>
  >(null!)

  const material = React.useMemo<RawShaderMaterialProps>(
    () => ({
      fragmentShader,
      side: THREE.DoubleSide,
      uniforms: {
        iFrame: new THREE.Uniform(0),
        iMouse: new THREE.Uniform(new THREE.Vector2()),
        iResolution: new THREE.Uniform(new THREE.Vector4()),
        iTime: new THREE.Uniform(0)
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
      viewport: { aspect, dpr }
    }) => {
      const m = ref.current?.material

      if (m instanceof THREE.RawShaderMaterial) {
        const w = width * dpr
        const h = height * dpr

        m.uniforms.iFrame.value = clock.getDelta()
        m.uniforms.iMouse.value.copy(new THREE.Vector2(x, y))
        m.uniforms.iResolution.value.copy(new THREE.Vector4(w, h, aspect, dpr))
        m.uniforms.iTime.value = clock.getElapsedTime()

        m.uniformsNeedUpdate = true
      }
    }
  )

  return (
    <group position={[0, 0, 4]} {...{ scale }}>
      <mesh key={Math.random()} {...{ ref }}>
        <planeBufferGeometry />
        <rawShaderMaterial {...material} />
      </mesh>
    </group>
  )
}
