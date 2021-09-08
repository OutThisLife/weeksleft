import 'normalize.css'
import React from 'react'
import { render } from 'react-dom'
import App from './app'
import GlobalStyles from './style'

render(
  <React.StrictMode>
    <App />
    <GlobalStyles />
  </React.StrictMode>,
  document.getElementById('root')
)
