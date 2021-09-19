import { Stats } from '@react-three/drei'
import { Canvas } from '@react-three/fiber'
import 'normalize.css'
import * as React from 'react'
import { render } from 'react-dom'
import App from './app'
import './index.css'

render(
  <React.StrictMode>
    <Canvas camera={{ fov: 75, position: [0, 0, 1] }} dpr={[1, 2]}>
      <ambientLight />

      <React.Suspense fallback={null}>
        <Stats />
        <App />
      </React.Suspense>
    </Canvas>
  </React.StrictMode>,
  document.getElementById('root')
)
