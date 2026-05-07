# Devlog - 2026-05-08 - SkillData 기초 구조 추가

## 오늘 한 일

- 스킬 데이터 구조 `SkillData` 추가
- 속성 enum `ElementType` 추가
- 상태이상 enum `StatusEffectType` 추가
- `CharacterData`에 약점 속성 `weaknessElement` 추가
- `BattleManager`의 기본 공격을 `SkillData` 기반 계산으로 변경
- 약점 속성이 일치하면 데미지가 +10 증가하는 초안 규칙 추가

## 왜 이 작업을 했는가

앞으로 AP 스킬, 약점 공격, Break, 상태이상을 넣으려면 전투 코드 안에 숫자와 문자열을 직접 쓰는 방식으로는 금방 복잡해진다. 그래서 먼저 스킬과 속성 데이터를 따로 관리할 수 있는 작은 구조를 만들었다.

## 테스트/확인 방법

Unity에서 BattleManager가 붙은 오브젝트를 선택한 뒤 Inspector에서 다음을 확인한다.

1. `Enemy Weakness`를 `Fire`로 설정
2. `Basic Skill Element`를 `Fire`로 설정
3. Play 실행 후 공격 버튼 클릭
4. 기본 스킬 위력보다 10 높은 데미지가 들어가는지 확인

## 막힌 점

- 현재 cron 환경에서는 Unity Editor를 직접 실행해 씬을 검증하지 않았다.
- 그래서 씬/프리팹 같은 Unity 바이너리성 파일은 건드리지 않고 C# 스크립트와 문서만 수정했다.

## 다음 목표

- AP 값을 캐릭터 또는 BattleManager에 추가한다.
- 스킬 사용 시 AP가 충분한지 검사한다.
- AP가 부족할 때 버튼/메시지로 알려주는 흐름을 만든다.
