import { OrbitControls } from '@react-three/drei'
import { useFrame } from '@react-three/fiber'
import * as React from 'react'
import * as THREE from 'three'
import fragmentShader from './shaders/frag-sphere.fs'
import vertexShader from './shaders/vert-sphere.vs'

const App: React.FC = () => {
  const ref = React.useRef<THREE.RawShaderMaterial>()

  useFrame(({ size: { width, height }, viewport: { dpr } }) => {
    const w = width * dpr
    const h = height * dpr

    ref.current?.uniforms.iResolution.value.copy(new THREE.Vector3(w, h, w / h))
  })

  return (
    <React.Suspense key={Math.random()} fallback={null}>
      <OrbitControls enableDamping makeDefault />

      <color args={[0x000000]} attach="background" />

      <mesh>
        <sphereBufferGeometry args={[0.5, 120, 120]} />
        <rawShaderMaterial
          uniforms={THREE.UniformsUtils.merge([
            { iResolution: new THREE.Uniform(new THREE.Vector3()) }
          ])}
          {...{ ref, fragmentShader, vertexShader }}
        />
      </mesh>
    </React.Suspense>
  )
}

export default App
