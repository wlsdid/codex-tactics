# Unity 세팅 가이드

## 프로젝트 생성

1. Unity Hub 실행
2. New Project 클릭
3. 템플릿: 2D Core 선택
4. 프로젝트 이름: CodexTactics
5. 위치: `C:\Users\jywls\Desktop\game_portfolio`

## 스크립트 배치

이미 준비된 스크립트 위치:

```text
Assets/Scripts/Data/CharacterData.cs
Assets/Scripts/Battle/BattleState.cs
Assets/Scripts/Battle/BattleManager.cs
```

Unity 프로젝트를 만든 뒤 같은 `Assets` 폴더 구조로 유지하면 된다.

## BattleScene 만들기

1. `Assets/Scenes/BattleScene.unity` 생성
2. Canvas 생성
3. TextMeshPro Text 3개 생성
   - PlayerHpText
   - EnemyHpText
   - MessageText
4. Button 1개 생성
   - AttackButton
5. 빈 오브젝트 생성
   - 이름: BattleManager
6. BattleManager.cs 추가
7. Inspector에서 Text와 Button 연결
8. Play 버튼으로 테스트

## 예상 동작

- 시작하면 “전투 시작!” 표시
- 공격 버튼 클릭 시 적 HP 감소
- 1초 뒤 적이 자동 공격
- HP가 0이 되면 승리/패배 메시지 표시
