package com.alphagot.kumjunsiklang

import java.util.*

fun String.countChar(c: String): Int {
    var ret = 0
    this.toCharArray().forEach {
        if(it.toString() == c){
            ret++
        }
    }
    return ret
}

fun String.toUmmInt(): Int {
    return toUmmmInt(this)
}

fun toUmmmInt(a: String): Int {
    var n = 0
    if (a.contains(" ")) {
        val b = a.split(" ").map {
            toUmmmIntInternal(it)
        }
        n = b[0]
        b.subList(1, b.size).forEach {
            n *= it
        }
    }
    else {
        n = toUmmmIntInternal(a)
    }
    return n
}

fun toUmmmIntInternal(a: String): Int {
    var s = a
    var n = 0

    if(s.contains("식?")) {
        val answer = Scanner(System.`in`).nextInt()
        s = s.replace("식?", ".".repeat(answer))
    }
    if(s.contains("어")) n += memory.getSafe(s.countChar("어") - 1)
    if(s.contains(".")) n += s.countChar(".")
    if(s.contains(",")) n -= s.countChar(",")
    return n
}

fun String.toVarIndex(isLoad: Boolean = true): Int {
    if(this.split(" ").size > 1){
        val s = this.split(" ")

        val b = mutableListOf<Int>()
        s.forEach { b.add(it.countChar("어") - 1) }

        var r = 1

        b.forEach {
            r *= it
        }

        return r + if(isLoad) -1 else 0
    }
    else {
        return this.countChar("어") + if(isLoad) -1 else 0
    }
}

fun String.isVariable(): Boolean = this.startsWith("어") || this.endsWith("엄")

fun convertUnicodeToString(unicodeString: String): String {
    var str: String = unicodeString.split(" ")[0]
    str = str.replace("\\", "")
    val arr = str.split("u").toTypedArray()
    var text = ""
    for (i in 1 until arr.size) {
        val hexVal = arr[i].toInt(16)
        text += hexVal.toChar()
    }

    return text
}
