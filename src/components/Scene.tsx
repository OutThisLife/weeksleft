import { useAspect } from '@react-three/drei'
import type { MeshProps } from '@react-three/fiber'
import { createPortal, useFrame } from '@react-three/fiber'
import * as React from 'react'
import * as THREE from 'three'

export default function Scene({ children, ...props }: MeshProps) {
  const scale = useAspect(1920, 1080)
  const scene = React.useMemo(() => new THREE.Scene(), [])

  const camera = React.useMemo(
    () => new THREE.PerspectiveCamera(75, 1, 0.01, 1e3),
    []
  )

  const target = React.useMemo(
    () =>
      new THREE.WebGLRenderTarget(1920, 1680, {
        encoding: THREE.sRGBEncoding,
        format: THREE.RGBAFormat,
        generateMipmaps: true,
        minFilter: THREE.LinearMipMapLinearFilter
      }),
    []
  )

  useFrame(({ gl }) => {
    gl.clearColor()
    gl.setRenderTarget(target)
    gl.render(scene, camera)
    gl.setRenderTarget(null)
  })

  return (
    <>
      {createPortal(children, scene, { camera })}

      <mesh {...{ scale, ...props }}>
        <planeGeometry />
        <meshBasicMaterial map={target.texture} transparent />
      </mesh>
    </>
  )
}
