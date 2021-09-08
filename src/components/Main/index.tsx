import styled, { keyframes } from 'styled-components'

const pulse = keyframes`
  from {
    filter: saturate(0.5);
  }
`

export const Main = styled.main`
  cursor: crosshair;

  > form {
    padding: calc(var(--pad) / 2) var(--pad);
    z-index: 1;
  }

  > section {
    display: flex;
    flex-wrap: wrap;
    gap: max(5px, var(--vsq) / 2);
    overflow: hidden;
    padding: 0 var(--pad) var(--pad);
    place-content: normal;
    place-items: center;
    position: relative;

    > {
      * {
        position: relative;

        > span {
          border: 1px solid currentColor;
          display: block;
          flex: auto 0 0;
          height: max(10px, var(--vsq));
          width: max(10px, var(--vsq));
          border-radius: 2px;
        }

        &:hover > span {
          border-color: var(--accent);
        }

        &:hover:after {
          background: var(--accent);
          border-radius: 2px;
          color: #fff;
          content: attr(name);
          font-size: 12px;
          inset: 0 auto auto calc(100% + 1em);
          padding: 0.5rem;
          pointer-events: none;
          position: absolute;
          white-space: nowrap;
          z-index: 5;
        }
      }

      span > span {
        opacity: 0.6;
      }

      div {
        > span {
          background: currentColor;
        }

        &:last-of-type > span {
          animation: ${pulse} 0.3s ease-in-out infinite alternate;
          color: #f36;
        }
      }
    }
  }
`

export default Main
