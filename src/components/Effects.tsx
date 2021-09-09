import { extend, useFrame, useThree } from '@react-three/fiber'
import * as React from 'react'
import { EffectComposer } from 'three/examples/jsm/postprocessing/EffectComposer'
import { RenderPass } from 'three/examples/jsm/postprocessing/RenderPass'
import { ShaderPass } from 'three/examples/jsm/postprocessing/ShaderPass'
import { SSAOPass } from 'three/examples/jsm/postprocessing/SSAOPass'
import { UnrealBloomPass } from 'three/examples/jsm/postprocessing/UnrealBloomPass'
import { FXAAShader } from 'three/examples/jsm/shaders/FXAAShader'

extend({
  EffectComposer,
  RenderPass,
  SSAOPass,
  ShaderPass,
  UnrealBloomPass
})

export const Effects: React.FC = () => {
  const { camera, gl, scene, size } = useThree()
  const ref = React.useRef<EffectComposer>()

  React.useEffect(
    () => void ref.current?.setSize(size.width * 2, size.height * 2),
    [size]
  )

  useFrame(() => void ref.current?.render(), 2)

  return (
    <effectComposer args={[gl]} {...{ ref }}>
      <renderPass attachArray="passes" {...{ camera, scene }} />
      <sSAOPass
        args={[scene, camera]}
        attachArray="passes"
        kernelRadius={0.4}
        maxDistance={0.03}
      />

      <shaderPass
        args={[FXAAShader]}
        attachArray="passes"
        material-uniforms-resolution-value={[1 / size.width, 1 / size.height]}
        renderToScreen
      />

      <unrealBloomPass attachArray="passes" radius={1.2} threshold={0.95} />
    </effectComposer>
  )
}

export default Effects
