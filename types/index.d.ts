import type { Object3DNode } from '@react-three/fiber'
import type { OrbitControls } from 'three/examples/jsm/controls/OrbitControls'
import type { EffectComposer } from 'three/examples/jsm/postprocessing/EffectComposer'
import type { GlitchPass } from 'three/examples/jsm/postprocessing/GlitchPass'
import type { RenderPass } from 'three/examples/jsm/postprocessing/RenderPass'
import type { ShaderPass } from 'three/examples/jsm/postprocessing/ShaderPass'
import type { SSAOPass } from 'three/examples/jsm/postprocessing/SSAOPass'
import type { UnrealBloomPass } from 'three/examples/jsm/postprocessing/UnrealBloomPass'

declare global {
  namespace JSX {
    interface IntrinsicElements {
      effectComposer: Object3DNode<EffectComposer, typeof EffectComposer>
      renderPass: Object3DNode<RenderPass, typeof RenderPass>
      shaderPass: Object3DNode<ShaderPass, typeof ShaderPass>
      sSAOPass: Object3DNode<SSAOPass, typeof SSAOPass>
      orbitControls: Object3DNode<OrbitControls, typeof OrbitControls>
      unrealBloomPass: Object3DNode<UnrealBloomPass, typeof UnrealBloomPass>
      glitchPass: Object3DNode<GlitchPass, typeof GlitchPass>
    }
  }
}
