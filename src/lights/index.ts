import * as THREE from 'three'
import { scene } from '../context'

export default () => {
  const ambientLight = new THREE.AmbientLight('#fff', 1)
  scene.add(ambientLight)
}
