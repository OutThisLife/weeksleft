/* eslint-disable no-nested-ternary */
import { Html, useAspect } from '@react-three/drei'
import type { RawShaderMaterialProps } from '@react-three/fiber'
import { useLoader } from '@react-three/fiber'
import gsap from 'gsap'
import * as React from 'react'
import * as THREE from 'three'
import { Shader } from '~/components'
import fragmentShader from './frag.fs'
import vertexShader from './vert.vs'

export default function Index() {
  const ref = React.useRef<THREE.Mesh>(null!)
  const scale = useAspect(1920, 1080)

  const [state, update] = React.useState(() => ({
    idx: 0,
    running: false
  }))

  const slides = useLoader(
    THREE.TextureLoader,
    Array.from(Array(5)).map((_, i) => `/slides/${i}.webp`)
  )

  const material = React.useMemo<RawShaderMaterialProps>(
    () => ({
      depthTest: false,
      fragmentShader,
      uniforms: {
        dir: new THREE.Uniform(1),
        progress: new THREE.Uniform(0),
        tex0: new THREE.Uniform(slides[0]),
        tex1: new THREE.Uniform(slides[1])
      },
      vertexShader
    }),
    []
  )

  const handle = React.useCallback(
    (i = 1, force = false) => {
      const { idx: cur, running } = state
      const m = ref?.current?.material

      if (!running && m instanceof THREE.RawShaderMaterial) {
        const r = slides.length
        const idx = force ? i : i === 1 ? (cur + 1) % r : (cur - 1 + r) % r

        update({ idx, running: true })

        m.uniforms.dir.value = force ? (idx < cur ? -1 : 1) : i
        m.uniforms.tex1.value = slides[idx]

        gsap.to(m.uniforms.progress, {
          duration: 1,
          onComplete: () => {
            m.uniforms.tex0.value = m.uniforms.tex1.value
            m.uniforms.progress.value = 0

            update(s => ({ ...s, running: false }))
          },
          value: 1
        })
      }
    },
    [state]
  )

  return (
    <>
      <Shader
        key={fragmentShader + vertexShader}
        onPointerDown={e =>
          handle(e.nativeEvent.x > window.innerWidth / 2 ? 1 : -1)
        }
        {...{ material, ref, scale }}
      />

      <Html as="nav" center>
        {slides.map((i, n) => {
          return (
            <a
              key={`slide-${n}`}
              className={state.idx === n ? 'active' : ''}
              href="#!"
              onPointerDown={e => {
                e.preventDefault()
                e.stopPropagation()

                handle(n, true)
              }}
            >
              <img alt="" height={25} src={i.source.data.src} width={25} />
            </a>
          )
        })}
      </Html>
    </>
  )
}
