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
      transparent: true,
      uniforms: {
        cameraProjectionMatrixInverse: new THREE.Uniform(new THREE.Matrix4()),
        cameraWorldMatrix: new THREE.Uniform(new THREE.Matrix4()),
        iFrame: new THREE.Uniform(1),
        iMouse: new THREE.Uniform(new THREE.Vector2(0, 0)),
        iResolution: new THREE.Uniform(new THREE.Vector3(0, 0, 1)),
        iTime: new THREE.Uniform(0)
      },
      vertexShader
    }),
    []
  )

  useFrame(({ camera, clock, mouse, size, viewport: { dpr } }) => {
    const $m = ref.current as THREE.Mesh

    if ($m instanceof THREE.Mesh) {
      const $s = $m.material

      if ($s instanceof THREE.RawShaderMaterial) {
        $s.uniforms.iFrame.value += 1
        $s.uniforms.iTime.value = clock.getElapsedTime()
        $s.uniforms.iMouse.value = new THREE.Vector2(mouse.x, mouse.y)

        $s.uniforms.iResolution.value = new THREE.Vector3(
          size.width * dpr,
          size.height * dpr,
          dpr
        )

        $s.uniforms.cameraWorldMatrix.value.copy(camera.matrixWorld)
        $s.uniforms.cameraProjectionMatrixInverse.value.copy(
          camera.projectionMatrixInverse
        )
      }
    }
  })

  return (
    <React.Suspense key={Math.random()} fallback={null}>
      <OrbitControls autoRotateSpeed={0.3} enableDamping />

      <mesh {...{ ref }}>
        <planeBufferGeometry args={[2, 2]} />
        <rawShaderMaterial {...data} />
      </mesh>
    </React.Suspense>
  )
}

export default App
