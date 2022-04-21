import type {
  MeshProps,
  RawShaderMaterialProps,
  RootState
} from '@react-three/fiber'
import { useFrame } from '@react-three/fiber'
import * as React from 'react'
import * as THREE from 'three'

export default function Shader({
  material,
  onFrame = () => void null,
  children,
  ...props
}: ShaderProps) {
  const ref = React.useRef<THREE.Mesh>(null)

  const args = React.useMemo<RawShaderMaterialProps>(
    () => ({
      side: THREE.DoubleSide,
      ...material,
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

    onFrame({ ...st, el: ref.current })
  })

  return (
    <mesh frustumCulled={false} {...{ ref, ...props }}>
      {children || <planeGeometry />}
      <rawShaderMaterial {...args} />
    </mesh>
  )
}

interface ShaderProps extends Omit<MeshProps, 'material'> {
  material?: RawShaderMaterialProps
  onFrame?: FrameCB
}

export type FrameCB = (e: RootState & { el: THREE.Mesh | null }) => void
