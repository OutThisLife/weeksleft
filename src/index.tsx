import { Stats } from '@react-three/drei'
import { Canvas } from '@react-three/fiber'
import 'normalize.css'
import * as React from 'react'
import { render } from 'react-dom'
import App from './app'
import './index.css'

render(
  <React.StrictMode>
    <Canvas dpr={[2, 4]} onCreated={({ gl }) => gl.setClearColor(0x000000)}>
      <React.Suspense fallback={null}>
        <App />
      </React.Suspense>

      <Stats />
    </Canvas>
  </React.StrictMode>,
  document.getElementById('root')
)
