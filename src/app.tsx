import { Billboard, OrbitControls } from '@react-three/drei'
import type { ShaderMaterialProps } from '@react-three/fiber'
import { useFrame, useLoader } from '@react-three/fiber'
import * as React from 'react'
import * as THREE from 'three'
import fragmentShader from './shaders/frag.fs'
import vertexShader from './shaders/vert.vs'

const Earth: React.FC = () => {
  const ref = React.useRef<any>()

  const difTex = useLoader(THREE.TextureLoader, '/earth.jpg')
  const bumpTex = useLoader(THREE.TextureLoader, '/earth-bump.jpg')
  const speTex = useLoader(THREE.TextureLoader, '/earth-spe.jpg')
  const cloudTex = useLoader(THREE.TextureLoader, '/earth-clouds.jpg')

  const data = React.useMemo<ShaderMaterialProps>(
    () => ({
      fragmentShader,
      side: THREE.DoubleSide,
      uniforms: {
        bumpTex: new THREE.Uniform(bumpTex),
        cameraProjectionMatrixInverse: new THREE.Uniform(new THREE.Matrix4()),
        cameraWorldMatrix: new THREE.Uniform(new THREE.Matrix4()),
        cloudTex: new THREE.Uniform(cloudTex),
        difTex: new THREE.Uniform(difTex),
        iFrame: new THREE.Uniform(1),
        iGlobalTime: new THREE.Uniform(0),
        iMouse: new THREE.Uniform(new THREE.Vector2(0, 0)),
        iResolution: new THREE.Uniform(new THREE.Vector3(0, 0, 1)),
        speTex: new THREE.Uniform(speTex)
      },
      vertexShader
    }),
    []
  )

  useFrame(
    ({ camera, clock, mouse, size: { height, width }, viewport: { dpr } }) => {
      const $m = ref.current as THREE.Mesh

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
          $s.uniforms.cameraProjectionMatrixInverse.value.copy(
            camera.projectionMatrixInverse
          )
        }
      }
    }
  )

  return (
    <mesh {...{ ref }}>
      <sphereBufferGeometry args={[15, 32, 100]} />
      <rawShaderMaterial {...data} />
    </mesh>
  )
}

const Stars: React.FC = () => {
  const positions = React.useMemo(
    () =>
      new Float32Array(
        Array.from([...Array(1e4)]).flatMap(() => [
          (Math.random() - 0.5) * 1e2,
          (Math.random() - 0.5) * 1e2,
          -Math.random()
        ])
      ),
    []
  )

  return (
    <Billboard>
      <points>
        <bufferGeometry>
          <bufferAttribute
            array={positions}
            attachObject={['attributes', 'position']}
            count={positions.length / 3}
            itemSize={3}
          />
        </bufferGeometry>

        <pointsMaterial size={0.1} sizeAttenuation />
      </points>
    </Billboard>
  )
}

const App: React.FC = () => (
  <React.Suspense key={Math.random()} fallback={null}>
    <OrbitControls
      autoRotate
      enableDamping
      enableZoom={false}
      maxPolarAngle={Math.PI / 2.1}
    />

    <Earth />
    <Stars />
  </React.Suspense>
)

export default App
