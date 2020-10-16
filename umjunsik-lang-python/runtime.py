import sys

class Umjunsik:
    def __init__(self):
        self.data = [0]*256

    def str2data(self, code):
        return eval("*".join(list(map(lambda cmd:str((self.data[cmd.count("어")-1] if cmd.count("어") else 0) + cmd.count(".") - cmd.count(",")), code.split(" ")))))

    def type(self, code):
        if "동탄" in code: return "IF"
        if "준" in code: return "MOVE"
        if "화이팅!" in code: return "END"
        if "식" in code and "?" in code: return "INPUT"
        if "식" in code and "!" in code: return "PRINT"
        if "식" in code and "ㅋ" in code: return "PRINTASCII"
        if "엄" in code: return "DEF"

    def compileLine(self, code):
        if code == "": return None
        TYPE = self.type(code)
        
        if TYPE == "DEF":
            var, cmd = code.split("엄")
            self.data[var.count("어")] = self.str2data(cmd)
        if TYPE == "END":
            print(self.str2data(code.split("화이팅!")[1]), end="")
            sys.exit()
        if TYPE == "INPUT":
            self.data[code.replace("식?", "").count("어")] = int(input())
        if TYPE == "PRINT":
            print(self.str2data(code[1:-1]), end="")
        if TYPE == "PRINTASCII":
            value = self.str2data(code[1:-1])
            print(chr(value) if value else "\n", end="")
        if TYPE == "IF":
            cond, cmd = code.replace("동탄", "").split("?")
            if self.str2data(cond) == 0: return cmd
        if TYPE == "MOVE":
            return self.str2data(code.replace("준", ""))

    def compile(self, code, check=True):
        jun = False
        recode = ""
        spliter = "\n" if "\n" in code else "~"
        code = code.split(spliter)
        if check and (code[0] != "어떻게" or code[-1] != "이 사람이름이냐!"):
            raise SyntaxError("어떻게 이게 엄랭이냐!")
        index = 0
        error = 0
        while index < len(code):
            errorline = index
            c = code[index]
            res = self.compileLine(c)
            if jun:
                jun = False
                code[index] = recode                
            if isinstance(res, int): index = res-2
            if isinstance(res, str):
                recode = code[index]
                code[index] = res
                index -=1
                jun = True

            index += 1
            error += 1
            if error == 100000:
                raise RecursionError(str(errorline+1) + "번째 줄에서 무한 루프가 감지되었습니다.")
