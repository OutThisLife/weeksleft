import { Stats } from '@react-three/drei'
import { Canvas } from '@react-three/fiber'
import 'normalize.css'
import * as React from 'react'
import { render } from 'react-dom'
import App from './app'
import './index.css'

render(
  <React.StrictMode>
    <Canvas
      camera={{ far: 1e3, fov: 45, near: 0.1, position: [0, 0, 1.5] }}
      dpr={[2, 4]}
      mode="concurrent">
      <Stats />

      <React.Suspense fallback={null}>
        <App />
      </React.Suspense>
    </Canvas>
  </React.StrictMode>,
  document.getElementById('root')
)
