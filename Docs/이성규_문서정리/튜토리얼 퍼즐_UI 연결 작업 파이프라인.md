# 튜토리얼 UI 연결 작업 파이프라인

---

## 1. Inspector 세팅

- BoardManager의 `_tutorialHighlightPrefab`에 하이라이트 Image 프리팹 할당
- TutorialBoardPreset SO 생성 → 기획서 p.5 기준 블록 배치 세팅 (60칸 전부)
- 각 BlockDataSO에 `_matchEffectFrames` 스프라이트 배열 할당

---

## 2. 튜토리얼 컨트롤러에서 BoardManager 참조

```csharp
ITutorialBoardControl board = boardManager as ITutorialBoardControl;
// GetComponent<ITutorialBoardControl>()는 사용 불가 — MonoBehaviour 참조로 캐스팅
```

---

## 3. 각 단계별 호출 순서

### 튜토리얼 시작 (보드 생성 직후)

```csharp
board.LoadPresetBoard(preset.ToGrid());  // 프리셋 보드 즉시 로드 (랜덤 보드 노출 방지)
board.SetInputLocked(true);              // 전체 입력 차단
```

### 1단계: 첫 대사 (프롤로그)

```csharp
// 입력은 이미 잠금 상태
// → 프롤로그 팝업 표시
```

### 2단계: 퍼즐 UI 안내 (팝업 닫힌 후)

```csharp
board.SetInputLocked(false);           // 입력 허용 (탭만 감지)
board.SetInteractionFilter(_ => false); // 드래그 전면 차단
board.OnBoardTapped += 핸들러;          // 퍼즐 영역 클릭 대기
// → 클릭 시 퍼즐 UI 팝업 표시
```

### 3단계: 기본 조작 안내 (팝업 닫힌 후)

```csharp
board.SetBlockHighlights(preset.GetHighlightPositions(), Color.yellow);
// → 기본 조작 팝업 표시
// → 닫힌 후에도 하이라이트 유지
```

### 4단계: 행동력 시스템 안내 (팝업 닫힌 후)

```csharp
// 잡기 제한: 드래그 소스만 잡을 수 있음
board.SetInteractionFilter(pos => pos.Equals(preset.DragSource));

// 스왑 방향 제한: 소스 → 타겟 방향으로만 놓을 수 있음
board.SetSwapFilter((from, to) =>
    from.Equals(preset.DragSource) && to.Equals(preset.DragTarget));

// 매칭 파이프라인 가로채기
board.SetChainInterceptor((matches, proceed) =>
{
    savedProceed = proceed;              // proceed를 보관
    // → 행동력 시스템 팝업 표시
    // → 팝업 닫힐 때 savedProceed 호출
});
```

### 5단계: 콤보 시스템 안내 (팝업 닫힌 후)

```csharp
board.SetChainInterceptor(null);        // 인터셉터 해제
board.ClearAllHighlights();             // 하이라이트 종료
savedProceed?.Invoke();                 // 매칭 파이프라인 재개
// → 콤보 시스템 팝업 표시
```

### 6단계: 전투 UI 안내 (팝업 닫힌 후)

```csharp
// → 전투 UI 팝업 표시
```

### 튜토리얼 종료 (팝업 닫힌 후)

```csharp
board.SetInteractionFilter(null);       // 잡기 필터 해제
board.SetSwapFilter(null);              // 스왑 방향 필터 해제
board.SetInputLocked(false);            // 입력 해제
board.SetChainInterceptor(null);        // 인터셉터 해제
board.ClearAllHighlights();             // 하이라이트 해제
// → 자율 플레이 시작
```

---

## 4. 스킵 처리

어느 단계에서든 스킵 시 모든 제한을 해제.

```csharp
board.SetInteractionFilter(null);
board.SetSwapFilter(null);
board.SetInputLocked(false);
board.SetChainInterceptor(null);
board.ClearAllHighlights();
savedProceed?.Invoke();                 // 일시정지된 파이프라인이 있으면 재개
```