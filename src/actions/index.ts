import * as THREE from 'three'
import { font, gui, raycaster, scene, ticks } from '../context'
import fragmentShader from './frag.frag?raw'
import vertexShader from './vert.vert?raw'

const n = 52
const len = n ** 2

export default () => {
  const group = new THREE.Group()

  const color1 = gui.addColor({ color1: '#f36' }, 'color1')
  const color2 = gui.addColor({ color2: '#222' }, 'color2')
  const weeksLived = gui.add({ weeksLived: 1600 }, 'weeksLived', 0, len)

  const grid = () => {
    const buf = new THREE.PlaneBufferGeometry(n, n, n - 1, n - 1)

    const colours = [...Array(len).keys()].map(
      () => new THREE.Vector4(1, 1, 1, 0.1)
    )

    const getColour = (c: dat.GUIController): THREE.Vector4 => {
      const { b, g, r } = new THREE.Color(c.getValue())

      return new THREE.Vector4(r, g, b, 1).normalize()
    }

    {
      let i = 0

      for (let x = 0; x < n; x++)
        for (let y = 0; y < n; y++) {
          const id = i++

          colours[id].copy(getColour(color1))
        }

      buf.setAttribute(
        'color',
        new THREE.Float32BufferAttribute(
          colours.flatMap(c => c.toArray()),
          4
        )
      )
    }

    const mat = new THREE.RawShaderMaterial({
      fragmentShader,
      uniforms: {
        uResolution: new THREE.Uniform(
          new THREE.Vector2(window.innerWidth, window.innerHeight)
        ),
        uTime: new THREE.Uniform(0)
      },
      vertexColors: true,
      vertexShader
    })

    const points = new THREE.Mesh(buf, mat)

    const draw = (e?: number) => {
      let i = 0

      const intersects = raycaster.intersectObject(points)
      const pickedId = intersects?.[0]?.index

      for (let y = n; y > 0; y--)
        for (let x = n; x > 0; x--) {
          const id = i++

          if (pickedId === id) {
            colours[id].lerp(new THREE.Vector4(1, 0, 0, 1), 0.1)
          } else {
            colours[id].lerp(getColour(color1), 0.1)
          }

          colours[id]
            .normalize()
            .toArray(points.geometry.attributes.color.array, id * 4)
        }

      points.material.uniforms.uTime = new THREE.Uniform(e)

      points.material.uniforms.uResolution = new THREE.Uniform(
        new THREE.Vector2(window.innerWidth, window.innerHeight)
      )

      points.geometry.attributes.color.needsUpdate = true
    }

    ticks.push(draw)
    group.add(points)
  }

  const tooltip = () => {
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

  grid()
  tooltip()

  scene.add(group)
}
