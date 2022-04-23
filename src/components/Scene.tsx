import { OrbitControls } from '@react-three/drei'
import type { MeshProps } from '@react-three/fiber'
import { createPortal, useFrame, useThree } from '@react-three/fiber'
import * as React from 'react'
import * as THREE from 'three'
import type { OrbitControls as OrbitControlsType } from 'three/examples/jsm/controls/OrbitControls'

export default function Scene({
  camera: initCamera,
  children,
  controlled,
  ...props
}: SceneProps) {
  const st = useThree()
  const scene = React.useMemo(() => new THREE.Scene(), [])
  const controls = React.useRef<OrbitControlsType>(null!)

  const camera = React.useMemo(
    () => initCamera ?? st.camera.clone(false),
    [initCamera]
  )

  const target = React.useMemo(() => new THREE.WebGLRenderTarget(1, 1), [])
  const map = React.useMemo(() => target.texture, [target])

  const onPointer = React.useCallback(
    (b: boolean) => () => controlled && (controls.current.enabled = b),
    []
  )

  React.useEffect(() => () => target.dispose(), [target])

  useFrame(({ gl, size, viewport }) => {
    camera.updateProjectionMatrix()

    target.setSize(
      size.width * viewport.aspect * viewport.dpr,
      size.height * viewport.aspect * viewport.dpr
    )

    gl.clearColor()
    gl.setRenderTarget(target)
    gl.render(scene, camera)
    gl.setRenderTarget(null)
  })

  return (
    <>
      {children && createPortal(children, scene)}

      <mesh
        onPointerEnter={onPointer(true)}
        onPointerLeave={onPointer(false)}
        {...props}
      >
        <planeBufferGeometry />
        <meshBasicMaterial transparent {...{ map }} />
      </mesh>

      {controlled && (
        <OrbitControls ref={controls as any} enabled={false} {...{ camera }} />
      )}
    </>
  )
}

interface SceneProps extends MeshProps {
  controlled?: boolean
  size?: [number, number]
  camera?: THREE.PerspectiveCamera | THREE.OrthographicCamera
}
