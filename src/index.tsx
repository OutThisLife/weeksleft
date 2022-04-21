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
      camera={{ position: [0, 0, 1] }}
      dpr={[2, 4]}
      gl={{
        alpha: false,
        antialias: true,
        depth: false,
        powerPreference: 'high-performance',
        stencil: false
      }}
    >
      <Stats />

      <React.Suspense fallback={null}>
        <App />
      </React.Suspense>
    </Canvas>
  </React.StrictMode>,
  document.getElementById('root')
)
