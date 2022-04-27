export default (code: string): string =>
  code
    .replace(
      /\\(?:\r\n|\n\r|\n|\r)|\/\*.*?\*\/|\/\/(?:\\(?:\r\n|\n\r|\n|\r)|[^\n\r])*/g,
      ''
    )
    .split(/\n+/)
    .reduce((result, line) => {
      line = line.trim().replace(/\s{2,}|\t/, ' ')

      if (line.charAt(0) === '#') {
        result.push(line, '\n')
      } else {
        result.push(
          line.replace(
            /\s*({|}|=|\*|,|\+|\/|>|<|&|\||\[|\]|\(|\)|-|!|;)\s*/g,
            '$1'
          ),
          '\n'
        )
      }

      return result
    }, [])
    .join('')
    .replace(/\n+/g, '\n')
