import { OrbitControls } from '@react-three/drei'
import { useFrame } from '@react-three/fiber'
import * as React from 'react'
import * as THREE from 'three'
import fragmentShader from './shaders/frag-sphere.fs'
import vertexShader from './shaders/vert-sphere.vs'

const App: React.FC = () => {
  const ref = React.useRef<THREE.RawShaderMaterial>()

  useFrame(({ mouse, size: { width, height }, viewport: { dpr }, clock }) => {
    const w = width
    const h = height

    if (ref.current) {
      ref.current.uniforms.iResolution.value.copy(
        new THREE.Vector3(w, h, w / h)
      )

      ref.current.uniforms.iMouse.value.copy(mouse)
      ref.current.uniforms.iTime.value = clock.getElapsedTime()
    }
  })

  return (
    <React.Suspense key={Math.random()} fallback={null}>
      <OrbitControls enableDamping makeDefault />

      <color args={[0x222222]} attach="background" />

      <mesh>
        <planeBufferGeometry args={[2, 2]} />
        <rawShaderMaterial
          uniforms={THREE.UniformsUtils.merge([
            { iResolution: new THREE.Uniform(new THREE.Vector3(1, 1, 1)) },
            { iTime: new THREE.Uniform(0) },
            { iMouse: new THREE.Uniform(new THREE.Vector2(1, 1)) }
          ])}
          {...{ ref, fragmentShader, vertexShader }}
        />
      </mesh>
    </React.Suspense>
  )
}

export default App
