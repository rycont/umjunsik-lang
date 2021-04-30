> 어디선가 유입이 자꾸 들어오고 있는것같은데.. 혹시 시간 되신다면 어디쪽 링크 통해서 들어오셨는지 기재 부탁드리겠습니다. [어떻게 엄랭을 발견하셨나요..!](https://github.com/rycont/umjunsik-lang/issues/1)
<!-- ALL-CONTRIBUTORS-BADGE:START - Do not remove or modify this section -->
[![All Contributors](https://img.shields.io/badge/all_contributors-6-orange.svg?style=flat-square)](#contributors-)
<!-- ALL-CONTRIBUTORS-BADGE:END -->

![](https://hits.seeyoufarm.com/api/count/incr/badge.svg?url=https://github.com/rycont/umjunsik-lang)

# 엄랭

**주의: 이미 Umlang이라는 프로젝트가 있기때문에, 꼭 한글로만 표기해주세요.**
영문표기를 해야할때는 "Um. Junsik Lang" (or "Umjunsik-lang")이라고 표기해주세요.

엄랭은 세계 최초의 인물이름으로 만들어진 난해한 프로그래밍 언어입니다. 엄준식이 어떻게 인물 이름이냐고요? 그러게요ㅋㅋ 어떻게 엄준식이 어떻게 사람 이름이지ㅋㅋ "엄준식 사람이름인데요"

# Contributors ✨
<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tr>
    <td align="center"><a href="https://github.com/cstria0106"><img src="https://avatars1.githubusercontent.com/u/11474150?v=4?s=100" width="100px;" alt=""/><br /><sub><b>goorm</b></sub></a><br /><a href="#platform-cstria0106" title="Packaging/porting to new platform">📦</a></td>
    <td align="center"><a href="https://github.com/pl-Steve28-lq"><img src="https://avatars2.githubusercontent.com/u/64412954?v=4?s=100" width="100px;" alt=""/><br /><sub><b>Steve28</b></sub></a><br /><a href="#platform-pl-Steve28-lq" title="Packaging/porting to new platform">📦</a></td>
    <td align="center"><a href="https://trinets.xyz"><img src="https://avatars2.githubusercontent.com/u/39158228?v=4?s=100" width="100px;" alt=""/><br /><sub><b>PMH</b></sub></a><br /><a href="#platform-pmh-only" title="Packaging/porting to new platform">📦</a></td>
    <td align="center"><a href="https://github.com/AkiaCode"><img src="https://avatars0.githubusercontent.com/u/71239005?v=4?s=100" width="100px;" alt=""/><br /><sub><b>Aki</b></sub></a><br /><a href="https://github.com/rycont/umjunsik-lang/commits?author=AkiaCode" title="Code">💻</a></td>
    <td align="center"><a href="https://info.tim23.me"><img src="https://avatars1.githubusercontent.com/u/64291996?v=4?s=100" width="100px;" alt=""/><br /><sub><b>Tim232</b></sub></a><br /><a href="https://github.com/rycont/umjunsik-lang/commits?author=Tim232" title="Code">💻</a></td>
    <td align="center"><a href="https://github.com/Ign0reLee"><img src="https://avatars.githubusercontent.com/u/46403849?v=4?s=100" width="100px;" alt=""/><br /><sub><b>JeYoung</b></sub></a><br /><a href="https://github.com/rycont/umjunsik-lang/commits?author=Ign0reLee" title="Code">💻</a></td>
  </tr>
</table>

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->

<!-- ALL-CONTRIBUTORS-LIST:END -->

# 문법

엄랭은 "엄", "준", "식", "동탄" 네개의 키워드와 "!", ".", " ", "~", "ㅋ" 다섯개의 기호로 코드가 이루어집니다.
모든 프로그램은 "어떻게"로 시작하며, 항상 "이 사람이름이냐ㅋㅋ"로 끝나야 합니다.

## 자료형

정수: 온점, 반점의 갯수로 나타냅니다. 온점의 갯수만큼 1을 더하며, 반점의 갯수만큼 1을 뻅니다다.

```
... => 3
.. => 2
,, => -2
,,, => -3
.,., => 0
```

## 연산자

- 1 증가: `.`
- 1 감소: `,`
- 곱하기: " "(공백)
- 나누기: (미정)

## 변수

변수는 인덱싱(양의 정수)을 통해 접근하고 대입할 수 있습니다. 지정하지 않았을경우 모든 변수의 기본값은 0입니다.

### 대입(엄)

연음의 갯수번째 변수에 뒤에 오는 수를 대입합니다

```
어어엄 => 3번째 변수에 0 지정
어엄 => 2번째 변수에 0 지정
엄.. => 1번째 변수에 2 지정
어엄. => 2번째 변수에 1 지정
엄,,, => 1번째 변수에 -3 지정
```

### 사용(어)

연음의 갯수번째 변수를 불러옵니다

```
어 => 1번째 변수
어어 => 2번째 변수
어어어 => 3번째 변수
```

## 콘솔

### 식?

콘솔에서 정수를 입력받습니다.

```
엄식? => 콘솔을 입력받아서 1번째 변수에 대입한다.
어엄식? => 콘솔을 입력받아서 2번째 변수에 대입한다.
```

### 식!

콘솔에 정수를 출력합니다.

```tsx
식..! => 콘솔에 2 출력
식어! => 콘솔에 첫번째 변수 출력
```

### 식ㅋ

콘솔에 문자를 출력합니다. `식`과 `ㅋ`사이에 오는 정수를 유니코드 문자로 변환하여 콘솔에 출력합니다. `식`과 `ㅋ`사이에 정수가 주어지지 않으면 개행합니다(`식ㅋ` => `\n`)

```tsx
식........... ........ㅋ => 콘솔에 X 출력
```

## 지시문

### 동탄?

`동탄{정수}?{실행할 명령}`으로 작성합니다. 정수가 0이라면 `실행할 명령`이 실행되며, 그렇지 않다면 다음줄로 넘어갑니다.

### 준

`준` 뒤에 오는 정수번째 줄로 이동합니다. `준.. => 2번째 줄(글자)로 이동`. 원라인코드의 경우에는 `~`로 분리된 코드단위로 카운트하여 이동합니다.

### 화이팅!

`화이팅!`뒤에 오는 정수를 반환하며 프로그램을 종료합니다.

## 기타

- 확장자는 `.umm`입니다.
- One-line 작성은 `\n`을 `~`로 치환합니다. (예제: 구구단 참조)

# 구현체
- [디노](https://github.com/rycont/umjunsik-lang/tree/master/umjunsik-lang-deno) : 가장 처음 만들어진 런타임입니다. Deno 1.4.6, Ubuntu 18.04 on WSL 에서 테스트되었습니다.
- [노드JS](https://github.com/rycont/umjunsik-lang/tree/master/umjunsik-lang-node) : Deno 구현체의 NodeJS 포트버전입니다.
- [파이썬](https://github.com/rycont/umjunsik-lang/tree/master/umjunsik-lang-python)
- [웹-엄](https://github.com/rycont/umjunsik-lang/tree/master/umjunsik-lang-web) : PMH님이 [호스팅해주시고 있습니다🎉](https://static.pmh.codes/umjunsik-lang/umjunsik-lang-web/)

# 예제

[위키를 참조해주세요](https://github.com/rycont/umjunsik-lang/wiki)

# To-Do
- [ ] 엄랭아희
- [ ] gnex-umjunsik [What is Gnex?](https://github.com/rycont/Gnex)
- [x] ~~웹-엄~~
- [x] ~~엄랭파이썬~~

# History

- 20200626 0030 : 엄랭 공개
- 20200626 0855 : 엄랭 문서 완성
- 20200625 1256 : 엄랭 Deno 구현체 배포
- 20200804 : 엄랭v2
  1. 모든 콘솔 출력은 인라인
  1. `화이팅!` 후에 오는 문자열을 반환하며 프로그램이 종료
  1. 새 문법 추가: `식ㅋ`
  1. 새 문법 추가: `동탄?`
  1. `화이팅!`의 명세 변경
- 20200805 : 문서 개정
  1. `동탄?` 설명 추가
  1. `화이팅!` 설명 변경
  1. 지시문들을 별도의 단락으로 분리
- 20200912 : [WIP] 99병의 맥주 예제 작업중
- 20200915 : 엄랭v2-엄랭노드 구현체 배포
- 20201017 : 엄랭v2-파이썬 구현체 배포 by [Steve28](https://github.com/pl-Steve28-lq)
- 20201105 : 웹-엄: 자바스크립트로 된 엄랭 처리기(웹런타임) 배포 by [PMH](https://github.com/pmh-only)
