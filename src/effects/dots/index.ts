import { BlendFunction, Effect } from 'postprocessing'
import wrapEffect from '../wrap'
import fragmentShader from './frag.fs'

export default wrapEffect(
  class extends Effect {
    constructor() {
      super('DotEffect', fragmentShader, {
        blendFunction: BlendFunction.NORMAL
      })
    }
  }
)
