import * as THREE from 'three'
import { font, gui, raycaster, scene, ticks } from '../context'

const n = 52
const len = n ** 2
const r = [...Array(len).keys()]

const group = new THREE.Group()

const color1 = gui.addColor({ color1: '#f36' }, 'color1')
const color2 = gui.addColor({ color2: '#222' }, 'color2')
const weeksLived = gui.add({ weeksLived: 1600 }, 'weeksLived', 0, len)

const grid = () => {
  const buf = new THREE.PlaneGeometry(n, n, n - 1, n - 1)
  const colours = r.map(() => new THREE.Vector4(1, 1, 1, 0.1))

  const getColour = (c: dat.GUIController): THREE.Vector4 => {
    const { b, g, r } = new THREE.Color(c.getValue())

    return new THREE.Vector4(r, g, b, 1).normalize()
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
        colours.flatMap(c => c.normalize().toArray()),
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

  const mat2 = new THREE.RawShaderMaterial({
    depthTest: true,
    fragmentShader: `
precision mediump float;

#define PI 3.14159265359
#define TWO_PI 6.28318530718

uniform float time;
varying vec4 vColor;
varying vec2 vUv;

void main() {
  vec3 c = vColor.xyz;
  gl_FragColor = vec4(c, 1.);
}
      `,
    transparent: false,
    uniforms: {
      size: { value: 20 },
      time: { value: 1.0 }
    },
    vertexColors: true,
    vertexShader: `
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;
attribute vec3 position;

uniform float size;

attribute vec4 color;
varying vec4 vColor;

attribute vec2 uv;
varying vec2 vUv;

void main() {
  vUv = uv;
  vColor = color;

  gl_PointSize = size;
  gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(position, 1.0);
}
      `
  })

  const points = new THREE.Points(buf, mat2)

  if (raycaster.params?.Points) {
    raycaster.params.Points.threshold = mat.size
  }

  const draw = (e?: number) => {
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

        colours[id]
          .normalize()
          .toArray(points.geometry.attributes.color.array, id * 4)
      }

    points.geometry.attributes.color.needsUpdate = true
    points.material.uniforms.time.value = e
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
// tooltip()

scene.add(group)
