import type { RootState } from '@react-three/fiber'
import { createPortal, useFrame, useThree } from '@react-three/fiber'
import * as React from 'react'
import * as THREE from 'three'

export default function Scene({
  camera: initCamera,
  children,
  onFrame
}: SceneProps) {
  const st = useThree()
  const scene = React.useMemo(() => new THREE.Scene(), [])

  const camera = React.useMemo<THREE.PerspectiveCamera>(
    () => initCamera ?? (st.camera.clone(false) as THREE.PerspectiveCamera),
    [initCamera]
  )

  const target = React.useMemo(() => new THREE.WebGLRenderTarget(1, 1), [])

  useFrame(e => {
    const { gl, size, viewport } = e

    target.setSize(
      size.width * viewport.aspect * viewport.dpr,
      size.height * viewport.aspect * viewport.dpr
    )

    camera.aspect = viewport.aspect
    camera.updateProjectionMatrix()

    gl.clearColor()
    gl.setRenderTarget(target)
    gl.render(scene, camera)
    gl.setRenderTarget(null)

    onFrame?.({ ...e, target })
  })

  React.useEffect(() => {
    camera.zoom = 5

    return () => target.dispose()
  }, [target])

  return <>{createPortal(children, scene)}</>
}

export interface SceneProps {
  children: React.ReactNode
  camera?: THREE.PerspectiveCamera
  onFrame?(e: RootState & { target: THREE.WebGLRenderTarget }): void
}
