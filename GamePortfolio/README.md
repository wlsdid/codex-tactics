# GamePortfolio — 2D Turn-Based RPG Prototype

Unity 2D 턴제 RPG 포트폴리오용 전투 프로토타입입니다. 37개 배치에 걸쳐 단계적으로 구축된 완전한 전투 시스템을 갖추고 있습니다.

**영문 README:** [`README.EN.md`](README.EN.md)

---

## 게임 루프

```
타이틀 화면 → 스테이지 선택 → 전투(6스테이지 × 2 encounter = 12 전투) → 결과/보너스 → 다음 encounter → 스테이지 선택
```

## 주요 시스템 (37 Batches)

### 전투 시스템
- **6개 스테이지** — Fire, Nature, Earth, Lightning, Dark, Light (각 2 encounters)
- **5개 플레이어 스킬** — Slash(Phys/0AP), Ice Lance(Ice+Stun/1AP), Earth Wall(Earth+Shield/2AP), Fire Bolt(Fire+Burn/2AP), Lightning Strike(Lightning/3AP)  
- **스킬 잠금 해제** — 스테이지 클리어 시 순차적으로 해제
- **속성 시스템** — 1.5x 약점 배율, 7개 원소 (Fire/Ice/Lightning/Nature/Dark/Light/Earth)
- **상태이상** — Burn(도트 데미지), Stun(턴 스킵), Shield(데미지 흡수), Break(1.5x 보너스), Enrage(HP<30%)
- **난이도 모드** — Normal/Hard (enemy HP x1.5, damage x1.3, Break x2)

### 아이템
- **Potion** — HP 30 회복 (3개)
- **Ether** — AP 2 회복 (2개)
- Auto-battle: Potion (HP<30%), Ether (AP<1) 자동 사용

### AI & 자동화
- **Auto-Battle AI** — Guard → Item → Weakness → Lightning → Ice → Earth → Fire → Basic (우선순위 기반)
- **Enrage AI** — HP 30% 이하에서 공격력 1.5x + ENRAGED! 표시

### UI & 시각 효과
- **다이나믹 HP/AP 바** — 초록→노랑→빨강 (HP), 파랑→시안→주황 (AP)
- **2x 속도 토글**
- **상태이상 오버레이** — Burn(빨강 펄스), Stun(파랑 펄스), Broken(흰색)
- **스크린 쉐이크** — 강공격 시 카메라 흔들림
- **히트 플래시** — 적(흰색)/플레이어(빨강) 스프라이트 점멸
- **스킬 발사체** — 속성별 색상 (Fire=주황, Ice=파랑, Lightning=노랑, Earth=초록)
- **페이드 전환** — encounter 간 검은색 페이드 인/아웃
- **일시정지 메뉴** — Resume / Stage Select

### 저장 & 진행
- **JSON 저장** — 완료한 스테이지 영구 저장
- **레벨/XP** — 스테이지 클리어 시 XP 획득, 레벨업 시 Max HP +20
- **스테이지 보너스** — NoDamage(+50g), FastClear(+30g), SkillMastery(+20g), PerfectGuard(+15g), ItemFree(+10g)

### 오디오
- **BGM** — 전투/승리 BGM 훅
- **SFX** — 공격/스킬/가드/피격/승리/패배 효과음
- **AudioManager** — 싱글톤, DontDestroyOnLoad

### 도구 (Unity Editor)
- `Tools > Codex Tactics > Create Battle Test Scene` — 전투씬 자동 생성
- `Tools > Codex Tactics > Validate Battle Test Scene` — 씬 검증 (220+ 체크)
- `Tools > Codex Tactics > Create Stage Select Scene` — 스테이지 선택씬 자동 생성

## 시스템 요구사항

- **Unity:** 6000.4.6f1 (또는 호환 버전)
- **플랫폼:** Windows/Mac
- **저장공간:** ~100MB

## 빠른 시작

1. Unity Hub에서 프로젝트 폴더 추가/열기
2. `Tools > Codex Tactics > Create Battle Test Scene` 클릭
3. `Tools > Codex Tactics > Validate Battle Test Scene` 으로 검증
4. `Tools > Codex Tactics > Create Stage Select Scene` 클릭
5. **Build Settings**에 씬 추가: TitleScene, StageSelectScene, BattleScene
6. Play! (Title Scene부터 시작)

## 테스트

- **Edit Mode Tests:** `Window > General > Test Runner` > `EditMode` > `Run All`
- **Scene Validate:** `Tools > Codex Tactics > Validate Battle Test Scene`
- 230+ 자동 테스트 체크, ALL PASS

## 문서 구조

```
Docs/
├── Devlog/        — 날짜별 개발 기록 (38개 파일)
└── Study/         — 시스템별 설계 문서 (13개 파일)
```

## 기술 스택

- **Unity 6000.4.6f1** (URP, 2D Orthographic)
- **C#** — 순수 C#, no 외부 라이브러리
- **TextMeshPro** — UI 텍스트
- **Batchmode 테스트** — WSL/CI 호환
