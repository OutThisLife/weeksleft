import { OrbitControls, OrthographicCamera } from '@react-three/drei'
import type { ShaderMaterialProps } from '@react-three/fiber'
import { useFrame } from '@react-three/fiber'
import * as React from 'react'
import * as THREE from 'three'
import fragmentShader from './shaders/frag.fs'
import vertexShader from './shaders/vert.vs'

const App: React.FC = () => {
  const ref = React.useRef<any>()
  const myCamera = React.useRef<any>()

  const data = React.useMemo<ShaderMaterialProps>(
    () => ({
      fragmentShader,
      transparent: false,
      uniforms: {
        iCamera: new THREE.Uniform(new THREE.Vector3(0, 0, 0)),
        iMouse: new THREE.Uniform(new THREE.Vector2(0, 0)),
        iResolution: new THREE.Uniform(new THREE.Vector2(0, 0)),
        iTime: new THREE.Uniform(0)
      },
      vertexShader
    }),
    []
  )

  useFrame(({ camera, clock, mouse, size, viewport }) => {
    const $m = ref.current as THREE.Mesh

    if ($m instanceof THREE.Mesh) {
      const $s = $m.material

      if ($s instanceof THREE.RawShaderMaterial) {
        $s.uniforms.iTime.value = clock.getElapsedTime()
        $s.uniforms.iMouse.value = new THREE.Vector2(mouse.x, mouse.y)
        $s.uniforms.iCamera.value = camera.position

        $s.uniforms.iResolution.value = new THREE.Vector2(
          size.width * viewport.dpr,
          size.height * viewport.dpr
        )

        $m.geometry.setAttribute('pos', $m.geometry.getAttribute('position'))
      }
    }
  })

  return (
    <>
      <OrthographicCamera ref={myCamera} position={[0, 0, 5]} />
      <OrbitControls camera={myCamera.current} enableDamping />

      <mesh frustumCulled={false} {...{ ref }}>
        <planeBufferGeometry args={[2, 2]} />
        <rawShaderMaterial {...data} />
      </mesh>
    </>
  )
}

export default App
