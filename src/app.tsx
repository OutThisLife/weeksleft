import { OrbitControls, softShadows } from '@react-three/drei'
import { useFrame, useThree } from '@react-three/fiber'
import glsl from 'glslify'
import * as React from 'react'
import * as THREE from 'three'

softShadows()

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

  ${chunks.common}
  ${chunks.packing}
  ${chunks.fog_pars_fragment}
  ${chunks.shadowmap_pars_fragment}

  out vec4 fragColor;

  void main() {
    vec3 col;

    for (int i = 0; i < NUM_DIR_LIGHT_SHADOWS; i++) {
      DirectionalLightShadow lig = directionalLightShadows[i];
      
      vec4 uv = vDirectionalShadowCoord[i];
      float c = getShadow(directionalShadowMap[0], lig.shadowMapSize, lig.shadowBias, lig.shadowRadius, uv);

      col += vec3(.7 + c);
    }

    col = mix(col, fogColor, smoothstep(fogNear, fogFar, vFogDepth));
    
    fragColor = vec4(col, 1.);
  }
`.trim()

const Sphere: React.FC<any> = props => {
  const ref = React.useRef<THREE.Group>()
  const [camera, setCamera] = React.useState<THREE.CubeCamera>()
  const { gl, scene } = useThree()

  const [fbo] = React.useState(
    () =>
      new THREE.WebGLCubeRenderTarget(2048, {
        minFilter: THREE.LinearFilter,
        magFilter: THREE.LinearFilter,
        format: THREE.RGBFormat,
        encoding: gl.outputEncoding
      })
  )

  useFrame(() => {
    ref.current?.traverse(o => (o.visible = false))
    camera?.update(gl, scene)
    ref.current?.traverse(o => (o.visible = true))
  })

  return (
    <group {...props}>
      <cubeCamera args={[0.1, 1e3, fbo]} ref={setCamera} />

      <group {...{ ref }}>
        <mesh castShadow receiveShadow>
          <sphereBufferGeometry args={[0.7, 100, 100]} />
          <meshPhongMaterial envMap={fbo.texture} />
        </mesh>
      </group>
    </group>
  )
}

const App: React.FC = () => {
  const ref = React.useRef<THREE.Group>()

  useFrame(() => void (ref.current.rotation.z += 0.01))

  return (
    <React.Suspense key={Math.random()} fallback={null}>
      <OrbitControls enableDamping makeDefault />

      <fog args={[0xeeeeee, -1, 40]} attach="fog" />
      <color args={[0xeeeeee]} attach="background" />

      <ambientLight />
      <directionalLight
        castShadow
        position={[0, 10, 0]}
        intensity={2}
        shadow-mapSize-width={2048}
        shadow-mapSize-height={2048}
        shadow-camera-far={100}
        shadow-camera-left={-10}
        shadow-camera-right={10}
        shadow-camera-top={10}
        shadow-camera-bottom={-10}
      />

      <Sphere position={[2, 1, 0]} />
      <Sphere position={[-2, 1, 0]} />

      <group position={[0, 1, 0]} rotation={[0, 0, -1]} {...{ ref }}>
        <mesh castShadow receiveShadow>
          <boxBufferGeometry args={[1, 1]} />
          <meshStandardMaterial color={new THREE.Color(0xff3366)} />
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
}

export default App
