import {promises as fs} from 'fs'
import readline from 'readline-sync'
  
function stringify(unicode: number) {
  return String.fromCharCode(unicode)
}

async function run(code: string) {
  const statements = code.trim().split(code.includes('~') ? '~' : '\n').map(line => line.trim())

  if(statements[0] !== '어떻게' || !statements.slice(-1)[0].startsWith('이 사람이름이냐')) {
    throw new Error('Error: 어떻게 이 코드가 엄랭이냐ㅋㅋ')
  }
 
  const variables: number[] = []
  let pointer = 0

  const evaluate = async (x: string): Promise<number> => {
    let n = 0
    if(x.includes(' ')) return (await Promise.all(x.split(' ').map(evaluate))).reduce((a, b) => a * b)
    if(x.includes('식?')) {
      const answer= readline.question();
      x = x.replace('식?', '.'.repeat(Number(answer)))
    }
    if(x.includes('어')) n += variables[x.split('어').length - 1]
    if(x.includes('.')) n += x.split('.').length - 1
    if(x.includes(',')) n -= x.split(',').length - 1
    return n
  }

  const execute = async (statement: string): Promise<number | undefined> => {
    if (statement.includes('동탄') && statement.includes('?')) { // IF GOTO
      const condition = await evaluate(statement.substring(2, statement.lastIndexOf('?') + 1))
      if (condition === 0) {
        return execute(statement.substr(statement.lastIndexOf('?') + 1))
      }
      return
    }
    
    if(statement.includes('엄')) {
      const variablePointer = statement.split('엄')[0].split('어').length
      const setteeValue = await evaluate(statement.split('엄')[1])
      variables[variablePointer] = setteeValue
    }
  
    if (statement.includes('식') && statement[statement.length - 1] === '!') {      
      process.stdout.write(String(await evaluate(statement.slice(1, -1))))
    }
  
    if (statement.includes('식') && statement[statement.length - 1] === 'ㅋ') {
      if (statement === '식ㅋ') process.stdout.write('\n')
      process.stdout.write(stringify(await evaluate(statement.slice(1, -1))))
    }
  
    if(statement.includes('준')) {
      pointer = await evaluate(statement.split('준')[1]) - 1
    }
  
    if (statement.indexOf('화이팅!') === 0) {
      return evaluate(statement.split('화이팅!')[1])
    }
  }
 
  while(!statements[pointer].startsWith('이 사람이름이냐')) {    
    const statement = statements[pointer++]
    const evaluated = await execute(statement)
    if(evaluated) return evaluated
  }
}
 
async function bootstrap(path: string) {
  try {
    try {
      await fs.access(path)
    } catch(e) {
      throw new Error(`Error: ${path}가 어떻게 파일이름이냐ㅋㅋ`)
    }

    await run((await fs.readFile(path, 'utf-8')))
  } catch(e) {
    process.stderr.write(`${e.message}\n`)
  }
}
 
if(process.argv[2]) bootstrap(process.argv[2])