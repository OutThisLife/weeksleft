import * as dat from 'dat.gui'
import * as THREE from 'three'
import fontface from 'three/examples/fonts/droid/droid_sans_mono_regular.typeface.json'

export const scene = new THREE.Scene()
export const raycaster = new THREE.Raycaster()
export const clock = new THREE.Clock()
export const font = new THREE.Font(fontface)
export const gui = new dat.GUI()

export const ticks: Array<(e?: number) => void> = []
export const mouse = new THREE.Vector2()
export const mouseTicks: Array<(e: MouseEvent) => void> = []

export const camera = new THREE.PerspectiveCamera(
  75,
  window.innerWidth / window.innerHeight
)

camera.position.z = 40

export const bind = (renderer: THREE.WebGL1Renderer) => {
  renderer.setPixelRatio(Math.max(window.devicePixelRatio, 2))

  const handleResize = () => {
    const { innerHeight: h, innerWidth: w } = window

    camera.aspect = w / h
    camera.updateProjectionMatrix()

    renderer.setSize(w, h)
  }

  const handleMouse = (e: MouseEvent) => {
    mouse.x = (e.clientX / window.innerWidth) * 2 - 1
    mouse.y = -((e.clientY / window.innerHeight) * 2 - 1)
    mouseTicks.map(f => f(e))
  }

  renderer.setAnimationLoop(() => {
    const t = clock.getElapsedTime()

    raycaster.setFromCamera(mouse, camera)
    ticks.forEach(f => f(t))
    renderer.render(scene, camera)
  })

  handleResize()
  window.addEventListener('resize', handleResize)
  window.addEventListener('mousemove', handleMouse)
  window.addEventListener('mousedown', handleMouse)

  return () => {
    try {
      gui?.destroy()
    } catch (_) {}

    window.removeEventListener('resize', handleResize)
    window.removeEventListener('mousemove', handleMouse)
    window.removeEventListener('mousedown', handleMouse)
  }
}
