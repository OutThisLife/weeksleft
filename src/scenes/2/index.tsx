import { useFrame } from '@react-three/fiber'
import * as React from 'react'
import { Scene } from '~/components'

function Inner() {
  const ref = React.useRef<THREE.Mesh>(null!)

  useFrame(() => {
    ref.current.rotation.z += 0.001
    ref.current.rotation.y += 0.001
  })

  return (
    <mesh position={[0, 0, -0.5]} scale={0.2} {...{ ref }}>
      <boxGeometry args={[1, 1]} />
      <meshNormalMaterial />
    </mesh>
  )
}

export default function Index() {
  return (
    <Scene>
      <Inner />
    </Scene>
  )
}
