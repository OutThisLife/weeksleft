/* eslint-disable @typescript-eslint/ban-ts-comment */
import { useFBO } from '@react-three/drei'
import type { RawShaderMaterialProps } from '@react-three/fiber'
import { createPortal, useFrame } from '@react-three/fiber'
import * as React from 'react'
import * as THREE from 'three'
import fragmentShader from './frag.fs'
import fragmentShader2 from './frag2.fs'
import vertexShader from './vert.vs'

export default function Index() {
  const ref = React.useRef<
    THREE.Mesh<THREE.BufferGeometry, THREE.Material | THREE.Material[]>[]
  >([]!)
  // const scale = useAspect(1920, 1080)

  const cam = React.useRef<THREE.PerspectiveCamera>(null!)
  const scene = React.useMemo(() => new THREE.Scene(), [])

  const fbo = useFBO()

  const material = React.useMemo<RawShaderMaterialProps>(
    () => ({
      fragmentShader,
      side: THREE.DoubleSide,
      transparent: true,
      uniforms: {
        iFrame: new THREE.Uniform(1),
        iMouse: new THREE.Uniform(new THREE.Vector2(1, 1)),
        iResolution: new THREE.Uniform(new THREE.Vector4(1, 1, 1, 2)),
        iTime: new THREE.Uniform(0)
      },
      vertexShader
    }),
    []
  )

  useFrame(
    ({
      clock,
      gl,
      mouse: { x = 0, y = 0 },
      size: { height, width },
      viewport: { dpr }
    }) => {
      const w = width * dpr
      const h = height * dpr

      ref.current
        ?.filter(i => i?.material instanceof THREE.RawShaderMaterial)
        ?.flatMap(i => i.material as THREE.RawShaderMaterial)
        .forEach(m => {
          m.uniforms.iFrame.value = clock.getDelta()
          m.uniforms.iMouse.value.copy(new THREE.Vector2(x, y))
          m.uniforms.iResolution.value.copy(new THREE.Vector4(w, h, w / h, dpr))
          m.uniforms.iTime.value = clock.getElapsedTime()

          if (m.uniforms?.tex0) {
            m.uniforms.tex0.value = fbo.texture
          }
        })

      if (cam.current) {
        gl.setRenderTarget(fbo)
        gl.render(scene, cam.current)
        gl.setRenderTarget(null)
      }
    }
  )

  return (
    <group key={Math.random()}>
      <mesh ref={e => ref.current?.push(e!)}>
        <planeBufferGeometry args={[1, 1, 25, 25]} />
        <rawShaderMaterial
          {...{
            ...material,
            uniforms: {
              ...material.uniforms,
              tex0: new THREE.Uniform(new THREE.Texture())
            }
          }}
        />
      </mesh>

      {createPortal(
        <>
          <perspectiveCamera ref={cam} position={[0, 0, 1]} />
          <mesh ref={e => ref.current?.push(e!)}>
            <planeBufferGeometry />
            <rawShaderMaterial
              {...{ ...material, fragmentShader: fragmentShader2 }}
            />
          </mesh>
        </>,
        scene
      )}
    </group>
  )
}
