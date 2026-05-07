# C# / Unity 공부 로드맵

## 공부 원칙

C# 문법을 전부 외우고 시작하지 않는다. 게임 구현에 필요한 개념만 바로 배우고 적용한다.

## 1주차 필수 개념

1. 변수와 자료형
2. 조건문 / 반복문
3. 함수
4. class
5. enum
6. List
7. Unity MonoBehaviour
8. Button OnClick 연결

## C를 알고 있을 때 주의할 점

- C#은 포인터를 거의 직접 쓰지 않는다.
- 메모리 해제를 직접 하지 않아도 된다.
- struct보다 class를 더 자주 사용한다.
- Unity에서는 `Start()`, `Update()` 같은 생명주기 함수가 중요하다.

## 프로젝트와 연결해서 공부하기

| 배울 개념 | 프로젝트 적용 |
|---|---|
| class | Character 데이터 묶기 |
| enum | BattleState 관리 |
| 함수 | Attack(), TakeDamage() |
| List | 파티원/적 목록 관리 |
| UI Button | 공격 버튼 연결 |
| Coroutine | 적 턴 대기 연출 |
