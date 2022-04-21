import type { RootState } from '@react-three/fiber'
import { useFrame } from '@react-three/fiber'
import { EffectComposer } from '@react-three/postprocessing'
import * as React from 'react'
import * as THREE from 'three'
import { DotsEffect } from './effects'
import Shader from './shader'
import bgFrag from './shaders/1/frag.fs'
import bgVert from './shaders/1/vert.vs'
import sphereFrag from './shaders/2/frag.fs'
import sphereVert from './shaders/2/vert.vs'

function BG() {
  const material = React.useMemo(
    () => ({ fragmentShader: bgFrag, vertexShader: bgVert }),
    []
  )

  return (
    <Shader {...{ material }}>
      <sphereBufferGeometry args={[1, 100, 100]} />
    </Shader>
  )
}

function Sphere() {
  const ref = React.useRef<THREE.CubeCamera>(null)

  const material = React.useMemo(
    () => ({
      fragmentShader: sphereFrag,
      uniforms: { tCube: new THREE.Uniform(new THREE.CubeTexture()) },
      vertexShader: sphereVert
    }),
    []
  )

  const target = React.useMemo(
    () =>
      new THREE.WebGLCubeRenderTarget(512, {
        encoding: THREE.sRGBEncoding,
        format: THREE.RGBAFormat,
        generateMipmaps: true,
        minFilter: THREE.LinearMipMapLinearFilter
      }),
    []
  )

  const onFrame: FrameCB = ({ el, gl, scene }) => {
    const cam = ref.current

    if (
      el?.material instanceof THREE.RawShaderMaterial &&
      cam instanceof THREE.CubeCamera
    ) {
      el.visible = false
      cam.update(gl, scene)
      el.visible = true

      el.material.uniforms.tCube.value = cam.renderTarget.texture
    }
  }

  return (
    <>
      <cubeCamera args={[0.1, 10, target]} {...{ ref }} />

      <Shader position={[0.6, 0.2, 0]} {...{ material, onFrame }}>
        <sphereBufferGeometry args={[0.5, 100, 100]} />
      </Shader>
    </>
  )
}

export default function App() {
  const cam = React.useMemo(() => new THREE.Vector2(), [])
  const dir = React.useMemo(() => new THREE.Vector2(), [])

  useFrame(({ camera, mouse }) => {
    dir.subVectors(mouse, cam).multiplyScalar(0.01)
    cam.addVectors(cam, dir)

    camera.position.x = cam.x * -0.6
    camera.position.x = cam.y * -0.3
    camera.lookAt(new THREE.Vector3(0, 0, 0))
  })

  return (
    <React.Suspense key={Math.random()} fallback={null}>
      <color args={[0x000000]} attach="background" />

      <group>
        <Sphere />
        <BG />
      </group>

      <EffectComposer>
        <DotsEffect />
      </EffectComposer>
    </React.Suspense>
  )
}

export type FrameCB = (e: RootState & { el: THREE.Mesh | null }) => void
