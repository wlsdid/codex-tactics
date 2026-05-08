# Battle Scene 자동 생성 가이드

이 문서는 Unity에서 전투 테스트용 UI를 직접 배치하지 않고 자동으로 만드는 방법을 정리한다.

## 1. 자동 생성 스크립트 위치

```text
Assets/Editor/BattleSceneAutoBuilder.cs
```

Unity Editor 전용 스크립트라서 게임 실행 파일에는 포함되지 않는다.

## 2. 사용 방법

1. Unity Hub에서 프로젝트를 연다.

```text
C:\Users\jywls\Desktop\game_portfolio
```

2. Unity가 스크립트 컴파일을 끝낼 때까지 기다린다.
3. 상단 메뉴에서 아래 항목을 누른다.

```text
Tools > Codex Tactics > Create Battle Test Scene
```

4. 자동으로 아래 씬이 생성된다.

```text
Assets/Scenes/BattleScene.unity
```

5. 생성된 씬에서 Play 버튼을 누른다.
6. 아래 버튼을 눌러 전투를 테스트한다.

```text
Attack
Fire Skill
End Turn
```

## 3. 자동으로 만들어지는 오브젝트

```text
BattleScene
├─ Main Camera
├─ Canvas
│  ├─ Player HP Text
│  ├─ Player AP Text
│  ├─ Enemy HP Text
│  ├─ Enemy Status Text
│  ├─ Message Text
│  ├─ Attack Button
│  ├─ Fire Skill Button
│  └─ End Turn Button
├─ EventSystem
└─ BattleManager
```

`BattleManager`의 Inspector 연결도 자동으로 들어간다.

## 4. 테스트할 것

| 테스트 | 기대 결과 |
|---|---|
| Attack 클릭 | 적 HP 감소 |
| Fire Skill 클릭 | AP 2 소모, 적 HP 감소, Burn 적용 |
| End Turn 클릭 | 공격하지 않고 적 턴으로 넘어감 |
| Burn 상태 | 적 턴 시작에 피해 3 적용 |
| 적 HP 0 이하 | 승리 메시지 표시 |
| 플레이어 HP 0 이하 | 패배 메시지 표시 |

## 5. 주의

- 이 UI는 포트폴리오 최종 디자인이 아니라 테스트용 임시 UI다.
- 나중에 픽셀 아트 스타일 버튼, HP 바, 상태 아이콘으로 교체하면 된다.
- 만약 TextMeshPro 관련 팝업이 나오면 `Import TMP Essentials`를 누르면 된다.
