import { OrbitControls } from '@react-three/drei'
import { useFrame } from '@react-three/fiber'
import glsl from 'glslify'
import * as React from 'react'
import * as THREE from 'three'

const vertexShader = glsl`#version 300 es
  uniform mat3 normalMatrix;
  uniform mat4 modelMatrix;
  uniform mat4 projectionMatrix;
  uniform mat4 modelViewMatrix;
  uniform mat4 viewMatrix;
  uniform vec3 lightPosition;

  in vec3 normal;
  in vec3 uv;
  in vec4 position;

  out vec3 vLightDir;
  out vec3 vNormal;

  vec3 applyQuaternionToVector(vec4 p, vec3 v) {
    return v + 2. * cross(p.xyz, cross(p.xyz, v) + p.w * v);
  }

  void main() {
    vec4 worldPos = modelMatrix * position;

    vNormal = normalMatrix * (normal * .3);
    vLightDir = mat3(viewMatrix) * (lightPosition - worldPos.xyz);

    gl_Position = projectionMatrix * viewMatrix * worldPos;
  }
`

const fragmentShader = glsl`#version 300 es
  precision mediump float;
  
  uniform float fogNear;
  uniform float fogFar;
  uniform vec3 fogColor;

  in vec3 vNormal;
  in vec3 vLightDir;
  
  out vec4 fragColor;

  void main() {
    vec3 lig = normalize(vLightDir);
    float c = 1. - max(0., dot(vNormal, lig)) * 2.;
    
    float fog = smoothstep(fogNear, fogFar, gl_FragCoord.z / gl_FragCoord.w);

    fragColor = vec4(pow(mix(vec3(.2 + c), fogColor, fog), vec3(1. / 2.2)), 1.);
  }
`

const App: React.FC = () => {
  const ref = React.useRef<THREE.Group>()
  const light = React.useRef<THREE.Light>()

  useFrame(({ scene }) => {
    ref.current?.children?.forEach($m => {
      if (
        $m instanceof THREE.Mesh &&
        $m.material instanceof THREE.RawShaderMaterial
      ) {
        $m.material.uniforms.lightPosition.value = light.current?.position
        $m.material.uniforms.fogNear.value = scene?.fog?.near || 0
        $m.material.uniforms.fogFar.value = scene?.fog?.far || 0
        $m.material.uniforms.fogColor.value =
          scene?.fog?.color || new THREE.Color(0xffffff)
      }
    })
  })

  return (
    <React.Suspense key={Math.random()} fallback={null}>
      <fog args={['#eee', 0, 30]} attach="fog" />

      <OrbitControls enableDamping makeDefault />

      <ambientLight castShadow intensity={0.5} />
      <directionalLight
        ref={light}
        castShadow
        shadow-mapSize-height={1e3}
        shadow-mapSize-width={1e3}
      />

      <group {...{ ref }}>
        <mesh castShadow position={[0, 0.5, 0]} receiveShadow>
          <sphereBufferGeometry args={[0.5, 120, 120]} />
          <meshStandardMaterial color={0xffffff} />
        </mesh>

        <mesh
          position={[0, -1, 0]}
          receiveShadow
          rotation={[-Math.PI / 2, 0, 0]}>
          <planeBufferGeometry args={[1e3, 1e3]} />
          <rawShaderMaterial
            fog
            uniforms={{
              fogColor: new THREE.Uniform(new THREE.Color(0xffffff)),
              fogFar: new THREE.Uniform(0),
              fogNear: new THREE.Uniform(0),
              lightPosition: new THREE.Uniform(new THREE.Vector3())
            }}
            {...{ fragmentShader, vertexShader }}
          />
        </mesh>
      </group>
    </React.Suspense>
  )
}

export default App
