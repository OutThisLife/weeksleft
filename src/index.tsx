import { Stats } from '@react-three/drei'
import { Canvas } from '@react-three/fiber'
import 'normalize.css'
import * as React from 'react'
import { createRoot } from 'react-dom/client'
import App from './app'
import './index.css'

createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <Canvas camera={{ position: [0, 0, 1] }}>
      <React.Suspense>
        <App />
      </React.Suspense>

      <Stats />
    </Canvas>
  </React.StrictMode>
)
