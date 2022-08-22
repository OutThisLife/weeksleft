/* eslint-disable no-nested-ternary */
import { useAspect, useFBO } from '@react-three/drei'
import type { GroupProps, RawShaderMaterialProps } from '@react-three/fiber'
import { createPortal, useFrame } from '@react-three/fiber'
import * as React from 'react'
import * as THREE from 'three'
import type { OnFrame } from '~/components'
import { Shader } from '~/components'
import fragmentShader from './frag.fs'
import vertexShader from './vert.vs'

function Box(props: GroupProps) {
  const ref = React.useRef<THREE.Group>(null!)
  const [over, set] = React.useState(() => false)

  useFrame(() => {
    if (ref.current instanceof THREE.Group) {
      ref.current.rotation.x += 0.01 * (over ? 0.2 : 1)
      ref.current.rotation.z -= 0.005 * (over ? 0.2 : 1)
    }
  })

  return (
    <>
      <pointLight position={[5, 5, 5]} />
      <ambientLight intensity={0.3} />

      <group
        onPointerLeave={() => set(false)}
        onPointerOver={() => set(true)}
        {...{ ref, ...props }}
      >
        <mesh rotation={[Math.PI / 4, Math.PI / 4, 1]}>
          <boxBufferGeometry />
          <meshStandardMaterial color="#f00" />
        </mesh>
      </group>
    </>
  )
}

export default function Index() {
  const ref = React.useRef<THREE.Mesh>(null!)
  const scale = useAspect(1920, 1080)

  const target = useFBO()
  const scene = React.useMemo(() => new THREE.Scene(), [])

  const material = React.useMemo<RawShaderMaterialProps>(
    () => ({
      depthTest: false,
      fragmentShader,
      uniforms: THREE.UniformsUtils.merge([
        { progress: new THREE.Uniform(0) },
        { tex0: new THREE.Uniform(new THREE.Texture()) }
      ]),
      vertexShader
    }),
    []
  )

  const onFrame = React.useCallback<OnFrame>(({ camera, el, gl }) => {
    const m = el?.material

    if (m instanceof THREE.RawShaderMaterial) {
      m.uniforms.tex0 = new THREE.Uniform(target.texture)
    }

    gl.setRenderTarget(target)
    gl.render(scene, camera)
    gl.setRenderTarget(null)
  }, [])

  return (
    <group key={fragmentShader} {...{ scale }}>
      {createPortal(<Box />, scene)}

      <Shader {...{ material, onFrame, ref }} />
    </group>
  )
}
