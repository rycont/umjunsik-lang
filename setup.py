from setuptools import setup, find_packages

setup(
    name="umjunsik",
    version="2.0.2",
    packages=find_packages(),
    entry_points={
        "console_scripts": [
            "umjunsik=umjunsik-lang-python.__main__:main",
        ],
    },
    author="rycont",
    author_email="rycont@outlook.kr",
    description="어떻게 엄준식이 언어이름이냐🤣",
    long_description=open("../README.md", "r", encoding="UTF-8").read(),
    long_description_content_type="text/markdown",
    url="https://github.com/rycont/umjunsik-lang",
    classifiers=[
        "Intended Audience :: Developers",
        "License :: OSI Approved :: MIT License",
        "Programming Language :: Python :: 3",
        "Programming Language :: Python :: 3.7",
        "Programming Language :: Python :: 3.8",
        "Programming Language :: Python :: 3.9",
        "Programming Language :: Python :: 3.10",
    ],
)