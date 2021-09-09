/* eslint-disable no-nested-ternary */
import type { InstancedMeshProps } from '@react-three/fiber'
import { useFrame } from '@react-three/fiber'
import * as React from 'react'
import type { InstancedMesh } from 'three'
import * as THREE from 'three'

const len = 52 * 100
const tmp = new THREE.Object3D()

const data = Array.from({ length: len }, () => ({
  scale: 1,
  wireframe: false
}))

export const Box: React.FC<InstancedMeshProps> = props => {
  const ref = React.useRef<InstancedMesh>()
  const prev = React.useRef<number>()

  const [state, set] = React.useState<Record<string, number | undefined>>(
    () => ({})
  )

  const colors = React.useMemo(
    () =>
      Float32Array.from(
        [...Array(len).keys()].flatMap(() => new THREE.Color('#fff').toArray())
      ),
    []
  )

  React.useEffect(() => void (prev.current = state.hovered), [state])

  useFrame(() => {
    if (ref.current) {
      let i = 0

      for (let x = 0; x < len / 100; x++)
        for (let y = 0; y < len / 100; y++) {
          const id = i++
          const isHovered = id === state.hovered

          tmp.position.set(27 - x, 26 - y, -33)
          tmp.rotation.set(0, 0, 0)

          new THREE.Color()
            .set(isHovered ? '#f36' : id * 3 <= 1600 ? '#fff' : '#333')
            .toArray(colors, id * 3)

          ref.current.geometry.attributes.color.needsUpdate = isHovered

          const scale = THREE.MathUtils.lerp(
            data[id].scale,
            isHovered ? 4 : 1,
            0.1
          )

          data[id].scale = scale
          tmp.scale.setScalar(scale)

          tmp.updateMatrix()
          ref.current.setMatrixAt(id, tmp.matrix)
        }

      ref.current.instanceMatrix.needsUpdate = true
    }
  })

  return (
    <instancedMesh
      args={[undefined, undefined, len]}
      onPointerMove={e => set(st => ({ ...st, hovered: e.instanceId }))}
      onPointerOut={() => set(st => ({ ...st, hovered: undefined }))}
      {...{ ref, ...props }}>
      <boxGeometry args={[0.3, 0.3, 0.3]} attach="geometry">
        <instancedBufferAttribute
          args={[colors, 3]}
          attachObject={['attributes', 'color']}
        />
      </boxGeometry>

      <meshPhongMaterial
        attach="material"
        clipShadows
        fog
        shadowSide={THREE.BackSide}
        vertexColors
      />
    </instancedMesh>
  )
}

export default Box
