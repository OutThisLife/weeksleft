import * as React from 'react'

export default function Index() {
  return (
    <mesh position={[0, 0, 3]} rotation={[0, 1, 0.5]}>
      <boxGeometry />
      <meshNormalMaterial />
    </mesh>
  )
}
