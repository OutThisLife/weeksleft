import { OrbitControls } from '@react-three/drei'
import { useFrame } from '@react-three/fiber'
import glsl from 'glslify'
import * as React from 'react'
import * as THREE from 'three'

const vertexShader = glsl`
  #version 300 es
  precision highp float;
  
  uniform mat3 normalMatrix;
  uniform mat4 modelViewMatrix;
  uniform mat4 projectionMatrix;
  uniform mat4 modelMatrix;

  in vec3 normal;
  in vec3 uv;
  in vec4 position;

  out vec3 vUv;
  out vec4 vPos;
  out vec3 vNormal;

  void main() {
    vUv = uv;
    vPos = modelMatrix * position;
    vNormal = normalMatrix * normal;

    gl_Position = projectionMatrix * modelViewMatrix * position;
  }
`.trim()

const fragmentShader = glsl`
  #version 300 es
  precision highp float;

  uniform vec3 iResolution;
  
  in vec3 vUv;
  in vec3 vNormal;
  in vec4 vPos;
  
  out vec4 fragColor;

  #define PI 3.1415926538
  #define PHI 2.399963229728653

vec3 getPoint(vec3 p, vec3 offset) {
  p -= offset / iResolution.z;

  vec3 id = floor(p);
  float s = length(p);
  
  float i = dot(id.x, id.y);
  float y = (1. / i / s) * 2.;
  float r = sqrt(pow(y, 2.));

  float x = cos(i * PHI) * r;
  float z = sin(i * PHI) * r;
  
  return vec3(x, y, z);
}

void main() {
  vec3 col;

  vec3 st = vUv * iResolution.z;
  st *= 2. - 1.;
  
  vec3 gv = fract(st * 44.) - .5;
  float f = gl_FrontFacing ? 1. : -1.;

  {
    vec3 p = getPoint(gv, vec3(.48));
    col += vec3(clamp(1. - p.y, .001, .2 * f));
  }

  fragColor = vec4(pow(col, vec3(1. / 2.2)), 1.);
}
`.trim()

const App: React.FC = () => {
  const ref = React.useRef<THREE.RawShaderMaterial>()

  useFrame(({ size: { width, height }, viewport: { dpr } }) => {
    const w = width * dpr
    const h = height * dpr

    ref.current?.uniforms.iResolution.value.copy(new THREE.Vector3(w, h, w / h))
  })

  return (
    <React.Suspense key={Math.random()} fallback={null}>
      <OrbitControls enableDamping makeDefault />

      <color args={[0x000000]} attach="background" />

      <mesh>
        <sphereBufferGeometry args={[0.5, 120, 120]} />
        <rawShaderMaterial
          uniforms={THREE.UniformsUtils.merge([
            { iResolution: new THREE.Uniform(new THREE.Vector3()) }
          ])}
          {...{ ref, fragmentShader, vertexShader }}
        />
      </mesh>
    </React.Suspense>
  )
}

export default App
