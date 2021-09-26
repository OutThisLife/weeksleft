import { Stats } from '@react-three/drei'
import { Canvas } from '@react-three/fiber'
import 'normalize.css'
import * as React from 'react'
import { render } from 'react-dom'
import App from './app'
import './index.css'

render(
  <React.StrictMode>
    <Canvas camera={{ position: [0, 0, 1] }} mode="concurrent">
      <Stats />
      <App />
    </Canvas>
  </React.StrictMode>,
  document.getElementById('root')
)
