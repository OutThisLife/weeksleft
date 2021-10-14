import { Stats } from '@react-three/drei'
import { Canvas } from '@react-three/fiber'
import 'normalize.css'
import * as React from 'react'
import { render } from 'react-dom'
import * as THREE from 'three'
import App from './app'
import './index.css'

render(
  <React.StrictMode>
    <Canvas
      camera={{ position: [0, 2, 5] }}
      dpr={[2, 4]}
      mode="concurrent"
      shadows={{ type: THREE.VSMShadowMap }}>
      <Stats />

      <React.Suspense fallback={null}>
        <App />
      </React.Suspense>
    </Canvas>
  </React.StrictMode>,
  document.getElementById('root')
)
