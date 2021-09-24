import { Stats } from '@react-three/drei'
import { Canvas } from '@react-three/fiber'
import 'normalize.css'
import * as React from 'react'
import { render } from 'react-dom'
import App from './app'
import './index.css'

console.clear()

render(
  <React.StrictMode>
    <Canvas mode="concurrent">
      <color args={['#222']} attach="background" />
      <Stats />
      <App />
    </Canvas>
  </React.StrictMode>,
  document.getElementById('root')
)
