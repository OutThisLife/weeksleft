import { Stats } from '@react-three/drei'
import { Canvas } from '@react-three/fiber'
import 'normalize.css'
import * as React from 'react'
import { render } from 'react-dom'
import App from './app'
import './index.css'

render(
  <React.StrictMode>
    <Canvas dpr={[2, 4]} linear orthographic>
      <React.Suspense key={Math.random()} fallback={null}>
        <App />
      </React.Suspense>

      <Stats />
    </Canvas>
  </React.StrictMode>,
  document.getElementById('root')
)
