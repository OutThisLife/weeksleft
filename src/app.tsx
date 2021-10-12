import { Billboard, OrbitControls } from '@react-three/drei'
import type { ShaderMaterialProps } from '@react-three/fiber'
import { useFrame, useLoader } from '@react-three/fiber'
import * as React from 'react'
import * as THREE from 'three'
import fragmentShader from './shaders/frag.fs'
import vertexShader from './shaders/vert.vs'

const Stars: React.FC = () => {
  const ref = React.useRef<any>()

  const positions = React.useMemo(
    () =>
      new Float32Array(
        Array.from([...Array(1e4)]).flatMap(() => [
          (Math.random() - 0.5) * 20,
          (Math.random() - 0.5) * 20,
          -Math.random() * 64
        ])
      ),
    []
  )

  const colours = React.useMemo(
    () =>
      new Float32Array(
        Array.from([...Array(1e4)]).flatMap(() => [1, 1, 1, 1 + Math.random()])
      ),
    []
  )

  const data = React.useMemo<ShaderMaterialProps>(
    () => ({
      fragmentShader: `
        uniform vec4 color;
        
        void main() {
          vec3 col = vec3(1.);
          pc_fragColor = vec4(clamp(col, 0., 1.), color.a);
        }
      `,
      side: THREE.DoubleSide,
      uniforms: {
        iResolution: new THREE.Uniform(new THREE.Vector3(0, 0, 1))
      },
      vertexShader: `
        uniform vec3 iResolution;
        
        void main() {
          gl_PointSize = 0.1 / iResolution.z;
          gl_Position = projectionMatrix * modelViewMatrix * vec4(position, 1.);
        }
      `
    }),
    []
  )

  useFrame(({ size: { height, width }, viewport: { dpr } }) => {
    const $m = ref.current as THREE.Mesh

    if ($m instanceof THREE.Mesh) {
      const $s = $m.material

      if ($s instanceof THREE.ShaderMaterial) {
        const w = width * dpr
        const h = height * dpr

        $s.uniforms.iResolution.value.copy(new THREE.Vector3(w, h, w / h))
      }
    }
  })

  return (
    <Billboard>
      <points {...{ ref }}>
        <sphereBufferGeometry args={[90, 64, 64]}>
          <bufferAttribute
            array={positions}
            attachObject={['attributes', 'position']}
            count={positions.length / 3}
            itemSize={3}
          />

          <bufferAttribute
            array={colours}
            attachObject={['attributes', 'color']}
            count={colours.length / 4}
            itemSize={4}
          />
        </sphereBufferGeometry>

        <shaderMaterial {...data} />
      </points>
    </Billboard>
  )
}

const Earth: React.FC = () => {
  const ref = React.useRef<any>()

  const difTex = useLoader(THREE.TextureLoader, '/earth.jpg')
  const bumpTex = useLoader(THREE.TextureLoader, '/earth-bump.jpg')
  const speTex = useLoader(THREE.TextureLoader, '/earth-spe.png')
  const cloudTex = useLoader(THREE.TextureLoader, '/earth-clouds.png')

  const data = React.useMemo<ShaderMaterialProps>(
    () => ({
      fragmentShader,
      side: THREE.DoubleSide,
      uniforms: {
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
    <>
      <mesh>
        <sphereBufferGeometry args={[0.5, 120, 120]} />
        <meshPhongMaterial
          bumpMap={bumpTex}
          bumpScale={0.05}
          map={difTex}
          side={THREE.DoubleSide}
          specularMap={speTex}
        />
      </mesh>

      <mesh>
        <sphereBufferGeometry args={[0.5001, 120, 120]} />
        <meshPhongMaterial map={cloudTex} transparent />
      </mesh>

      <mesh {...{ ref }}>
        <sphereBufferGeometry args={[0.54, 120, 120]} />
        <rawShaderMaterial transparent {...data} />
      </mesh>
    </>
  )
}

const App: React.FC = () => (
  <React.Suspense key={Math.random()} fallback={null}>
    <OrbitControls autoRotateSpeed={0.3} enableDamping />

    <ambientLight color={0x888888} />
    <directionalLight color={0xffffff} position={[5, 3, 5]} />

    <Earth />
    <Stars />
  </React.Suspense>
)

export default App
