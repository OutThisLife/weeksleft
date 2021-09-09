import { Canvas, extend } from '@react-three/fiber'
import * as React from 'react'
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls'
import { Box, Controls, Effects } from './components'

extend({ OrbitControls })

const App: React.FC = () => (
  <Canvas
    camera={{ zoom: 17 }}
    gl={{ alpha: false, antialias: true }}
    linear
    onCreated={({ gl }) => gl.setClearColor('#333')}
    orthographic
    style={{ height: '100vh', width: '100vw' }}>
    <ambientLight />
    <pointLight color="#fff" intensity={0.5} />

    <Box />
    <Effects />
    <Controls />
  </Canvas>
)

export default App
