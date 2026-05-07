# GitHub 연결 가이드

현재 로컬 Git 저장소와 첫 커밋은 완료되었다.

로컬 위치:

```text
C:\Users\jywls\Desktop\game_portfolio
```

WSL 경로:

```text
/mnt/c/Users/jywls/Desktop/game_portfolio
```

## 자동 GitHub 생성 실패 이유

Hermes 환경의 GitHub 토큰은 로그인 확인은 가능하지만, 새 저장소 생성 권한이 부족해서 다음 오류가 발생했다.

```text
403 Resource not accessible by personal access token
```

## 직접 연결 방법

1. GitHub에서 `codex-tactics` 저장소를 새로 만든다.
2. 저장소는 포트폴리오용이므로 Public 추천.
3. README 자동 생성은 체크하지 않는다.
4. 생성 후 터미널에서 아래 명령 실행:

```bash
cd /mnt/c/Users/jywls/Desktop/game_portfolio
git remote add origin https://github.com/wlsdid/codex-tactics.git
git push -u origin main
```

이미 origin이 있다면:

```bash
git remote set-url origin https://github.com/wlsdid/codex-tactics.git
git push -u origin main
```

## Hermes가 다시 자동으로 하게 하려면

GitHub 토큰에 repo 생성 권한을 추가하거나, GitHub CLI 로그인을 사용할 수 있게 설정해야 한다.
