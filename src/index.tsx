import { Stats } from '@react-three/drei'
import { Canvas } from '@react-three/fiber'
import 'normalize.css'
import * as React from 'react'
import { render } from 'react-dom'
import App from './app'
import './index.css'

render(
  <React.StrictMode>
    <Canvas camera={{ position: [0, 2, 4] }} dpr={[2, 4]} linear>
      <Stats />
      <App />
    </Canvas>
  </React.StrictMode>,
  document.getElementById('root')
)
