import { OrbitControls } from '@react-three/drei'
import { useFrame } from '@react-three/fiber'
import glsl from 'glslify'
import * as React from 'react'
import * as THREE from 'three'

const chunks = Object.fromEntries(
  Object.entries(THREE.ShaderChunk).map(([k, v]) => [
    k,
    v
      .replace(/varying/gm, /frag/i.test(k) ? 'in' : 'out')
      .replace(/texture2D/gm, 'texture')
      .replace(/gl_FragColor/gm, 'fragColor')
  ])
) as typeof THREE.ShaderChunk

const vertexShader = glsl`
  #version 300 es
  precision highp float;
  
  #define USE_FOG
  #define USE_SHADOWMAP
  
  ${chunks.common}
  ${chunks.shadowmap_pars_vertex}
  ${chunks.fog_pars_vertex}

  uniform mat3 normalMatrix;
  uniform mat4 modelMatrix;
  uniform mat4 modelViewMatrix;
  uniform mat4 projectionMatrix;
  uniform mat4 viewMatrix;

  in vec3 normal;
  in vec3 uv;
  in vec4 position;

  void main() {
    ${chunks.beginnormal_vertex}
    ${chunks.defaultnormal_vertex}
    ${chunks.begin_vertex}
    ${chunks.project_vertex}
    ${chunks.fog_vertex}
    ${chunks.worldpos_vertex}
    ${chunks.shadowmap_vertex}
  }
`.trim()

const fragmentShader = glsl`
  #version 300 es
  precision highp float;
  
  #define USE_FOG
  #define USE_SHADOWMAP
  #define SHADOWMAP_TYPE_VSM

  ${chunks.packing}
  ${chunks.fog_pars_fragment}
  ${chunks.shadowmap_pars_fragment}

  out vec4 fragColor;

  void main() {
    vec3 col;

    for (int i = 0; i < NUM_SPOT_LIGHT_SHADOWS; i++) {
      SpotLightShadow lig = spotLightShadows[i];
      
      vec4 uv = vSpotShadowCoord[i];
      float c = getShadow(spotShadowMap[0], lig.shadowMapSize, lig.shadowBias, lig.shadowRadius, uv);

      col += vec3(.02 * c);
    }

    col = mix(col, fogColor, smoothstep(fogNear, fogFar, vFogDepth));
    
    fragColor = vec4(pow(col, vec3(1. / 2.2)), 1.);
  }
`.trim()

const Sphere: React.FC<any> = props => {
  const mirror = React.useRef<THREE.CubeCamera>()
  const [renderTarget] = React.useState(
    () => new THREE.WebGLCubeRenderTarget(2048)
  )

  useFrame(({ gl, scene }) => void mirror.current?.update(gl, scene))

  return (
    <group {...props}>
      <cubeCamera ref={mirror} args={[0.1, 5e3, renderTarget]} />

      <mesh castShadow>
        <sphereBufferGeometry args={[0.7, 100, 100]} />
        <meshPhongMaterial envMap={renderTarget.texture} />
      </mesh>
    </group>
  )
}

const App: React.FC = () => (
  <React.Suspense key={Math.random()} fallback={null}>
    <OrbitControls enableDamping makeDefault />

    <fog args={[0xffffff, 0, 20]} attach="fog" />
    <ambientLight />
    <spotLight
      castShadow
      position={[0, 10, 0]}
      shadow-mapSize-height={2048}
      shadow-mapSize-width={2048}
      shadow-radius={8}
      shadow-bias={-0.0001}
    />

    <Sphere position={[2, 1, 0]} />
    <Sphere position={[-2, 1, 0]} />

    <group position={[0, 1, 0]}>
      <mesh castShadow>
        <boxBufferGeometry args={[1, 1]} />
        <meshStandardMaterial color={new THREE.Color(0xff3366)} />
      </mesh>

      <mesh>
        <boxBufferGeometry args={[1.2, 1.2]} />
        <meshNormalMaterial wireframe />
      </mesh>
    </group>

    <mesh position={[0, -1, 0]} rotation={[-Math.PI / 2, 0, 0]}>
      <planeBufferGeometry args={[1e3, 1e3]} />
      <rawShaderMaterial
        lights
        fog
        uniforms={THREE.UniformsUtils.merge([
          THREE.UniformsLib.lights,
          THREE.UniformsLib.fog
        ])}
        {...{ fragmentShader, vertexShader }}
      />
    </mesh>
  </React.Suspense>
)

export default App
