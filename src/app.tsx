import { OrbitControls, useFBO } from '@react-three/drei'
import type { ShaderMaterialProps } from '@react-three/fiber'
import { createPortal, useFrame, useLoader } from '@react-three/fiber'
import * as React from 'react'
import * as THREE from 'three'

const Earth: React.FC = () => {
  const difTex = useLoader(THREE.TextureLoader, '/earth.jpg')
  const bumpTex = useLoader(THREE.TextureLoader, '/earth-bump.jpg')
  const speTex = useLoader(THREE.TextureLoader, '/earth-spe.png')
  const cloudTex = useLoader(THREE.TextureLoader, '/earth-clouds.png')

  const data = React.useMemo<ShaderMaterialProps>(
    () => ({
      blending: THREE.AdditiveBlending,
      fragmentShader: `#version 300 es
      precision highp float;

      in vec3 vNormal;
      in vec3 vNormel;
      out vec4 fragColor;

      void main() {
        vec3 col = vec3(0., 8, 1.);
        float d = pow(1. - dot(vNormal, vNormel), 4.);

        fragColor = vec4(col, smoothstep(0., 4., d));
      }
      `,
      side: THREE.DoubleSide,
      transparent: true,
      vertexShader: `#version 300 es

      uniform mat3 normalMatrix;
      uniform mat4 modelViewMatrix;
      uniform mat4 projectionMatrix;
      uniform vec3 cameraPosition;

      in vec3 normal;
      in vec3 uv;
      in vec4 position;

      out vec3 vNormal;
      out vec3 vNormel;
      out vec3 vUv;

      void main() {
        vUv = uv;
        vNormal = normalize(normalMatrix * normal);
        vNormel = normalize(normalMatrix * cameraPosition);

        gl_Position = projectionMatrix * modelViewMatrix * position;
      }
    `
    }),
    []
  )

  return (
    <group>
      <ambientLight color={0x888888} />
      <directionalLight color={0xffffff} position={[5, 3, 5]} />

      <mesh>
        <sphereBufferGeometry args={[0.5, 120, 120]} />
        <meshPhongMaterial
          bumpMap={bumpTex}
          bumpScale={0.05}
          map={difTex}
          side={THREE.DoubleSide}
          specularMap={speTex}
        />
      </mesh>

      <mesh>
        <sphereBufferGeometry args={[0.5001, 120, 120]} />
        <meshPhongMaterial map={cloudTex} transparent />
      </mesh>

      <mesh>
        <sphereBufferGeometry args={[0.54, 120, 120]} />
        <rawShaderMaterial {...data} />
      </mesh>
    </group>
  )
}

const Stars: React.FC<{ size?: number; billboard?: boolean }> = ({
  billboard = false,
  size = 1
}) => {
  const ref = React.useRef<THREE.Points>()

  const positions = React.useMemo(
    () =>
      new Float32Array(
        Array.from([...Array(1e4)]).flatMap(() => [
          (Math.random() - 0.5) * (billboard ? 10 : 5),
          (Math.random() - 0.5) * (billboard ? 10 : 5),
          -Math.random()
        ])
      ),
    []
  )

  const colours = React.useMemo(
    () =>
      new Float32Array(
        Array.from([...Array(1e4)]).flatMap(() => [1, 1, 1, Math.random()])
      ),
    []
  )

  const data = React.useMemo<ShaderMaterialProps>(
    () => ({
      fragmentShader: `#version 300 es
        precision mediump float;

        in vec4 vColour;
        out vec4 fragColor;

        void main() { fragColor = vColour; }
      `,
      uniforms: {
        iResolution: new THREE.Uniform(new THREE.Vector3())
      },
      vertexColors: true,
      vertexShader: `#version 300 es
        
        uniform mat4 projectionMatrix;
        uniform mat4 modelViewMatrix;
        uniform vec3 iResolution;

        in vec4 color;
        in vec4 position;

        out vec4 vColour;
        
        void main() {
          vColour = color;
          
          gl_Position = ${
            !billboard
              ? `projectionMatrix * modelViewMatrix * position`
              : `position`
          };
          
          gl_PointSize = ${size}. * iResolution.z;
        }
      `
    }),
    []
  )

  useFrame(({ size: { height, width }, viewport: { dpr } }) => {
    const w = width * dpr
    const h = height * dpr

    const c = ref.current

    if (
      c instanceof THREE.Points &&
      c.material instanceof THREE.RawShaderMaterial
    ) {
      c.material.uniforms.iResolution.value.copy(new THREE.Vector3(w, h, w / h))
    }
  })

  return (
    <points {...{ ref }}>
      <sphereBufferGeometry args={[90, 64, 64]}>
        <bufferAttribute
          array={positions}
          attachObject={['attributes', 'position']}
          count={positions.length / 3}
          itemSize={3}
        />

        <bufferAttribute
          array={colours}
          attachObject={['attributes', 'color']}
          count={colours.length / 4}
          itemSize={4}
        />
      </sphereBufferGeometry>

      <rawShaderMaterial {...data} />
    </points>
  )
}

const BG: React.FC = () => {
  const ref = React.useRef<THREE.Group>()

  const [scene] = React.useState(() => new THREE.Scene())
  const target = useFBO()

  const data = React.useMemo<ShaderMaterialProps>(
    () => ({
      blending: THREE.AdditiveBlending,
      fragmentShader: `#version 300 es
        precision mediump float;

        uniform sampler2D iChannel0;
        uniform vec3 iResolution;

        in vec3 vUv;
        out vec4 fragColor;

        const float M = .1;

        void main() {
          vec2 uv = vUv.xy * 2. - 1.;

          float len = length(uv);
          float theta = atan(len);
          
          float alpha = (4. * M) / len;
          float beta = theta - alpha;
          float into = len / tan(beta);
          float rs = step(2. * M, len);

          vec2 uvCube = vec2(fract(3. * abs(uv.y * uv.x)), abs(into));

          vec3 col;
          vec4 tex = texture(iChannel0, uvCube);

          col += tex.xyz + tex.xyz + tex.xyz;
          col *= rs;

          fragColor = vec4(col, 1.);
        }
      `,
      uniforms: {
        iChannel0: new THREE.Uniform(target.texture),
        iResolution: new THREE.Uniform(new THREE.Vector3())
      },
      vertexShader: `#version 300 es
        in vec3 uv;
        in vec4 position;

        out vec3 vUv;
        
        void main() {
          vUv = uv;
          gl_Position = position;
        }
      `
    }),
    []
  )

  useFrame(({ camera, gl, size: { height, width }, viewport: { dpr } }) => {
    ref.current?.children?.forEach(c => {
      const w = width * dpr
      const h = height * dpr

      if (
        (c instanceof THREE.Mesh || c instanceof THREE.Points) &&
        c.material instanceof THREE.RawShaderMaterial
      ) {
        c.material.uniforms.iResolution.value.copy(
          new THREE.Vector3(w, h, w / h)
        )
      }
    })

    gl.setRenderTarget(target)
    gl.render(scene, camera)
    gl.setRenderTarget(null)
  })

  return (
    <>
      <group {...{ ref }}>
        <Stars billboard size={2} />

        <mesh frustumCulled={false}>
          <planeBufferGeometry args={[2, 2]} />
          <rawShaderMaterial {...data} />
        </mesh>
      </group>

      {createPortal(
        <>
          <Earth />
        </>,
        scene
      )}
    </>
  )
}

const App: React.FC = () => (
  <React.Suspense key={Math.random()} fallback={null}>
    <OrbitControls autoRotateSpeed={0.3} enableDamping makeDefault />

    <BG />
  </React.Suspense>
)

export default App
