import { extend, useFrame, useThree } from '@react-three/fiber'
import * as React from 'react'
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls'

extend({ OrbitControls })

export const Controls: React.FC = () => {
  const { camera, gl } = useThree()
  const ref = React.useRef<OrbitControls>()

  useFrame(() => ref.current?.update())

  return (
    <orbitControls
      args={[camera, gl.domElement]}
      enableDamping
      enablePan={false}
      {...{ ref }}
    />
  )
}

export default Controls
