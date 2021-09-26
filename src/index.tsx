import { Stats } from '@react-three/drei'
import { Canvas } from '@react-three/fiber'
import 'normalize.css'
import * as React from 'react'
import { render } from 'react-dom'
import App from './app'
import './index.css'

render(
  <React.StrictMode>
    <Canvas camera={{ position: [0, 0, 1] }} linear mode="concurrent">
      <React.Suspense key={Math.random()} fallback={null}>
        <Stats />

        <App />
      </React.Suspense>

      <color args={['#222']} attach="background" />
    </Canvas>
  </React.StrictMode>,
  document.getElementById('root')
)
