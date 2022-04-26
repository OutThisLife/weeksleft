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
      camera={{ fov: 90, position: [0, 0, 10] }}
      dpr={[2, 4]}
      gl={{ antialias: true, toneMapping: THREE.NoToneMapping }}
      linear
    >
      <React.Suspense fallback={null}>
        <App key={Math.random()} />
      </React.Suspense>

      <Stats />
    </Canvas>
  </React.StrictMode>,
  document.getElementById('root')
)
