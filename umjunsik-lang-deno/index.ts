import { readLines } from "https://deno.land/std@v0.68.0/io/bufio.ts";

export const run = async (content: string) => {
  let proceed = content.split("\n");
  const encoder = new TextEncoder();
  if (
    !(proceed[0] === "어떻게" && proceed.slice(-1)[0] === "이 사람이름이냐!")
  ) {
    new Error("어떻게 이 코드가 엄랭이냐ㅋㅋ");
  }
  const variables: number[] = [];
  if (content.includes("~")) proceed = content.split("~");
  let pointer = 0;
  const numberify = async (a: string): Promise<number> => {
    let numbered = 0;
    if (a.includes(" "))
      return (await Promise.all(a.split(" ").map(numberify))).reduce(
        (a, b) => a * b
      );
    if (a.includes("식?")) {
      for await (const line of readLines(Deno.stdin)) {
        a = a.replace("식?", ".".repeat(Number(line)));
        break;
      }
    }
    if (a.includes("어")) numbered += variables[a.split("어").length - 1];
    if (a.includes(".")) numbered += a.split(".").length - 1;
    if (a.includes(",")) numbered -= a.split(",").length - 1;
    return numbered;
  };
  const stringify = (unicode: number) => {
    return String.fromCharCode(unicode);
  };

  const parseOperation = async (
    operation: string
  ): Promise<number | undefined> => {
    if (operation.includes("동탄") && operation.includes("?")) {
      //IFGOTO
      const condition = await numberify(
        operation.substring(2, operation.lastIndexOf("?") + 1)
      );
      if (condition === 0) {
        return parseOperation(operation.substr(operation.lastIndexOf("?") + 1));
      }
      return;
    }
    if (operation.includes("엄")) {
      const variablePointer = operation.split("엄")[0].split("어").length;
      const setteeValue = await numberify(operation.split("엄")[1]);
      variables[variablePointer] = setteeValue;
    }
    if (operation.includes("식") && operation[operation.length - 1] === "!") {
      await Deno.stdout.write(
        encoder.encode(String(await numberify(operation.slice(1, -1))))
      );
    }
    if (operation.includes("식") && operation[operation.length - 1] === "ㅋ") {
      if (operation === "식ㅋ") console.log();
      await Deno.stdout.write(
        encoder.encode(stringify(await numberify(operation.slice(1, -1))))
      );
    }
    if (operation.includes("준")) {
      pointer = (await numberify(operation.split("준")[1])) - 1;
    }
    if (operation.indexOf("화이팅!") === 0) {
      return numberify(operation.split("화이팅!")[1]);
    }
  };

  while (proceed[pointer] !== "이 사람이름이냐!") {
    const operation = proceed[pointer++];
    const evaludated = await parseOperation(operation);
    if (evaludated) return evaludated;
  }
};

const bootstrap = async (filepath: string) => {
  try {
    if (await Deno.stat(filepath)) run(await Deno.readTextFile(filepath));
  } catch (e) {
    new Error(`${filepath}가 어떻게 파일이름이냐ㅋㅋ`);
  }
};

if (Deno.args[0]) bootstrap(Deno.args[0]);
