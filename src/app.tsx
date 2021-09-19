import { useAspect } from '@react-three/drei'
import { useFrame } from '@react-three/fiber'
import * as React from 'react'
import * as THREE from 'three'
import fragmentShader from './shaders/fragment.frag?raw'
import vertexShader from './shaders/vertex.vert?raw'

const App: React.FC = () => {
  const ref = React.useRef<THREE.Mesh>()

  const scale = useAspect(1024, 768)

  const data = React.useMemo(
    () => ({
      fragmentShader,
      uniforms: {
        iMouse: new THREE.Uniform(new THREE.Vector2(0, 0)),
        iResolution: new THREE.Uniform(new THREE.Vector2(0, 0)),
        iTime: new THREE.Uniform(0)
      },
      vertexShader
    }),
    []
  )

  useFrame(({ clock, mouse, size }) => {
    if (ref.current instanceof THREE.Mesh) {
      const $s = ref.current.material as THREE.RawShaderMaterial

      $s.uniforms.iTime.value = clock.getElapsedTime()
      $s.uniforms.iResolution.value = new THREE.Vector2(size.width, size.height)
      $s.uniforms.iMouse.value = new THREE.Vector2(mouse.x, mouse.y)
    }
  })

  return (
    <React.Suspense key={Math.random()} fallback={null}>
      <mesh {...{ ref, scale }}>
        <planeBufferGeometry args={[1, 1]} />
        <rawShaderMaterial transparent {...data} />
      </mesh>
    </React.Suspense>
  )
}

export default App
