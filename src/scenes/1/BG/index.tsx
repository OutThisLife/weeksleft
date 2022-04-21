import * as React from 'react'
import { Shader } from '~/components'
import fragmentShader from './frag.fs'
import vertexShader from './vert.vs'

export default function BG() {
  const material = React.useMemo(() => ({ fragmentShader, vertexShader }), [])

  return (
    <Shader position={[0, 0, -1]} {...{ material }}>
      <sphereBufferGeometry args={[1, 100, 100]} />
    </Shader>
  )
}
