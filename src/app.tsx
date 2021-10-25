import { OrbitControls } from '@react-three/drei'
import { useFrame } from '@react-three/fiber'
import * as React from 'react'
import * as THREE from 'three'
import fragmentShader from './shaders/frag.fs'
import vertexShader from './shaders/vert.vs'

const App: React.FC = () => {
  const ref = React.useRef<THREE.RawShaderMaterial>()

  const props = React.useMemo(
    () => ({
      fragmentShader,
      ref,
      uniforms: {
        cameraProjectionMatrixInverse: new THREE.Uniform(new THREE.Matrix4()),
        cameraWorldMatrix: new THREE.Uniform(new THREE.Matrix4()),
        iFrame: new THREE.Uniform(1),
        iMouse: new THREE.Uniform(new THREE.Vector2(1, 1)),
        iResolution: new THREE.Uniform(new THREE.Vector3(1, 1, 1)),
        iTime: new THREE.Uniform(0)
      },
      vertexShader
    }),
    []
  )

  useFrame(
    ({ camera, clock, mouse, size: { height, width }, viewport: { dpr } }) => {
      if (ref.current) {
        const w = width * dpr
        const h = height * dpr

        ref.current.uniforms.iResolution.value.copy(
          new THREE.Vector3(w, h, w / h)
        )

        ref.current.uniforms.iMouse.value.copy(mouse)
        ref.current.uniforms.iTime.value = clock.getElapsedTime()

        ref.current.uniforms.cameraWorldMatrix.value.copy(camera.matrixWorld)
        ref.current.uniforms.cameraProjectionMatrixInverse.value.copy(
          camera.projectionMatrixInverse
        )
      }
    }
  )

  return (
    <React.Suspense key={Math.random()} fallback={null}>
      <OrbitControls enableDamping makeDefault />

      <color args={[0x222222]} attach="background" />

      <mesh>
        <planeBufferGeometry args={[2, 2]} />
        <rawShaderMaterial {...props} />
      </mesh>
    </React.Suspense>
  )
}

export default App
