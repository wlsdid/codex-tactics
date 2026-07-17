# 전투 액션 피드백 최종 수정·검증 보고서

- 날짜: 2026-07-17
- 프로젝트: `GamePortfolio`
- 최종 판정: **PASS**

## 1. 구현 내용

- ATTACK: Paladin 48px 전진(lunge), Goblin 타격 흔들림·burst·`-20` popup
- Fire: 발사체 이동 후 target impact, `-45`·`BURN` popup
- Ice / Lightning: 각 속성별 impact 및 상태 popup
- Guard / Earth: 시전자 위치의 self-only pulse와 상태 popup
- 피해·상태·Break 적용 시점을 presentation impact 프레임으로 통일
- 동일 impact 중복 적용 방지 유지

## 2. 코드 리뷰 후 추가 수정

### 처치 타격 VFX 누락

- presentation 시작 시 대상 `RectTransform`을 캐시하도록 변경
- 피해 적용 후 선택 indicator가 사라져도 캐시된 대상 body에 타격 VFX와 popup이 출력됨

### coroutine cleanup 안전성

- action feedback coroutine을 추적하고 battle restart/end 시 모두 중단
- actor/target 위치와 색상 복원
- 이전 액션 coroutine이 새 UI 상태를 덮어쓰는 문제 방지
- 정상 액션 종료 시 0.8초 popup은 끝까지 재생되도록 transient와 motion cleanup을 분리

### 지속 상태 overlay 회귀

- Burn/Stun/Guard/Shield/DONE overlay 계산을 유지
- transient impact 중에는 큰 사각 overlay를 숨겨 순간 피드백과 캐릭터 얼굴을 가리지 않음
- action 종료 후 저알파 지속 overlay와 상태 텍스트가 다시 표시됨

### Break 적용 시점

- weakness Break 감소·Break 보너스·Reset을 명령 입력 시점이 아닌 impact 시점에 원자적으로 적용
- impact 전에는 HP/상태/Break gauge가 변하지 않음

### 런타임 안전성

- impact 시 skill/actor/target null·index·사망 상태 재검증
- 외부 상태 변화가 있어도 죽은 대상에 재피해를 적용하지 않음

## 3. 자동 검증

| 검증 | 결과 |
|---|---|
| Unity compile/import | PASS (`exit 0`) |
| `BattleAutoTestRunner.RunBattleLogicAutoTest` | PASS |
| `BattleSceneAutoBuilder.ValidateBattleTestScene` | PASS |
| `GameFlowSceneAutoBuilder.ValidateGameFlowScenes` | PASS |
| Capture build | PASS (`exit 0`) |
| Capture runtime | PASS (`exit 0`) |
| `git diff --check` | PASS |

추가 회귀 테스트:

- lethal ATTACK target cache 및 처치 impact VFX
- battle restart 시 projectile/popup/motion coroutine cleanup
- ATTACK lunge 35–60px 런타임 범위
- Fire projectile 비행 및 impact 도착
- Guard actor-local pulse
- Break gauge impact 전 불변 / impact 시 감소·reset
- transient 종료 후 지속 Burn overlay 복원

최종 runtime QA 로그:

- ATTACK lunge: PASS
- ATTACK impact + damage popup: PASS
- Fire projectile flight/arrival + BURN: PASS
- Guard self pulse/popup: PASS
- lethal target cached impact: PASS
- restart cleanup: PASS

## 4. 최종 캡처

- `Docs/Captures/05_attack_impact.png`
- `Docs/Captures/06_fire_burn.png`
- `Docs/Captures/07_guard_feedback.png`

세 캡처 모두 1920×1080이며 다음을 육안 확인함:

- 3대3 배치, 전투 배경, HP 표시, 하단 action dock 유지
- impact와 popup의 대상 위치 일치
- 큰 지속 사각 overlay가 transient 피드백을 가리지 않음

## 5. 변경 파일

- `Assets/Scripts/Battle/BattleManager.cs`
- `Assets/Scripts/Battle/BattleUI.cs`
- `Assets/Editor/BattleAutoTestRunner.cs`
- `Assets/Scripts/ScreenshotCaptureJob.cs`
- `Docs/Captures/05_attack_impact.png`
- `Docs/Captures/06_fire_burn.png`
- `Docs/Captures/07_guard_feedback.png`
