package com.alphagot.kumjunsiklang

import java.io.File
import java.io.IOException
import java.nio.ByteBuffer
import java.util.*
import kotlin.system.exitProcess

val memory: MutableList<Int> = mutableListOf()

var pc: Int = 0

fun MutableList<Int>.getSafe(index: Int): Int {
    return if(index > this.lastIndex) 0 else this[index]
}

fun MutableList<Int>.setSafe(index: Int, value: Int) {
    if(index > this.lastIndex){
        for(i in 0 until index - this.lastIndex){
            this.add(0)
        }
    }
    this[index] = value
}

fun parseLine(code: String){
    if(code.startsWith("화이팅!")){
        exitProcess(memory.getSafe(code.replace("화이팅!", "").toUmmInt()))
    }
    else if(code.endsWith("식?")){
        if(code.replace("식?", "") == ""){
            throw java.lang.RuntimeException("문법 오류: 대입할 변수 미지정 (${pc + 1}번 줄)")
        }
        else {
            val sc = Scanner(System.`in`)
            memory.setSafe(code.replace("식?", "").toUmmInt(), sc.nextInt())
        }
    }
    else if(code.startsWith("식")){
        if(code.endsWith("!")){
            val q = code.replace("식", "").replace("!", "")
            print("${q.toUmmInt()}")
        }
        else if(code.endsWith("ㅋ")){
            val q = code.replace("식", "").replace("ㅋ", "")
            if(q == ""){
                println()
            }
            else {
                val b = q.toUmmInt()

                print(convertUnicodeToString("\\u${b.toString(16).padStart(4 - b.toString(16).length, '0')}"))
            }
        }
    }
    else if(code.startsWith("동탄")){
        if(code.contains("?")){
            val cc = code.replace("동탄", "").split("?")

            val r = cc[0].toUmmInt()

            if(r == 0){
                parseLine(cc[1])
            }
        }
    }
    else if(code.startsWith("준")){
        if(code.replace("준", "") == ""){
            throw java.lang.RuntimeException("문법 오류: 점프할 줄 번호 미지정 (${pc + 2}번 줄)")
        }
        else {
            val lPC = code.replace("준", "")

            pc = lPC.toUmmInt() - 3
        }
    }
    else if(code.contains("엄")){
        val c = code.split("엄")

        memory.setSafe(c[0].countChar("어"), c[1].toUmmInt())
    }
    else {
        return
    }
    return
}

fun main(args: Array<String>) {
    if(args.isEmpty()){
        throw IOException("어떻게 파일 이름이 없냐ㅋㅋ")
    }
    else if(!args[0].endsWith(".umm")){
        throw IOException("어떻게 ${args[0]}이 파일이름이냐ㅋㅋ")
    }

    val f = File(args[0])
    if(!f.canRead()) {
        throw IOException("파일을 읽을 수 없음")
    }
    else {
        var codes = f.readText().trim().replace("~", "\n").split("\n")

        if(codes[0] != "어떻게" && codes.last() != "이 사람이름이냐ㅋㅋ"){
            throw Exception("어떻게 이게 엄랭이냐ㅋㅋ")
        }

        codes = codes.subList(1, codes.size)

        while(true){
            if(codes[pc] != "이 사람이름이냐ㅋㅋ"){
                parseLine(codes[pc].trim())
                pc++
            }
            else break
        }
    }

    exitProcess(0)
}