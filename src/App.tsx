/* eslint-disable no-nested-ternary */
import * as dat from 'dat.gui'
import * as React from 'react'
import * as THREE from 'three'
import fontface from 'three/examples/fonts/droid/droid_sans_mono_regular.typeface.json'

const n = 52
const len = n ** 2
const r = [...Array(len).keys()]

const settings = {
  color1: '#f36',
  color2: '#222',
  weeksLived: 1600
}

const App: React.FC = () => {
  const ref = React.useRef<HTMLCanvasElement>(null)
  const aspect = window.innerWidth / window.innerHeight

  React.useLayoutEffect(() => {
    const scene = new THREE.Scene()
    const mouse = new THREE.Vector2()
    const font = new THREE.Font(fontface)
    const gui = new dat.GUI()

    const ticks: Array<(e?: number) => void> = []
    const mouseTicks: Array<(e: MouseEvent) => void> = []

    const renderer = new THREE.WebGL1Renderer({
      antialias: true,
      canvas: ref.current as HTMLCanvasElement,
      powerPreference: 'high-performance'
    })

    renderer.setPixelRatio(Math.max(window.devicePixelRatio, 2))

    const camera = new THREE.PerspectiveCamera(75, aspect)
    camera.position.z = 40

    const raycaster = new THREE.Raycaster()

    // Lights
    {
      const ambientLight = new THREE.AmbientLight('#fff', 1)

      scene.add(ambientLight)
    }

    // Meshes
    const group = new THREE.Group()

    {
      const color1 = gui.addColor(settings, 'color1')
      const color2 = gui.addColor(settings, 'color2')
      const weeksLived = gui.add(settings, 'weeksLived', 0, len)

      const buf = new THREE.PlaneGeometry(n, n, n - 1, n - 1)
      const colours = r.map(() => new THREE.Vector4(1, 1, 1, 0.1))

      const getColour = (c: dat.GUIController): THREE.Vector4 => {
        const { b, g, r } = new THREE.Color(c.getValue())

        return new THREE.Vector4(r, g, b, 1)
      }

      {
        let i = 0

        for (let x = 0; x < n; x++)
          for (let y = 0; y < n; y++) {
            const id = i++

            if (id <= weeksLived.getValue()) {
              colours[id].copy(getColour(color1))
            } else {
              colours[id].copy(getColour(color2))
            }
          }

        buf.setAttribute(
          'color',
          new THREE.Float32BufferAttribute(
            colours.flatMap(c => c.toArray()),
            4
          )
        )
      }

      const mat = new THREE.PointsMaterial({
        size: 0.6,
        sizeAttenuation: true,
        transparent: true,
        vertexColors: true
      })

      const points = new THREE.Points(buf, mat)

      if (raycaster.params?.Points) {
        raycaster.params.Points.threshold = mat.size
      }

      const draw = () => {
        let i = 0

        const intersects = raycaster.intersectObject(points)
        const pickedId = intersects?.[0]?.index

        for (let y = n; y > 0; y--)
          for (let x = n; x > 0; x--) {
            const id = i++

            if (pickedId === id) {
              colours[id].lerp(new THREE.Vector4(0, 0, 0, 1), 0.1)
            } else if (id <= weeksLived.getValue()) {
              colours[id].lerp(getColour(color1), 0.1)
            } else {
              colours[id].lerp(getColour(color2), 0.1)
            }

            colours[id].toArray(points.geometry.attributes.color.array, id * 4)
          }

        points.geometry.attributes.color.needsUpdate = true
      }

      ticks.push(draw)
      group.add(points)
    }

    {
      const points = group.children[0] as THREE.Points

      let txt: THREE.Mesh

      const mesh = new THREE.Mesh(
        new THREE.PlaneBufferGeometry(1, 1, 1, 1),
        new THREE.MeshBasicMaterial({
          opacity: 0,
          transparent: true
        })
      )

      const txtGroup = new THREE.Group()

      const totalText = new THREE.Mesh(
        new THREE.TextGeometry(`/5200`, {
          font,
          height: 0.1,
          size: 0.5
        }),
        new THREE.MeshBasicMaterial()
      )

      txtGroup.position.set(0.3, -0.3, 0)
      txtGroup.add(totalText)

      const draw = () => {
        const intersects = raycaster.intersectObjects(group.children)
        const pickedId = intersects?.[0]?.index

        if (typeof pickedId === 'number') {
          const pos = points.geometry.attributes.position

          mesh.position.setY(
            THREE.MathUtils.lerp(mesh.position.y, pos.getY(pickedId), 0.1)
          )

          mesh.position.setX(
            THREE.MathUtils.lerp(mesh.position.x, pos.getX(pickedId), 0.1)
          )

          const g = new THREE.TextGeometry(`${pickedId}`, {
            font,
            height: 0.1,
            size: 1
          })

          g.computeBoundingBox()

          totalText.position.set((g.boundingBox?.max.x || 0) + 0.3, 0.3, 0)

          const t = new THREE.Mesh(
            new THREE.TextGeometry(`${pickedId}`, {
              font,
              height: 0.1,
              size: 1
            }),
            new THREE.MeshBasicMaterial()
          )

          mesh.add(txtGroup)
          txtGroup.remove(txt)
          txtGroup.add((txt = t))
        } else {
          if (txt) {
            txtGroup.remove(txt)
          }

          mesh.remove(txtGroup)
        }
      }

      ticks.push(draw)
      group.add(mesh)
    }

    scene.add(group)

    // Resizing
    const handleResize = () => {
      const { innerHeight: h, innerWidth: w } = window

      camera.aspect = w / h
      camera.updateProjectionMatrix()

      renderer.setSize(w, h)
    }

    handleResize()
    window.addEventListener('resize', handleResize)

    // Mouse
    const handleMouse = (e: MouseEvent) => {
      mouse.x = (e.clientX / window.innerWidth) * 2 - 1
      mouse.y = -((e.clientY / window.innerHeight) * 2 - 1)
      mouseTicks.map(f => f(e))
    }

    window.addEventListener('mousemove', handleMouse)
    window.addEventListener('mousedown', handleMouse)

    // Animation loop
    const clock = new THREE.Clock()
    let rafId: number

    const loop = () => {
      const delta = clock.getElapsedTime()

      raycaster.setFromCamera(mouse, camera)
      ticks.forEach(f => f(delta))
      renderer.render(scene, camera)

      rafId = requestAnimationFrame(loop)
    }

    rafId = requestAnimationFrame(loop)

    return () => {
      cancelAnimationFrame(rafId)

      window.removeEventListener('resize', handleResize)
      window.removeEventListener('mousemove', handleMouse)
      window.removeEventListener('mousedown', handleMouse)

      gui.destroy()
    }
  }, [])

  return <canvas {...{ ref }} />
}

export default App
