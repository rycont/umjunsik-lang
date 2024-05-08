//
//  Umjunsik.swift
//
//
//  Created by Soongyu Kwon on 9/20/23.
//

import Foundation

class Umjunsik {
    var data: [Int]
    
    init() {
        self.data = Array(repeating: 0, count: 256)
    }
    
    func toNumber(code: String) -> Int {
        let tokens: [String] = code.components(separatedBy: " ")
        var result: Int = 1
        
        for token in tokens {
            let num = ((token.filter{ $0 == "어" }.count != 0) ? self.data[token.filter{ $0 == "어" }.count-1] : 0) + token.filter{ $0 == "." }.count - token.filter{ $0 == "," }.count
            result *= num
        }
        
        return result
    }
    
    static func type(code: String) -> String? {
        if code.contains("동탄") {
            return "IF"
        }
        if code.contains("준") {
            return "MOVE"
        }
        if code.contains("화이팅!") {
            return "END"
        }
        if code.contains("식") && code.contains("?") {
            return "INPUT"
        }
        if code.contains("식") && code.contains("!") {
            return "PRINT"
        }
        if code.contains("식") && code.contains("ㅋ") {
            return "PRINTASCII"
        }
        if code.contains("엄") {
            return "DEF"
        }
        
        return nil
    }
    
    func compileLine(code: String) -> Any? {
        if code.isEmpty {
            return nil
        }
        
        guard let TYPE = Umjunsik.type(code: code) else {
            return nil
        }

        if TYPE == "DEF" {
            let parts = code.components(separatedBy: "엄")
            let variable = parts[0]
            let cmd = parts[1]
            self.data[variable.filter{ $0 == "어" }.count] = self.toNumber(code: cmd)
        } else if TYPE == "END" {
            exit(Int32(self.toNumber(code: code.components(separatedBy: "화이팅!")[1])))
        } else if TYPE == "INPUT" {
            self.data[code.replacingOccurrences(of: "식?", with: "").filter{ $0 == "어" }.count] = Int(readLine() ?? "") ?? 0
        } else if TYPE == "PRINT" {
            print(self.toNumber(code: String(code.dropFirst().dropLast())), terminator: "")
        } else if TYPE == "PRINTASCII" {
            let value = self.toNumber(code: String(code.dropFirst().dropLast()))
            print((value != 0) ? String(UnicodeScalar(UInt8(value))) : "\n", terminator: "")
        } else if TYPE == "IF" {
            let parts = code.replacingOccurrences(of: "동탄", with: "").components(separatedBy: "?")
            let cond = parts[0]
            let cmd = parts[1]
            if self.toNumber(code: cond) == 0 {
                return cmd
            }
        } else if TYPE == "MOVE" {
            return self.toNumber(code: code.replacingOccurrences(of: "준", with: ""))
        }
        
        return nil
    }
    
    func compile(code: String, check: Bool = true, errors: Int = 100000) {
        var jun: Bool = false
        var recode: String = ""
        let splitter: String = code.contains("\n") ? "\n" : "~"
        var splittedCode = code.components(separatedBy: splitter)
        
        if check && (splittedCode[0].replacingOccurrences(of: " ", with: "") != "어떻게" || splittedCode[splittedCode.count-1] != "이 사람이름이냐ㅋㅋ" || !splittedCode[0].hasPrefix("어떻게")) {
            fatalError("어떻게 이게 엄랭이냐!")
        }
        
        var index: Int = 0
        var error: Int = 0
        while (index < splittedCode.count) {
            let errorline: Int = index
            let c: String = splittedCode[index].trimmingCharacters(in: .whitespacesAndNewlines)
            let res = self.compileLine(code: c)
            
            if jun {
                jun = false
                splittedCode[index] = recode
            }
            if res is Int {
                index = res as! Int - 2
            }
            if res is String {
                recode = splittedCode[index]
                splittedCode[index] = res as! String
                index -= 1
                jun = true
            }
            
            index += 1
            error += 1
            if error == errors {
                fatalError("\(errorline + 1)번째 줄에서 무한 루프가 감지되었습니다.")
            }
        }
    }
    
    func compilePath(path: String) {
        do {
            var code = try String(contentsOfFile: path, encoding: .utf8)
            code = code.trimmingCharacters(in: .whitespacesAndNewlines)
            self.compile(code: code)
        } catch {
            fatalError("어떻게 이게 파일이냐ㅋㅋ")
        }
    }
}
