import * as React from 'react'
import * as THREE from 'three'
import actions from './actions'
import { bind } from './context'
import lights from './lights'

const App: React.FC = () => {
  const ref = React.useRef<HTMLCanvasElement>(null)

  React.useLayoutEffect(() => {
    const renderer = new THREE.WebGL1Renderer({
      antialias: true,
      canvas: ref.current as HTMLCanvasElement,
      powerPreference: 'high-performance'
    })

    actions()
    lights()

    return bind(renderer)
  }, [])

  return <canvas {...{ ref }} />
}

export default App
