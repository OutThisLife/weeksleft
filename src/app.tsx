import { OrbitControls } from '@react-three/drei'
import type { ShaderMaterialProps } from '@react-three/fiber'
import { useFrame } from '@react-three/fiber'
import * as React from 'react'
import * as THREE from 'three'
import fragmentShader from './shaders/frag.fs'
import vertexShader from './shaders/vert.vs'

const App: React.FC = () => {
  const ref = React.useRef<any>()

  const data = React.useMemo<ShaderMaterialProps>(
    () => ({
      fragmentShader,
      uniforms: {
        cameraProjectionMatrix: new THREE.Uniform(new THREE.Matrix4()),
        cameraProjectionMatrixInverse: new THREE.Uniform(new THREE.Matrix4()),
        cameraWorldMatrix: new THREE.Uniform(new THREE.Matrix4()),
        iFrame: new THREE.Uniform(1),
        iGlobalTime: new THREE.Uniform(0),
        iMouse: new THREE.Uniform(new THREE.Vector2(0, 0)),
        iResolution: new THREE.Uniform(new THREE.Vector3(0, 0, 1))
      },
      vertexShader
    }),
    []
  )

  let i = 0

  useFrame(
    ({ camera, clock, mouse, size: { height, width }, viewport: { dpr } }) => {
      const $m = ref.current as THREE.Mesh

      if (!i) {
        i++
      }

      if ($m instanceof THREE.Mesh) {
        const $s = $m.material

        if ($s instanceof THREE.RawShaderMaterial) {
          const w = width * dpr
          const h = height * dpr

          $s.uniforms.iFrame.value += 1
          $s.uniforms.iGlobalTime.value = clock.getElapsedTime()
          $s.uniforms.iMouse.value.copy(mouse)
          $s.uniforms.iResolution.value.copy(new THREE.Vector3(w, h, w / h))

          $s.uniforms.cameraWorldMatrix.value.copy(camera.matrixWorld)

          $s.uniforms.cameraProjectionMatrix.value.copy(camera.projectionMatrix)

          $s.uniforms.cameraProjectionMatrixInverse.value.copy(
            camera.projectionMatrixInverse
          )
        }
      }
    }
  )

  return (
    <React.Suspense key={Math.random()} fallback={null}>
      <OrbitControls maxPolarAngle={Math.PI / 2.1} />

      <mesh frustumCulled={false} {...{ ref }}>
        <planeBufferGeometry args={[2, 2]} />
        <rawShaderMaterial {...data} />
      </mesh>
    </React.Suspense>
  )
}

export default App
