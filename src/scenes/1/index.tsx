import { useFrame } from '@react-three/fiber'
import * as React from 'react'
import * as THREE from 'three'
import { Scene } from '~/components'

const BG = React.lazy(() => import('./BG'))
const Sphere = React.lazy(() => import('./Sphere'))

function Inner() {
  const ref = React.useRef<THREE.Group>(null)

  const target = React.useMemo(
    () =>
      new THREE.WebGLCubeRenderTarget(512, {
        encoding: THREE.sRGBEncoding,
        format: THREE.RGBAFormat,
        generateMipmaps: true,
        minFilter: THREE.LinearMipMapLinearFilter
      }),
    []
  )

  const cube = React.useMemo<THREE.CubeCamera>(
    () => new THREE.CubeCamera(0.1, 10, target),
    [target]
  )

  useFrame(({ gl, scene }) => {
    const el = ref.current

    if (el instanceof THREE.Group) {
      const $s = el.children[1]

      if (
        $s instanceof THREE.Mesh &&
        $s.material instanceof THREE.RawShaderMaterial
      ) {
        $s.visible = false
        cube.update(gl, scene)
        $s.visible = true

        if ($s.material.uniforms.tCube) {
          $s.material.uniforms.tCube.value = cube.renderTarget.texture
        }
      }
    }
  })

  return (
    <group {...{ ref }}>
      <BG />
      <Sphere />
    </group>
  )
}

export default function Index() {
  return (
    <Scene>
      <Inner />
    </Scene>
  )
}
