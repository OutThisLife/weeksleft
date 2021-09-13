/* eslint-disable no-nested-ternary */
import * as React from 'react'
import * as THREE from 'three'
import { EffectComposer } from 'three/examples/jsm/postprocessing/EffectComposer'
import { RenderPass } from 'three/examples/jsm/postprocessing/RenderPass'
import { UnrealBloomPass } from 'three/examples/jsm/postprocessing/UnrealBloomPass'
import { Text } from 'troika-three-text'

const n = 52
const len = n ** 2
const r = [...Array(len).keys()]

const state = r.map(i => ({
  colour: new THREE.Vector4(1, 1, 1, 0.1),
  pos: new THREE.Vector3(0, 0, 0),
  scale: new THREE.Vector3(1, 1, 1),
  text: (() => {
    const txt = new Text()

    txt.text = `${i}\n5200`
    txt.fontSize = 0.1
    txt.textAlign = 'center'
    txt.anchorX = '50%'
    txt.color = '#fff'
    txt.anchorY = '50%'
    txt.renderOrder = 1

    return txt
  })()
}))

const mouse = new THREE.Vector2()
const picked: Record<string, number | undefined> = {}

const App: React.FC = () => {
  const ref = React.useRef<HTMLCanvasElement>(null)
  const aspect = window.innerWidth / window.innerHeight

  React.useLayoutEffect(() => {
    const scene = new THREE.Scene()
    const loops: CallableFunction[] = []

    const renderer = new THREE.WebGL1Renderer({
      antialias: true,
      canvas: ref.current as HTMLCanvasElement
    })

    renderer.setPixelRatio(window.devicePixelRatio ?? 2)

    const composer = new EffectComposer(renderer)
    const camera = new THREE.PerspectiveCamera(75, aspect, 0.1, 1000)
    camera.position.z = 30

    const raycaster = new THREE.Raycaster()

    // Lights
    const ambientLight = new THREE.AmbientLight('#fff', 0.7)
    const pointLight = new THREE.PointLight('#fff', 0.5)

    scene.add(ambientLight)
    scene.add(pointLight)

    // Postprocessing
    {
      const renderPass = new RenderPass(scene, camera)

      const bloomPass = new UnrealBloomPass(
        new THREE.Vector2(window.innerWidth, window.innerHeight),
        1,
        0.1,
        0.85
      )

      composer.addPass(renderPass)
      composer.addPass(bloomPass)
    }

    // Meshes
    {
      const tmp = new THREE.Object3D()
      const buf = new THREE.PlaneBufferGeometry(0.5, 0.5)

      buf.setAttribute(
        'color',
        new THREE.InstancedBufferAttribute(
          Float32Array.from(r.flatMap(() => [0, 0, 0, 0])),
          4
        )
      )

      const mesh = new THREE.InstancedMesh(
        buf,
        new THREE.MeshPhongMaterial({
          transparent: true,
          vertexColors: true
        }),
        len
      )

      const draw = (t?: number) => {
        let i = 0

        for (let y = n; y > 0; y--)
          for (let x = n; x > 0; x--) {
            const id = i++

            const a = picked.mousedown === id
            const h = picked.mousemove === id

            const { colour, pos, scale } = state[id]

            pos.set(
              THREE.MathUtils.lerp(
                pos.x,
                a ? 1.2 : 1 - x * 0.75 + 20,
                a ? 0.05 : 0.1
              ),
              -(-y * 0.75 + 20),
              a ? 0.1 : 0
            )

            scale.set(
              THREE.MathUtils.lerp(
                scale.x,
                a ? n * 1.499 : h ? 2 : 1,
                a ? 0.05 : 0.1
              ),
              THREE.MathUtils.lerp(scale.y, a ? 3 : h ? 2 : 1, a ? 0.05 : 0.1),
              THREE.MathUtils.lerp(scale.z, h ? 1.2 : 1, 0.1)
            )

            tmp.position.copy(pos)
            tmp.scale.copy(scale)

            if (a) {
              colour.set(1, 1, 1, 1)
            } else if (id <= 1600) {
              colour.set(1, 0.41, 0.7, 1)
            } else if (h) {
              colour.set(0.2, 0.2, 0.2, 1)
            } else {
              colour.set(
                0.2,
                0.2,
                0.2,
                THREE.MathUtils.lerp(colour.w, a ? 1 : h ? 0.8 : 0.5, 0.1)
              )
            }

            mesh.geometry.attributes.color.setXYZW(id, ...colour.toArray())
            mesh.geometry.attributes.color.needsUpdate = true

            tmp.updateMatrix()
            mesh.setMatrixAt(id, tmp.matrix)
          }

        mesh.instanceMatrix.needsUpdate = true
      }

      draw()

      scene.add(mesh)
      loops.unshift(draw)
    }

    // Resizing
    const handleResize = () => {
      const { innerHeight: h, innerWidth: w } = window

      camera.aspect = w / h
      camera.updateProjectionMatrix()

      renderer.setSize(w, h)
      composer.setSize(w, h)
    }

    handleResize()
    window.addEventListener('resize', handleResize)

    // Mouse
    const handleMouse = (e: MouseEvent) => {
      mouse.x = (e.clientX / window.innerWidth) * 2 - 1
      mouse.y = -((e.clientY / window.innerHeight) * 2 - 1)

      const intersects = raycaster.intersectObjects(
        scene.children.filter(c => c instanceof THREE.InstancedMesh)
      )

      if (intersects.length) {
        picked[e.type] =
          e.type === 'mousedown' && intersects[0].instanceId === picked[e.type]
            ? undefined
            : intersects[0].instanceId
      } else if (e.type !== 'mousedown') {
        delete picked[e.type]
      }
    }

    window.addEventListener('mousemove', handleMouse)
    window.addEventListener('mousedown', handleMouse)

    // Animation loop
    const clock = new THREE.Clock()
    let rafId: number

    const loop = () => {
      const delta = clock.getElapsedTime()

      raycaster.setFromCamera(mouse, camera)
      pointLight.position.copy(camera.position)

      loops.forEach(f => f(delta))

      composer.render()

      rafId = requestAnimationFrame(loop)
    }

    rafId = requestAnimationFrame(loop)

    return () => {
      cancelAnimationFrame(rafId)

      window.removeEventListener('resize', handleResize)
      window.removeEventListener('mousemove', handleMouse)
      window.removeEventListener('mousedown', handleMouse)
    }
  }, [])

  return <canvas {...{ ref }} />
}

export default App
