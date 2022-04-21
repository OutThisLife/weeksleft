import { useThree } from '@react-three/fiber'
import type { Effect } from 'postprocessing'
import { BlendFunction } from 'postprocessing'
import * as React from 'react'

const wrap = <
  T extends new (...args: any[]) => Effect,
  K extends Record<string, unknown>
>(
  EffectImpl: T,
  defaultBlendMode: BlendFunction = BlendFunction.NORMAL
) =>
  React.forwardRef<T, K & DefaultProps>(function W(
    {
      blendFunction,
      opacity,
      ...props
    }: React.PropsWithChildren<K & DefaultProps>,
    ref
  ) {
    const invalidate = useThree(state => state.invalidate)
    const object: Effect = React.useMemo(() => new EffectImpl(props), [props])

    React.useLayoutEffect(() => {
      object.blendMode.blendFunction =
        !blendFunction && blendFunction !== 0 ? defaultBlendMode : blendFunction

      if (opacity !== undefined) {
        object.blendMode.opacity.value = opacity
      }

      invalidate()
    }, [blendFunction, object.blendMode, opacity])

    return <primitive dispose={null} {...{ object, ref }} />
  })

interface DefaultProps {
  blendFunction?: BlendFunction
  opacity?: number
}

export default wrap
