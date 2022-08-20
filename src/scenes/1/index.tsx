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
    animating: false,
    idx: 0
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
      const { animating, idx: cur } = state
      const m = ref?.current?.material

      if (!animating && m instanceof THREE.RawShaderMaterial) {
        const r = slides.length
        const idx = force ? i : i === 1 ? (cur + 1) % r : (cur - 1 + r) % r
        const dir = !force ? i : idx < cur ? -1 : 1

        update({ animating: true, idx })

        m.uniforms.tex1.value = slides[idx]
        m.uniforms.dir.value = dir

        gsap.to(m.uniforms.progress, {
          duration: 1,
          onComplete: () => {
            m.uniforms.tex0.value = m.uniforms.tex1.value
            m.uniforms.progress.value = 0

            update(s => ({ ...s, animating: false }))
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
            <img
              key={`slide-${n}`}
              alt=""
              className={state.idx === n ? 'active' : ''}
              height={50}
              onPointerDown={e => {
                e.preventDefault()
                e.stopPropagation()

                handle(n, true)
              }}
              src={i.source.data.src}
              width={50}
            />
          )
        })}
      </Html>
    </>
  )
}
