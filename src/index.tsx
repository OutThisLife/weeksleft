import { Stats } from '@react-three/drei'
import { Canvas } from '@react-three/fiber'
import 'normalize.css'
import * as React from 'react'
import { render } from 'react-dom'
import App from './app'
import './index.css'

render(
  <React.StrictMode>
    <Canvas camera={{ fov: 30, position: [0, 0, 10] }} dpr={[2, 4]}>
      <Stats />
      <App />
    </Canvas>
  </React.StrictMode>,
  document.getElementById('root')
)
