import { createGlobalStyle } from 'styled-components'

export default createGlobalStyle`
:root {
  --vsq: calc((1vw + 1vh) / 2);
  --pad: calc(var(--vsq) * 4);

  --fg: #000;
  --bg: #fff;
  --accent: #f36;

  @media (prefers-color-scheme: dark) {
    --fg: #333;
    --bg: #000;
  }
}

* {
  box-sizing: border-box;
  margin: 0;
  padding: 0;
}

html {
  line-height: 1;
  font-size: clamp(13px, calc(var(--vsq) * 1.5), 14px);
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Open Sans', 'Helvetica Neue', sans-serif;
}

body {
  color: var(--fg);
  background: var(--bg);
}
`
