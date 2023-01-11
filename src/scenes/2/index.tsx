/* eslint-disable @typescript-eslint/ban-ts-comment */
import { OrbitControls } from '@react-three/drei'
import * as React from 'react'

export default function Index() {
  return (
    <>
      <color args={['#EDE8D7']} attach="background" />

      <OrbitControls
        maxAzimuthAngle={1}
        maxDistance={6}
        maxPolarAngle={2.3}
        minAzimuthAngle={-1}
        minDistance={3.33}
        minPolarAngle={0.3}
      />
      <ambientLight intensity={0.5} />

      <directionalLight
        castShadow
        intensity={0.75}
        position={[10, 10, 17]}
        shadow-mapSize={4096}
      />

      <pointLight intensity={1.2} position={[-5, 0, -30]} />
      <pointLight intensity={1.2} position={[0, -5, 0]} />

      <group position={[-0.25, 0, 0.2]} rotation={[1.5, 0, 0]}>
        <mesh castShadow position={[-0.12, 0, 0]}>
          <boxGeometry args={[0.2, 0.35, 1.5]} />
          <meshStandardMaterial color="#13171A" />
        </mesh>

        <mesh castShadow position={[1.12, 0, 0]}>
          <boxGeometry args={[0.2, 0.35, 1.5]} />
          <meshStandardMaterial color="#13171A" />
        </mesh>

        <group rotation={[0, Math.PI / 2, 0]}>
          <mesh castShadow position={[0.65, 0, 0.5]}>
            <boxGeometry args={[0.2, 0.35, 1.1]} />
            <meshStandardMaterial color="#13171A" />
          </mesh>

          <mesh castShadow position={[-0.65, 0, 0.5]}>
            <boxGeometry args={[0.2, 0.35, 1.1]} />
            <meshStandardMaterial color="#13171A" />
          </mesh>
        </group>
      </group>

      <mesh position={[0, 0, 0]} receiveShadow rotation={[-0.1, 0, 0]}>
        <planeGeometry args={[100, 100]} />
        <shadowMaterial color="#000" opacity={0.15} />
      </mesh>
    </>
  )
}
