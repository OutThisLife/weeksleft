import type {
  MeshProps,
  RawShaderMaterialProps,
  RootState
} from '@react-three/fiber'
import { useFrame } from '@react-three/fiber'
import * as React from 'react'
import * as THREE from 'three'

export default React.forwardRef<THREE.Mesh, ShaderProps>(function Shader(
  { children, material, onFrame, ...props },
  outerRef
) {
  const ref = React.useRef<THREE.Mesh>(null!)

  const args = React.useMemo<RawShaderMaterialProps>(
    () => ({
      ...material,
      side: THREE.DoubleSide,
      uniforms: {
        iFrame: new THREE.Uniform(1),
        iMouse: new THREE.Uniform(new THREE.Vector2(1, 1)),
        iResolution: new THREE.Uniform(new THREE.Vector4(1, 1, 1, 2)),
        iTime: new THREE.Uniform(0),
        ...material?.uniforms
      }
    }),
    []
  )

  useFrame(st => {
    const {
      clock,
      mouse: { x = 0, y = 0 },
      size: { height, width },
      viewport: { dpr }
    } = st

    const m = ref.current?.material

    if (m instanceof THREE.RawShaderMaterial) {
      const w = width * dpr
      const h = height * dpr

      m.uniforms.iFrame.value = clock.getDelta()
      m.uniforms.iMouse.value.copy(new THREE.Vector2(x, y))
      m.uniforms.iResolution.value.copy(new THREE.Vector4(w, h, w / h, dpr))
      m.uniforms.iTime.value = clock.getElapsedTime()
    }

    onFrame?.({ ...st, el: ref.current })
  })

  return (
    <mesh
      ref={(e: THREE.Mesh) => {
        ref.current = e

        if (typeof outerRef === 'function') {
          outerRef(e)
        } else if (outerRef) {
          outerRef.current = e
        }
      }}
      frustumCulled={false}
      {...props}
    >
      {children || <planeGeometry />}
      <rawShaderMaterial {...args} />
    </mesh>
  )
})

export interface ShaderProps extends Omit<MeshProps, 'material'> {
  material?: RawShaderMaterialProps
  onFrame?: OnFrame
}

export type OnFrame = (e: RootState & { el: Maybe<THREE.Mesh> }) => void
