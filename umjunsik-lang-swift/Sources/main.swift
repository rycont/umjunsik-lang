//
//  main.swift
//
//
//  Created by Soongyu Kwon on 9/20/23.
//
// The Swift Programming Language
// https://docs.swift.org/swift-book

if (CommandLine.arguments.count != 2) {
    print("어떻게 파일 이름이 없냐ㅋㅋ")
    exit(0)
}

let filename = CommandLine.arguments[1]

let interpreter = Umjunsik()
interpreter.compilePath(path: filename)
