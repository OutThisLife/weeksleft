import * as React from 'react'
import * as THREE from 'three'
import { bind } from './context'

const App: React.FC = () => {
  const ref = React.useRef<HTMLCanvasElement>(null)

  React.useEffect(() => {
    const renderer = new THREE.WebGL1Renderer({
      antialias: true,
      canvas: ref.current as HTMLCanvasElement,
      powerPreference: 'high-performance'
    })

    ;(async () => {
      await import('./actions')
      await import('./lights')
    })()

    return bind(renderer)
  }, [])

  return <canvas {...{ ref }} />
}

export default App
