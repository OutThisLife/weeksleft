import * as React from 'react'
import { Main } from './components'

const format = (d: Date): string =>
  `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(
    d.getDate()
  ).padStart(2, '0')}`

const App: React.FC = () => {
  const tm = React.useRef<number>()
  const [value, set] = React.useState<string>(() => '1990-06-21')

  const now = new Date()
  const total = 52 * 100

  const diff = React.useMemo(
    () => Math.round((+now - +new Date(value)) / (7 * 24 * 60 * 60 * 1000)),
    [value]
  )

  const onChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const v = e.currentTarget.value
    clearTimeout(tm.current)
    tm.current = setTimeout(() => set(v), 1e3)
  }

  return (
    <Main>
      <form action="#/" method="post">
        <input
          id="date"
          max={format(now)}
          min={format(new Date(now.setFullYear(now.getFullYear() - 100)))}
          name="date"
          type="date"
          {...{ onChange, value }}
        />
      </form>

      <section>
        {[...Array(total).keys()].map(i =>
          React.createElement(
            i <= diff ? 'div' : 'span',
            {
              key: i,
              name: `${i} / ${total}`
            },
            <span />
          )
        )}
      </section>
    </Main>
  )
}

export default App
