import org.jetbrains.kotlin.gradle.tasks.KotlinCompile

plugins {
    kotlin("jvm") version "1.5.10"
    id("com.github.johnrengelman.shadow") version "2.0.2"
    application
}

group = "com.alphagot"
version = "1.0"

repositories {
    mavenCentral()
    maven("https://jitpack.io")
}

val shade = configurations.create("shade")
shade.extendsFrom(configurations.implementation.get())

tasks {
    compileKotlin {
        kotlinOptions.jvmTarget = "16"
    }
    compileTestKotlin {
        kotlinOptions.jvmTarget = "16"
    }

    jar {
        duplicatesStrategy = DuplicatesStrategy.EXCLUDE

        from(
            shade.map {
                if (it.isDirectory)
                    it
                else
                    zipTree(it)
            }
        )
        this.manifest.attributes(Pair("Main-Class", "com.alphagot.kumjunsiklang.MainKt"))
    }
}

application {
    mainClass.set("com.alphagot.kumjunsiklang.MainKt")
}