import { readFileStr } from 'https://deno.land/std/fs/mod.ts';
import { readLines } from "https://deno.land/std@v0.51.0/io/bufio.ts";

export const run = async (content: string) => {
  let proceed = content.split('\n')
  if(proceed[0] === '어떻게' && proceed.slice(-1)[0] === '이 사람이름이냐!') {
    console.log("Error: 어떻게 이 코드가 엄랭이냐ㅋㅋ")
    return
  }
  const variables: number[] = []
  if(content.includes('~')) proceed = content.split('~')
  let pointer = 0;
  const numberify = async (a: string): Promise<number> => {
    let numbered = 0;
    if(a.includes(' ')) return await numberify(a.split(' ')[0]) * await numberify(a.split(' ')[1])
    if(a.includes('식?')) {
      for await (const line of readLines(Deno.stdin)) {
        a = a.replace('식?', '.'.repeat(Number(line)))
        break
      }
    }
    if(a.includes('어')) numbered += variables[a.split('어').length - 1]
    if(a.includes('.')) numbered += a.split('.').length - 1
    if(a.includes(',')) numbered += a.split(',').length - 1
    return numbered
  }
  while(proceed[pointer] !== '이 사람이름이냐!') {
    const operation = proceed[pointer++]
    if(operation.includes('엄')) { // 변수대입
      const variablePointer = operation.split('엄')[0].split('어').length
      const setteeValue = await numberify(operation.split('엄')[1])
      variables[variablePointer] = setteeValue
      continue
    }
    if(operation.includes('식') && operation.includes('!')) { // 콘솔출력
      console.log(await numberify(operation.slice(1, -1)))
    }
    if(operation.includes('준')) { //GOTO
      pointer = await numberify(operation.split('준')[1]) - 1
    }
    if(operation === '화이팅!') return numberify(operation.split('화이팅!')[1])
  }
}

const bootstrap = async (filepath: string) => {
  try {
    if(await Deno.stat(filepath)) run(await readFileStr(filepath))
  } catch(e) {
    console.log(`Error: ${filepath}가 어떻게 파일이름이냐ㅋㅋ`)
  }
}

if(Deno.args[0]) bootstrap(Deno.args[0])