import { css } from 'styled-components'

const { error, log } = console

export const onScreenLogging = () => {
  const $log = document.createElement('div')

  $log.style.setProperty('position', 'fixed')
  $log.style.setProperty('inset', 'auto 0 0 auto')
  $log.style.setProperty('z-index', `${1e3}`)
  $log.style.cssText = css`
    background: #1c1d1e;
    color: #fff;
    font-family: monospace;
    font-size: 11px;
    inset: auto 0 0 auto;
    letter-spacing: 0.02em;
    line-height: 1;
    padding: 1rem;
    position: fixed;
    white-space: break-spaces;
    z-index: 1000;
  `.toString()

  if (!document.body.contains($log)) {
    document.body.appendChild($log)
  }

  console.error = (...args) => {
    const $f = document.createDocumentFragment()
    const $c = $f.appendChild(document.createElement('div'))
    $c.style.cssText = css`
      max-height: calc(var(--vsq) * 50);
      max-width: calc(var(--vsq) * 50);
      overflow: overlay;
      padding-inline: 0 1rem;
    `.toString()

    $c.innerHTML = `${
      typeof args[0] === 'string' ? args[0] : JSON.stringify(args[0])
    }`

    $log.replaceChildren($f)

    error.apply(args)
  }

  console.log = (...args) => {
    $log.replaceChildren(document.createDocumentFragment())
    log.apply(args)
  }

  if (import.meta.hot) {
    $log.replaceChildren(document.createDocumentFragment())
  }
}
