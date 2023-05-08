import sys
from .umjunsik import Umjunsik

def main():
    if len(sys.argv) != 2:
        print("Usage: umjunsik-interpreter <filename.umm>")
        sys.exit(1)

    filename = sys.argv[1]
    with open(filename, 'r', encoding='utf-8') as file:
        code = file.read()

    interpreter = Umjunsik()
    interpreter.compile(code)

if __name__ == "__main__":
    main()