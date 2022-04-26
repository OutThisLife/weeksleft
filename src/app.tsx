import * as React from 'react'
import { Scene1 } from './scenes'

export default function App() {
  return (
    <>
      <color args={[0x000000]} attach="background" />

      <Scene1 />
    </>
  )
}
