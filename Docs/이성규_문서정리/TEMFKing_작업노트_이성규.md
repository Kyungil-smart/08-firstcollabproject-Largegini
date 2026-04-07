# TEMFKing_작업 노트

**작성자**: 이성규  
**게임명**: 스러진 왕의 영원한 행진(The Eternal March of the Fallen King)  
**작성일**: 2026-03-23  
**최종 수정**: 2026-04-07  

## 프로젝트 개요

- **진행 기간**: 2026.03.20(금)~2026.04.09(목)
- **개발 환경**: Unity / C# / URP 2D
- **유니티 버전**: 6.3 LTS

# 작업 일지

## Day 1 — 2026-03-20

기획팀과 회의 및 플밍팀 회의 시간

## Day 2 — 2026-03-23

프로젝트 기초 세팅을 공유 받음

작업 브랜치를 만든 후 기초 문서들 생성

R&D 시작

### 테스트용 UI 및 프리팹 제작

**테스트용 더미 UI 그리드**
![alt text](Resources/Grid_Dummy.png)
유니티상에서 개발에 사용할 테스트 더미 UI 작성 (5*6 그리드)  
빠른 구성을 위해 GridLayoutGroup 사용 후 배치 확정되면 비활성화.  
런타임에서 레이아웃 재계산 비용을 없애기 위함.

**블록 프리팹 생성**

블록을 그리드 슬롯 위에 배치하는 구조.

- 그리드 슬롯: Raycast Target Off — 블록에 가려져서 입력을 받을 일이 없음
- 블록: Raycast Target On — 드래그 입력을 받아야 하므로 필수

블록이 자기 그리드 좌표(x, y)를 들고 있어서,
드래그 시작 시 출발 좌표를 알 수 있고
드롭 시 인접 슬롯 위에 놓였는지로 스왑 여부를 결정한다.

드래그는 인접 1칸으로 제한하며,
가로/세로 중 큰 축만 허용하여 대각선 이동을 방지한다.

2D UI라 Physics Raycast나 Trigger는 적합하지 않음.
RectTransformUtility.RectangleContainsScreenPoint로
드롭 위치가 인접 그리드 슬롯(상하좌우 4칸) 안에 있는지 판정한다.
해당 슬롯 안이면 스왑, 아니면 원위치 복귀. 

### Grid2D 스크립트 작성

**기본 게임플레이**
기본 매치3 게임플레이는 2D 그리드를 타일(블록)로 채운 뒤, 플레이어가 타일을 교환하여 매치를 만드는 것이다. 매칭된 타일은 제거되고, 위의 타일들이 떨어져 빈 칸을 채우며, 필요에 따라 새 타일이 추가된다.

### 2D 그리드

게임 로직과 스킨 모두 2D 그리드를 다뤄야 하므로, 이를 쉽게 하기 위한 제네릭 직렬화 가능 `Grid2D<T>` 구조체를 만든다. 내부 셀 배열로 데이터를 저장하고 2D 크기를 `int2`로 추적하며, 생성자 메서드에 전달한다.

**다차원 배열이 아닌 이유**
Unity 직렬화가 다차원 배열을 지원하지 않아서다.
1차원 배열에 y * width + x로 인덱싱하면 직렬화도 되고, 캐시 친화적이라 순회 성능도 더 좋다. 제네릭 구조체로 감싸서 인덱서를 제공하면 외부에서는 2차원처럼 쓰면서 내부는 1차원이다.

- `IsUndefined` — 셀이 채워지지 않은 상태에서 그리드 접근을 방지
- `this[int x, int y]` / `this[int2 c]` — 2차원 좌표로 셀에 접근하는 인덱서
- `AreValidCoordinates(int2 c)` — 좌표가 그리드 범위 내인지 확인
- `Swap(int2 a, int2 b)` — 두 좌표의 데이터를 교체 (블록 스왑에 활용)


### 블록 재사용 방식 결정 (풀링 대체)

- 30개의 블록은 풀링하기엔 오버 엔지니어링이므로 쓰지않는다. 다만 매칭 될때마다 파괴하는건 메모리 효율에 좋지 않으므로 매칭된 블록을 즉시 파괴하는 대신, 투명하게(Alpha 0) 만들거나 화면 밖 저 멀리 보낸다.
- 위에서 새로운 블록이 내려와야 할 때, 방금 숨겨둔 블록을 그리드 맨 위쪽 좌표로 순간 이동시킨 뒤 데이터(색상/타입)만 바꾸어 다시 아래로 떨어뜨린다.
- 타일(블록)이 떨어질때는 위에서 아래 방식이지만 그리드 밖에서는 안보이게 한다.
  - 마스킹 기법 사용(RectMask2D)
  - RectMask2D 컴포넌트를 퍼즐 보드 캔버스 최상단에 추가한다

### 데이터 및 상태 열거형(Enum) 작성
- **BlockType**: 블록의 타입을 담을 열거형 스크립트 작성 (None, Attack, Defense, Heal, Special)
- **EBlockStatus**: 블록의 상태를 담을 열거형 스크립트 작성 (None, Freeze 등)

### BlockDataSO 스크립트 작성 (ScriptableObject)
- 타입별 데이터를 관리하는 데이터 컨테이너 구현.
- EBlockType type: 블록의 타입.
- Color color: 임시 컬러 세팅 (나중에 스프라이트로 교체 예정).
- Sprite sprite: 나중에 아트 리소스가 들어오면 교체할 용도.
- float effectValue: 블록당 효과 수치 (공격 5, 방어 2 등 매칭 시 전투 시스템에 전달).

### Block 스크립트 작성 (MonoBehaviour)
- 그리드에 채워질 기본 단위인 블록 오브젝트에 붙는 런타임 스크립트.
- 인스턴스별 런타임 데이터 관리: BlockDataSO data(SO 참조), EBlockStatus status(상태), int2 gridPosition(그리드 좌표).
- Image 컴포넌트를 참조하여 SetBlock 등에서 타입 세팅 및 비주얼을 반영함.
- 처음 생성될 때, 또는 화면 밖에서 재배치되어 내려올 때 블록 상태를 초기화하는 기능 추가.
- 위치 설정(SetPosition) 및 상태 설정(SetStatus) 메서드 추가.
- 매칭되어 터질 때를 위해 파괴 대신 비활성화하는 Despawn 스크립트 추가.

### SGrid2D 스크립트 개선
- 보드 전체를 비우는 초기화 메서드(`Clear()`) 추가.
- `yield return`과 튜플을 사용해 모든 셀을 순회하는 반복자(`GetAllCells()`)를 추가하여 편의성을 높임.

## Day 3 — 2026-03-24

아침 회의 및 에셋 세팅 진행

### BoardManager 스크립트 작성

그리드와 블록까지 기본 단위 스크립트를 작성했으므로 이제 게임 보드를 채우고 관리하기 위한 스크립트를 작성한다.

우선 블록을 담을 그리드 변수 필요.
보드관리자 스크립트니 보드의 정보를 담을 변수 선언.
행과 열의 갯수를 설정한다.

- 스폰할 블록 프리팹
- 블록들이 생성될 부모 캔버스 패널
- 스폰 시 랜덤으로 부여할 SO 데이터 풀
- 스폰 위치(그리드) 계산용 시작 좌표 및 셀 간격 세팅
- 초기 생성시 보드를 초기화 호출한다.
- 블록으로 그리드 구조 데이터를 생성하고
- 기존 수동 할당 방식에서 동적 계산 방식으로 전환하여, `BoardLayout` 클래스를 통해 산출된 UI 좌표에 맞춰 블록 객체를 생성 및 배치한다.
- 생성된 블록에는 SO 데이터를 통해 랜덤하게 데이터를 초기화하며 랜덤 스폰을 완료한다.
- 캔버스 기반 UI 객체이므로 위치 할당 시 `RectTransform.anchoredPosition`을 사용하며, 슬롯 앵커는 Center로 통일하여 좌표가 어긋남을 방지함.
- `ResetBoard` 로직 추가: 불필요한 파괴 및 재생성을 하지 않고 기존 오브젝트를 유지하되 데이터만 교체하며 비주얼 및 데이터를 갱신한다.

![alt text](Resources/RandomSpawn.png)
랜덤 스폰이 완료된 모습, 아직 초기 매칭 방지 로직은 없고 보드 초기화 기능 정상 동작 확인.

### 퍼즐 로직 보강 논의

---

**보드 구조**

전체 그리드를 12행 x 5열(보이는 6행 + 버퍼 6행)로 통합 관리한다. `SGrid2D<Block>`은 하나만 쓰고, y 0~5가 버퍼 영역, y 6~11이 플레이 영역이다. 그리드 슬롯 오브젝트는 60개(보드 30 + 버퍼 30)를 배치하되, 보드 패널에 `RectMask2D`를 걸어서 버퍼 행은 시각적으로 가린다.

**블록 라이프사이클**

블록 GameObject는 초기 생성 후 파괴하지 않는다. 매치 소멸 시 연출(알파 off 등) 후 비활성 상태로 전환하고, 버퍼 최상단 슬롯으로 논리적 재배치한 뒤 새 SO 데이터로 `Init()`하여 재사용한다.

**낙하 로직**

매치 제거 후 각 열별로 빈 칸을 아래부터 탐색한다. 빈 칸 위에 있는 블록들을 순차적으로 아래로 당기고, 버퍼 행의 블록들이 보드 영역으로 내려온다. 이동은 DoTween `DOAnchorPos`로 목표 슬롯 좌표까지 연출하며, 열별로 약간의 delay를 주어 캐스케이드 느낌을 준다.

**블록 상태 관리**

블록에 상태 구분을 둔다(활성 / 연출중 / 비활성 등). 연출중·비활성 블록은 입력 및 매칭 판정에서 제외한다. 낙하 완료 후 전체 블록이 활성 상태가 되면 보드 전체 매칭 스캔을 수행한다.

**시드 기반 초기화**

`UnityEngine.Random` 대신 `System.Random` 인스턴스를 사용한다. 스테이지 SO에 시드값 필드를 두고, CSV 파이프라인으로 세팅한다. 시드가 있으면 해당 시드로, 없으면 순수 랜덤으로 보드를 생성한다.

**매칭 판정 범위**

매칭 체크는 보이는 보드 영역(y 6~11)에서만 수행한다. 버퍼 행 블록은 내려와서 보드에 안착한 시점부터 판정 대상이 된다.

![alt text](Resources/BufferBlocks.png)
버퍼 영역까지 위치 보정해서 블록 생성 완료된 모습  
마스킹은 일부러 끈 상태

---

### 블록 드래그 시스템 개발

퍼즐에 한해서만 동작할 블록 드래그처리를 담당할 기능을 개발한다.

inputAction을 활용하기 보다는 UI에서 지원하는 Handler 이벤트를 활용해 구현한다.

IPointerDownHandler + IDragHandler + IPointerUpHandler

UI에 클릭했을때 Down, 그 상태에서 드래그할때 Drag, 클릭을 멈추고 땔때 Up

**Block**
- 드래그 시작점을 저장
- 스와이프 방향 판정
  - 델타값에서 상하좌우 중 가장 큰 축으로 (OnPointerUp 또는 OnDrag에서 임계값 넘었을 때)
- 판정된 방향 + 자신의 그리드 좌표를 BoardManager에 전달

**BoardManager**
- 스왑 요청 수신 (어떤 블록이 어느 방향으로)
- 인접 좌표 유효성 검사 (보드 범위 내인지, 버퍼 행 아닌지)
- 두 블록의 그리드 데이터 + 좌표 교환

- Block.cs 에서 Board 매니저를 연결해서 의존성 주입(DI)
  - BlockDragHandler에서 보드를 안찾아도된다.
  - BoardManager에서 블록을 생성할때 자신을 연결해준다.

- `IBoardInteractable` 인터페이스 도입으로 Block/DragHandler가 BoardManager 구체 타입에 의존하지 않도록 분리
  - `CanInteract(int2 pos)`: 버퍼 영역 및 블록 상태 기반 입력 차단
  - `OnSwipeBlock(int2 pos, Vector2Int dir)`: 스왑 요청 수신

- `_isDragging` 플래그로 `CanInteract` 실패한 터치가 `OnPointerUp`까지 흘러가는 엣지 케이스 차단

- 임계값(DRAG_THRESHOLD = 30px) 미만의 미세 터치는 무시하여 의도치 않은 스와이프 방지

### 블록 스왑 구현

스와이프 방향으로 인접 블록과 교체하는 스왑 로직을 구현했다.

- 스왑 시 세 가지를 교환: 그리드 데이터 참조, 논리 좌표(SetPosition), UI 좌표(anchoredPosition)
- UI Y축 반전 보정: UI에서 위로 드래그하면 direction.y = 1이지만 그리드에서는 y 감소 방향이므로 `pos.y - direction.y`로 보정
- `IsValidPlayArea`로 타겟 좌표가 보이는 영역 내인지 검증
- 매칭 없을 시 되돌리기는 같은 SwapBlocks를 한 번 더 호출하면 됨

### Block RectTransform 캐싱

스왑마다 `GetComponent<RectTransform>()` 호출하는 성능 낭비를 방지하기 위해, `Block.Init()` 시점에 `Rect` 프로퍼티로 캐싱. 이후 모든 좌표 접근은 `block.Rect.anchoredPosition`으로 통일.

### 보드 시스템 역할 분리

BoardManager가 비대해지는 것을 방지하기 위해 역할별로 클래스를 분리했다.

- **BoardManager**: 흐름 제어 컨트롤러. Inspector 설정 보유, 하위 시스템 생성 및 조율, 입력 검증(CanInteract, IsValidPlayArea)
- **BoardSpawner**: 블록 생성(SpawnBlock/SpawnAll) 및 데이터 교체(ResetAll) 담당
- **BoardSwapper**: 블록 스왑 실행 담당. 그리드 데이터 교환, 논리 좌표 교환, UI 좌표 교환

모두 순수 C# 클래스로 구현하여 MonoBehaviour 의존 없이 동작하며, BoardManager가 Awake에서 `SGrid2D<Block>`과 `BoardLayout`을 주입하여 생성한다.

`SGrid2D`가 struct이지만 내부 `T[] cells`가 참조 타입이므로, 복사되어도 같은 배열을 가리켜 하위 시스템 간 데이터 동기화에 문제없음. 단, `readonly` 키워드를 붙이면 struct 멤버 수정이 불가하므로 제거하여 사용.

## Day 4 - 2026-03-25

아침 회의 및 WBS 등 문서 작업 사전 진행

### 스왑 DoTween 연출

즉시 좌표 교환 대신 `DOAnchorPos`로 슬라이딩 이동 연출을 적용했다.

블록 상태(EBlockStatus)에 연출 중 상태(Moving)을 추가해서 연출 중 블록 조작 방지

BoardSwapper에 DoTween 연출을 넣는다.  
스왑 시 즉시 좌표 교환 대신 DOAnchorPos로 슬라이딩하고, 연출 시작 시 블록 상태를 연출중으로 바꾸고 완료 시 복귀하는 흐름

- 스왑 시작 시 양쪽 블록을 `EBlockStatus.Moving`으로 전환
- `DOAnchorPos`로 목표 좌표까지 0.2초 슬라이딩 (`Ease.OutQuad`)
- 완료 콜백에서 `EBlockStatus.None`으로 복귀
- `CanInteract`에서 `Status != None`이면 차단하므로 연출 중 입력 차단은 자동으로 처리됨

### 드래그 앤 드랍 방식 스왑 구현

스와이프 방식 DoTween 연출 테스트를 완료 했으므로 드래그 앤 드랍 방식의 스왑을 구현한다.

1. OnPointerDown: 블록 들어올림 (시각적 피드백 + 원래 위치 저장)
2. OnDrag: 블록이 손가락 따라 이동
3. OnPointerUp: 놓은 위치 → 그리드 인덱스 역변환 → 인접 칸이면 스왑, 아니면 원위치 복귀

UI 좌표 → 그리드 인덱스 인덱스 역변환 스크립트를 `BoardLayout`에 작성

**드래그 시 동작 방식**
유효한 칸에 놓음: 즉시 스냅 + 상대 블록만 DoTween으로 드래그한 블록의 자리로 슬라이딩
잘못된 칸에 놓음: 드래그한 블록만 DoTween으로 원위치 복귀

**필요 요소**
드래그 시작 전 원래 UI 좌표
터치 지점과 블록 중심의 오프셋

**드래그 범위 제한 (클램핑)**
- 1차: 원래 위치에서 stride + cellSize * 0.3f 범위 내로 제한. 셀 외곽 허용치를 0.5가 아닌 0.3으로 제한한 이유는 `BoardLayout.GetGridIndex`의 `RoundToInt` 반올림 시 0.5면 인접 1칸이 아닌 2칸 인덱스가 반환될 수 있기 때문.
- 2차: 보드 외곽선(셀 중심 ± cellSize * 0.5f) 범위로 추가 클램핑. 이중 제한으로 보드 밖으로 절대 나가지 않도록 보장.

**놓은 위치 판정 방식**
`OnPointerUp`에서 마우스 원본 좌표 대신 클램핑된 블록의 현재 `anchoredPosition`으로 그리드 인덱스를 계산. 드래그 중 보이는 위치와 판정 위치가 일치하도록 보장.

**스왑 참조 타이밍 주의점**
`SwapFromDrag`에서 `_blocks.Swap` 호출 후 참조를 잡아야 논리 좌표와 그리드 위치가 일치함. Swap 전에 참조를 잡으면 `SetPosition`과 UI 좌표 방향이 뒤바뀌어 두 번째 스왑부터 데이터가 꼬이는 버그 발생. `Swap` 메서드와 동일한 패턴(Swap 후 참조)으로 통일.

**IBoardInteractable 확장**
- `GetGridIndex(Vector2)`: UI 좌표 → 그리드 인덱스 역변환
- `IsValidSwapTarget(int2, int2)`: 맨해튼 거리 기반 인접 1칸 검증
- `OnDragSwapBlock(int2, int2)`: 드래그 스왑 요청
- `GetStride()`, `GetCellSize()`: 드래그 클램핑용
- `GetBoardMin()`, `GetBoardMax()`: 보드 외곽 클램핑용

### 보드 프로세싱 잠금

스왑 연출 중 다른 블록 조작을 방지하기 위해 `_isProcessing` 플래그를 BoardManager에 도입했다.

- BoardManager가 `_isProcessing` 플래그를 소유하고, `CanInteract`에서 체크하여 전체 입력 차단
- BoardSwapper는 `System.Action` 콜백(`onSwapStart`, `onSwapEnd`)으로 BoardManager에 연출 시작/완료를 알림
- Swapper가 플래그를 직접 모르고 콜백으로만 통신하여 책임 분리 유지
- 양쪽 블록 DoTween 완료를 카운터(`completed`)로 추적하여, 두 블록 모두 완료된 시점에 `onSwapEnd` 호출
- 매 스왑 시작 시 카운터를 0으로 리셋하여 누적 버그 방지
- 매칭/낙하/연쇄 처리에서도 동일한 `_isProcessing` 플래그로 확장 예정

### 인터페이스 역할 분리 리팩터링

`IBoardInteractable`이 비대해져서 역할별로 분리했다.

- **IBoard**: 복수 인터페이스의 통합 접근 창구
- **IBoardInteraction**: 블록 입력 검증 및 스왑 요청
- **IBoardQuery**: 보드 레이아웃 정보 조회
- **IBoardData**: 그리드 데이터 접근

BoardSwapper는 `IBoardData`만, BlockDragHandler는 `IBoardQuery` + `IBoardInteraction`을, Block/Spawner는 `IBoard`를 참조. 각 클래스가 필요한 만큼만 보드에 접근하도록 의존성 범위를 제한했다.

하위 시스템이 `SGrid2D<Block>`을 직접 참조하던 구조에서 `IBoardData` 인터페이스를 통한 간접 접근으로 전환하여, 그리드 자료구조가 변경되어도 하위 시스템은 수정 불필요.

### 매칭 시스템 뼈대 구축

매칭 판정 구현을 위한 기초 구조를 작성했다.

**Match 구조체**
- 매치 정보의 기본 단위: 시작 좌표, 길이, 방향(가로/세로), 블록 타입
- 콤보 계산 및 전투 아웃풋에서 타입별 집계에 활용

**MatchFinder 클래스**
- IBoardData를 통해 그리드 데이터에 접근
- 플레이 영역(bufferRows ~ rows)에서만 매치 탐색
- FindAllMatches: List<Match> 반환 (로직은 내일 구현)
- BoardManager.Awake에서 생성 및 연결 완료

**SGrid2D 범위 순회 추가**
- `GetCellsInRange(startY, endY)`: 지정 영역만 순회하는 반복자 추가
- MatchFinder에서 플레이 영역만 탐색할 때 활용


## Day 5 - 2026-03-26

### FindAllMatches 로직 구현

가로/세로 3매치 탐색

- 플레이 영역 전체에서 3개 이상 연속된 동일 타입 블록을 탐색
- 가로/세로 독립 탐색으로 L자, T자 매치도 자연스럽게 잡힘
- 기획서 기준 매치 형태 무관 일괄 소멸이므로 형태별 분기 불필요, "동일 타입 3개 이상 연속"만 판정

GetCellsInRange는 가로/세로를 각각 다른 축으로 순회해야 해서 통합 순회보다 개별 for 루프를 돌려야해 안쓰게 됨.

- 가로 매치 탐색 — 각 행을 왼쪽부터 스캔하며 연속 동일 타입 카운트
- 세로 매치 탐색 — 각 열을 위에서부터 스캔하며 연속 동일 타입 카운트
- 연속 끝까지 건너뛰어 중복 탐색 방지, 4매치 5매치도 count로 자연스럽게 처리
- 탐색 범위는 플레이 영역(bufferRows ~ rows-1)만, 버퍼 행 블록은 기획서 규칙대로 판정 제외
- 결과를 SMatch 구조체(시작 좌표, 길이, 방향, 블록 타입) 리스트로 반환, 타입별 콤보 집계에 활용

### 스왑 완료 → 매칭 체크 연결

- 스왑 연출 완료 콜백(`_onSwapEnd`)에서 `OnSwapComplete` 호출
- 매치 발견 시 `_isProcessing` 유지한 채 연쇄 처리 진행, 모든 연쇄 완료 후 해제
- 매치 없으면 되돌리기 스왑 후 `_isProcessing` 해제
- `_isProcessing = false`를 콜백에서 직접 하지 않고 `OnSwapComplete` 안에서만 제어 — 매치 있을 때 잠깐 false 되는 틈에 입력이 들어오는 버그 방지

### BoardSwapper 리팩터링

- Swap/SwapFromDrag 중복 로직을 ExecuteSwap + AnimateBlock으로 통합
- 드래그 스왑도 양쪽 DoTween 슬라이딩으로 통일

### 연쇄 루프 구현

스왑 완료 → 매칭 체크 → 제거 → 낙하 완료 → 매칭 체크 → 제거 → 낙하 완료 → 매칭 체크 → 없으면 끝

#### 클리어

- SMatch 리스트에서 실제 블록 좌표를 추출하여 제거
- 가로/세로 매치의 시작 좌표 + 길이 + 방향으로 개별 좌표를 펼침
- HashSet으로 중복 제거 — L자, T자 매치에서 교차점 블록이 가로/세로 양쪽에 잡히므로

### 낙하(DropBlocks)

매치 제거 후 빈 칸을 채우기 위해 블록을 아래로 당김

**낙하 로직 흐름**
1. 열별로 빈 칸 채우기
   - 각 열을 맨 아래(rows-1)부터 위로 스캔. 빈 칸 발견하면 그 위에서 활성 블록 찾아서 당겨내림. 버퍼 행 블록도 포함해서 전체 열을 처리.
2. 그리드 데이터 + 논리 좌표 갱신
   - 블록이 새 위치로 이동하면 _data.SetBlock + SetPosition으로 데이터 동기화.
3. DoTween 낙하 연출
   - 원래 위치에서 새 위치로 DOAnchorPos. 낙하 거리에 비례해서 duration 설정하면 자연스럽고, 열별로 약간의 딜레이 주면 캐스케이드 느낌.
4. 완료 감지
   - 모든 낙하 DoTween 완료 시 콜백.

**그리드 데이터 동기화**
- `SetBlock(newPos, block)` + `SetBlock(oldPos, null)`로 그리드 데이터 갱신
- `block.SetPosition(newPos)`로 논리 좌표 갱신
- UI 좌표는 DoTween이 처리

**낙하 연출**
- 낙하 거리에 비례한 duration으로 자연스러운 속도 차이
- `Ease.InQuad`로 가속 낙하 느낌
- 모든 블록 낙하 완료를 카운터로 추적, 전부 완료 시 콜백 호출
- 낙하할 블록이 없으면 즉시 완료 콜백

### 애니메이션 설정 인스펙터 노출

하드코딩된 연출 수치를 `BoardAnimationSettings` 직렬화 클래스로 분리하여 Inspector에서 실시간 튜닝 가능하게 했다.

- `[System.Serializable]` 클래스로 BoardManager 안에 배치
- Swap: duration, Ease 타입
- Drop: 셀당 duration, Ease 타입
- Match: 제거 후 대기시간, 리필 후 대기시간
- BoardSwapper, BoardProcessor 생성 시 주입하여 내부 const 교체
- 코드 수정 없이 인스펙터에서 수치/Ease 조절 가능

### 리필 구현

낙하 처리 후 빈 칸(null)에 비활성 블록을 재활용하여 채우는 로직.

**재활용 풀 방식**
- DropBlocks에서 빈 칸을 null로 처리하므로 Despawn된 블록 참조가 그리드에서 사라짐
- ClearMatches에서 Despawn된 블록을 별도 리스트(`_recyclePool`)에 보관하여 참조 유지
- RefillBuffer에서 빈 칸(null)을 위에서부터 순회하며 풀에서 순서대로 꺼내 재활용
- 루프 종료 후 풀 클리어

**리필 책임 분리**
- 블록 데이터 교체 및 배치는 BoardSpawner의 `RefillBlock(pos, recycled)` 담당
- BoardProcessor는 빈 칸 탐색 + Spawner 호출만 수행
- Init이 데이터 + 상태 + 비주얼 + SetActive(true) 전부 복원하므로 별도 활성화 처리 불필요

**리필 로직 정리**
1. 매치된 블록 Despawn → 재활용 풀에 등록
2. DropBlocks에서 활성 블록들 아래로 당김 → 원래 위치는 null
3. RefillBuffer에서 위에서부터 null인 칸 탐색 → 풀에서 블록 꺼내 Init으로 새 데이터 부여 + 해당 좌표에 배치

### 퍼즐 결과 아웃풋

연쇄 루프 완료 후 전투 시스템에 결과를 전달하는 구조.

**PuzzleResult 데이터 클래스**
- 타입별 매치된 블록 수 (Dictionary<EBlockType, int>)
- 콤보 카운트 (연쇄 횟수)
- AddMatches로 매 루프마다 SMatch 리스트에서 타입별 블록 수 누적

**전달 방식**
- BoardManager에 UnityEvent<PuzzleResult> 인스펙터 노출
- 연쇄 루프 완료 시 OnPuzzleComplete 콜백에서 _isProcessing 해제 + 이벤트 발사
- 전투 팀원은 인스펙터에서 자기 스크립트 메서드 드래그 연결하거나 코드로 AddListener
- 콤보 계산식(결과 = 기본값 * (1 + (N-1) * 배율))은 전투 쪽 책임, 퍼즐에서는 원재료만 전달

### 초기 보드 3매치 방지

초기 스폰 및 리셋 시 플레이 영역에서 3매치가 성립되지 않도록 블록 타입을 선택하는 로직.

**방지 원리**
- 왼쪽에서 오른쪽, 위에서 아래 순서로 배치하므로 현재 칸 기준 왼쪽/위쪽 블록은 이미 확정됨
- WouldCauseMatch: 왼쪽 2칸 + 위쪽 2칸이 같은 타입이면 매치 성립으로 판정
- GetNonMatchingData: 매치 유발 타입을 제외한 후보 리스트에서 랜덤 선택, 시도 횟수 반복 방식 대신 후보 필터링으로 무한 루프 가능성 제거
- 블록 타입 4개 중 가로/세로 최대 2개 제외돼도 후보가 최소 2개 남음

**적용 범위**
- SpawnAll(초기 생성)과 ResetAll(리셋) 시에만 적용
- 버퍼 영역(y < bufferRows)은 기획서대로 매치 허용, 플레이 영역만 방지
- SpawnBlock(단일 생성)과 RefillBlock(리필)은 순수 랜덤 유지

**리팩터링**
- SpawnAll/ResetAll 공통 로직을 InitBlockSafe로 추출
- 생성자에서 받은 필드(_rows, _columns, _bufferRows) 활용, 메서드 파라미터 제거

### 데드락 판정

플레이 영역에서 어떤 인접 스왑도 매치를 만들 수 없는 상태를 판정하는 로직.

**판정 방식**
- 플레이 영역 전체 순회, 각 블록에서 오른쪽/아래쪽 인접 스왑 시뮬레이션
- 임시 스왑 → MatchFinder.FindAllMatches로 매치 여부 체크 → 즉시 되돌림
- 오른쪽/아래쪽만 체크하면 모든 인접 쌍이 커버됨 (왼쪽/위쪽은 이전 칸에서 이미 체크)
- 매치 가능한 스왑이 하나라도 있으면 즉시 false 반환 (early exit)

**처리 흐름**
- 스왑 완료 후 매치 없을 때 데드락 체크 수행
- 데드락 발견 시 UnityEvent(_onDeadlock) 발사
- UI 팀은 _onDeadlock에 팝업 연결

**BoardValidator 분리**
- 데드락 판정 로직을 BoardValidator 순수 C# 클래스로 분리
- BoardManager는 Validator 호출 + 이벤트 발사만 담당

**테스트**
- CreateDeadlockBoard: 체커보드 패턴으로 강제 데드락 보드 생성
- 테스트 버튼에 할당하여 데드락 감지 및 리셋 동작 검증

### 셀 하이라이트

드래그 중 블록이 위치한 셀에 이동 가능 여부를 시각적으로 표시하는 기능.

**구현 방식**
- Block 프리팹 자식에 하이라이트 Image를 배치, 기본 비활성 상태
- 별도 셀 오브젝트 시스템 없이 블록 자체가 하이라이트를 들고 있는 구조
- Block.SetHighlight(bool, Color?)로 켜고 끄기 + 색상 변경

**드래그 중 동작**
- OnDrag에서 매 프레임 현재 위치의 그리드 인덱스 계산
- 자기 자신 위치면 하이라이트 끔
- 호버된 블록이 이전과 같으면 스킵 (불필요한 갱신 방지)
- 맨해튼 거리로 대각선 여부 판별: 인접 1칸이면 초록색, 대각선이면 빨간색
- OnPointerUp에서 하이라이트 전부 해제


스크립트 19개, 인터페이스 4개, 구조체 2개.  
보드 생성 → 드래그 스왑 → 매칭 → 제거 → 낙하 → 리필 → 연쇄 → 아웃풋 → 3매치 방지 → 데드락 판정 → 셀 하이라이트


## Day 6 — 2026-03-27

팀원 작업물과 퍼즐 연결 서포트

프로토타입 동작 확인 및 영상 녹화

## Day 7 — 2026-03-30

이미지 연결 작업 시작

### BoardSpawner 리팩토링
 
`BoardSpawner`의 중복 코드 정리.
 
`SpawnBlock`과 `SpawnAll` 안에 블록 생성 로직(Instantiate → Init -> 좌표 -> sizeDelta -> DragHandler -> 그리드 등록)이 중복되어 있었음.  
공통 로직을 `CreateBlock(int2, BlockDataSO)`로 추출.
 
- `CreateBlock`: 블록 GameObject 생성 + 초기 세팅 공통 (void, 반환값 불필요)
- `SpawnBlock`: 랜덤 데이터로 `CreateBlock` 호출 (단일 블록 추가 시)
- `SpawnAll`: 3매치 방지 데이터 선택 후 `CreateBlock` 호출 (초기 생성)
- `InitBlockSafe`: `ResetAll` 전용으로 유지 (Instantiate 없이 데이터만 교체)

### 퍼즐 매칭 및 콤보 이펙트 추가

이펙트 에셋이 SpriteRenderer 기반이라 UI Image 블록에 그대로 사용 불가.  
애니메이션 클립 확인 결과 샘플레이트 15.  
샘플레이트 간격으로 Image.sprite를 순차 교체하는 프레임 재생 스크립트(UIFrameEffect) 작성.  
블록 프리팹 최하단 자식에 이펙트 재생용 Image 배치 (UI 렌더링 순서상 최상단 표시).  
이펙트 이미지 데이터는 블록별로 스프라이트 배열로 보관

**블록 자식 이펙트 방식**
- 블록이 꺼질 때 이펙트도 같이 꺼지는 문제 → Despawn 타이밍을 이펙트 완료 후로 지연
- 블록 이미지만 먼저 숨기고(`_blockImage.enabled = false`), 이펙트 재생 완료 콜백에서 비활성화
- 이펙트 재생 중 `EBlockStatus.Destroying` 상태로 전환하여 매칭/낙하 로직에서 스킵
- BlockDataSO에 매치 이펙트 프레임 배열(MatchEffectFrames) 추가, 타입별 다른 이펙트 가능
- 이펙트 Image 오브젝트만 개별 활성/비활성 제어 (`_effectImage.gameObject.SetActive`)

### 보드 상호작용 컨트롤 함수 추가
- SetInteractable
- BoardManger에 외부에서 블럭 상호작용 가능 여부 조절용 함수추가
- GraphicRaycaster를 끄고 켜는 간단한 방식

## Day 8 — 2026-03-31

에셋 관리 최신화 및 팀원 작업 서포트 오전 중 실행완료

### 튜토리얼 퍼즐 서포트 작업

**튜토리얼 흐름도**

**1단계: 첫 대사 (프롤로그)**
- 메인 로비 → 페이드 아웃 → 전투화면 로딩 → 프롤로그 팝업창 표시
- '계속' 버튼 클릭으로 닫기

**2단계: 퍼즐 UI 안내**
- 팝업 닫힌 후 퍼즐 UI 영역만 밝게, 나머지 어둡게
- 퍼즐 영역 1회 클릭 → 퍼즐 UI 팝업창 표시
- '계속' 클릭으로 닫기

**3단계: 기본 조작 안내**
- 기본 조작 팝업창 표시 → '계속' 클릭으로 닫기
- 닫힌 후에도 밝게 빛나는 퍼즐 UI 연출 유지
- 퍼즐 영역에서 특정 블록이 빛나는 연출 출력

**4단계: 행동력 시스템 안내**
- 유저가 빛나는 특정 블록을 1회 조작
- 행동력 1 소모
- 3매치 조건 만족되지만 상호작용 일시정지 (파괴 안 됨)
- 행동력 시스템 팝업창 표시

**5단계: 콤보 시스템 안내**
- '계속' 클릭 → 일시정지된 3매치 파괴 재개
- 빛나는 퍼즐 UI 연출 종료
- 콤보 시스템 팝업창 표시

**6단계: 전투 UI 안내**
- '계속' 클릭 → 전투 UI 팝업창 표시
- '계속' 클릭 → 튜토리얼 종료, 자율 플레이 시작

**기획서에서 퍼즐 보드에 요구하는 것**

| 단계 | 기획 요구사항 | 보드 쪽 대응 |
|------|-------------|------------|
| 2단계 | 퍼즐 영역 클릭 감지, 드래그 불가 | 입력 잠금 + 탭 이벤트 |
| 3단계 | 특정 블록 빛나는 연출 | 좌표 지정 하이라이트 |
| 4단계 | 특정 블록만 조작 가능 | 좌표 화이트리스트 필터 |
| 4단계 | 특정 방향으로만 스왑 가능 | 스왑 방향 필터 |
| 4단계 | 반드시 3매치 만족 | 프리셋 보드 배치 |
| 4단계 | 3매치 후 파괴 일시정지 | 매칭 파이프라인 인터셉터 |
| 5단계 | 팝업 닫히면 파괴 재개 | proceed 콜백 호출 |

### ITutorialBoardControl 인터페이스 작성
 
튜토리얼 시스템이 보드를 제어하는 전용 인터페이스.
기존 IBoard(= IBoardInteraction + IBoardQuery + IBoardData)는 퍼즐 내부 시스템용이고,
이 인터페이스는 퍼즐 외부(튜토리얼)가 사용하는 채널이므로 별도 분리.
 
| 튜토리얼 단계 | 호출 메서드/이벤트 | 스크립트 |
|-------------|----------------|---------|
| 2단계 클릭 감지 | `OnBoardTapped` 이벤트 | ITutorialBoardControl |
| 3단계 블록 빛남 | `SetBlockHighlights()` | BoardTutorialHandler |
| 4단계 특정 블록만 조작 | `SetInteractionFilter()` | BoardTutorialHandler |
| 4단계 스왑 방향 제한 | `SetSwapFilter()` | BoardTutorialHandler |
| 4단계 파괴 일시정지 | `SetChainInterceptor()` | BoardTutorialHandler |
| 5단계 파괴 재개 | `proceed()` 콜백 호출 | 튜토리얼 컨트롤러 측 |
| 전체 입력 잠금/해제 | `SetInputLocked()` | BoardTutorialHandler |
| 프리셋 보드 배치 | `LoadPresetBoard()` | BoardTutorialHandler |

---

### BoardTutorialHandler 분리
 
BoardManager가 400줄 이상으로 비대해져서 튜토리얼 제어 로직을 별도 하위 시스템으로 분리.
기존 Spawner/Swapper/Processor/Validator와 동일한 패턴의 순수 C# 클래스.
BoardManager는 ITutorialBoardControl 구현을 한 줄씩 위임하는 구조.

---

### 잡기 필터 / 스왑 필터 분리

4단계에서 (1,10) 블록을 잡아서 (2,10)으로만 드래그해야 하는 요구사항.
기존 `IsValidSwapTarget`이 타겟에도 `CanInteract`를 호출해서
`InteractionFilter`에 의해 스왑 타겟까지 거부되는 문제 발생.

- `CanInteract` + `InteractionFilter` → "잡을 수 있는 블록" 제한 (OnPointerDown)
- `IsValidSwapTarget` + `SwapFilter` → "스왑 방향" 제한 (OnPointerUp)
- `IsValidSwapTarget`에서 `CanInteract(to)` 제거, 타겟 블록 기본 검증만 직접 수행

---

### 튜토리얼 하이라이트 시스템 변경
 
기존 드래그 하이라이트는 블록 프리팹 하위 오브젝트로 드래그 시 같이 움직임.
튜토리얼 하이라이트를 같은 구조로 넣으면 드래그 시 같이 끌려가고, 해당 하이라이트가 호버링 상태가 아닐때 꺼주는 연출로 인해 튜토리얼 하이라이트까지 꺼지는 문제 발생.
 
**해결: 두 레이어 분리**
 
- 기존 `_highlight` -> 블록 프리팹 하위에 그대로 유지, 드래그 시 초록/빨강, `BlockDragHandler`가 관리
- 튜토리얼 하이라이트 -> 보드 패널 하위에 별도 Image 오브젝트 생성, 그리드 좌표에 고정, `BoardTutorialHandler`가 풀링 관리
 
블록이 드래그돼서 움직여도 튜토리얼 하이라이트는 보드 패널 자식이라 제자리에 남음.
두 시스템이 완전히 독립이라 서로 꺼버리는 문제 없음.
 
**하이라이트 프리팹 세팅**
 
- Image 컴포넌트 1개짜리 프리팹
- Raycast Target Off (터치 이벤트 차단 방지)
- sizeDelta는 생성 시 `_startRect.sizeDelta`(블록 크기)에 자동 맞춤
- BoardManager Inspector의 `_tutorialHighlightPrefab`에 할당

---

### BlockDragHandler 탭 감지 추가
 
튜토리얼 2단계에서 "퍼즐 영역 1회 클릭"을 감지하기 위해
드래그가 발생하지 않은 단순 탭을 보드에 알리는 로직 추가.

---

### BoardTestHelper 분리
 
에디터 전용 테스트 메서드를 BoardManager에서 별도 MonoBehaviour로 분리.
`#if UNITY_EDITOR` 전체 래핑으로 빌드에 포함되지 않음.
Inspector 우클릭 -> ContextMenu로 개별 기능 테스트.

---

### TutorialBoardPreset SO 작성
 
튜토리얼 스테이지용 사전 정의 보드 배치 데이터 ScriptableObject.
기획서 4단계에서 "유저가 특정 블록을 조작하면 반드시 3매치가 만족"되어야 하므로
보드의 초기 블록 배치를 고정하기 위한 데이터 컨테이너.
 
- `BlockDataSO[] _layout`: 1D 배열, `ToGrid()`로 2D 변환하여 `LoadPresetBoard()`에 전달
- `_dragSource` / `_dragDirection`: 4단계 조작 대상 블록 좌표
- `_highlightPositions`: 3단계 하이라이트 대상 좌표
- `OnValidate`에서 배열 크기 불일치 경고

---

### TutorialTestRunner 작성

UI 없이 튜토리얼 6단계 흐름을 에디터에서 순차 테스트하는 스크립트.
`#if UNITY_EDITOR` 전체 래핑. 튜토리얼 담당이 참조하거나 에디터 풀어서 런타임용으로 전환 가능.

- Start()에서 프리셋 로드 + 입력 잠금 (랜덤 보드 노출 방지)
- Space키로 단계 진행, R키로 리셋
- 4단계에서 InteractionFilter + SwapFilter + ChainInterceptor 동시 설정
- 콘솔 로그로 각 단계 동작 확인

UI 연결 작업 파이프라인 문서와 함께 튜토리얼 담당에게 전달.

---

### 기존 코드 영향 범위
 
| 파일 | 변경 종류 | 규모 |
|------|---------|------|
| ITutorialBoardControl.cs | 신규 | 인터페이스 (SwapFilter 포함) |
| BoardTutorialHandler.cs | 신규 | 순수 C# 클래스 (하이라이트 풀링 포함) |
| BoardTestHelper.cs | 신규 | 에디터 전용 MonoBehaviour |
| TutorialTestRunner.cs | 신규 | 에디터 전용 6단계 테스트 |
| TutorialBoardPreset.cs | 신규 | ScriptableObject |
| BoardManager.cs | 수정 | 튜토리얼 로직 위임, IsValidSwapTarget 검증 분리 |
| BlockDragHandler.cs | 수정 | 필드 1개, 탭 감지 추가 |
| BoardSpawner.cs | 수정 | CreateBlock 추출, sizeDelta 버그 수정 |
| BlockDataSO.cs | 수정 | matchEffectFrames 필드 추가 |
| Block.cs | 수정 | 이펙트 재생 Image 참조 추가 |
 
기존 퍼즐 로직(매칭, 낙하, 리필, 연쇄, 데드락)은 변경하지 않음.
추가된 필드가 null/false인 기본 상태에서는 기존과 완전히 동일하게 동작.

## Day 9 — 2026-04-01
 
튜토리얼 UI 퍼즐 연결 작업 서포트
 
### 콤보 UI 갱신 구현
 
`_onPuzzleComplete`는 연쇄 루프가 전부 끝난 후 최종 결과만 전달하는 거라, 중간 콤보마다 "X2 → X3 → X4" 텍스트를 갱신하려면 매 매치 라운드마다 발사하는 이벤트가 필요.
 
**BoardProcessor 변경**
 
`ProcessMatches`에 `Action<int> onComboUpdated` 콜백 파라미터 추가.
while 루프 안에서 `result.comboCount >= 2`일 때 콜백 호출.
기획서대로 첫 매치는 표기하지 않고 2콤보부터 X2 출력.
 
**BoardManager 변경**
 
`UnityEvent<int> _onComboUpdated` 이벤트 추가.
`OnSwapComplete`에서 `ProcessMatches` 호출 시 콤보 콜백을 `_onComboUpdated.Invoke`로 연결.
인터셉터 경로(튜토리얼)에서도 동일하게 연결.
 
### ComboDisplayUI 작성
 
퍼즐 보드 위에 콤보 텍스트를 표시하는 UI 컴포넌트.
퍼즐 보드 프리팹에 배치하여 전투 시스템과 독립적으로 동작.
 
- TextMeshPro + CanvasGroup 기반
- 등장: 스케일 0→1 `Ease.OutBack` (오버슈트 팝핑) + 페이드인
- 유지: `_displayDuration` 동안 표시
- 퇴장: 페이드아웃
- 연쇄 중 콤보 갱신 시 기존 연출 Kill 후 새로 재생
- 연쇄 완료 시 `OnChainComplete`로 즉시 숨김
- TMP Inspector에서 Outline(검정, 0.15~0.2) + 노랑/주황 텍스트로 가독성 확보
 
### ComboDisplayBinder 작성
 
`UnityEvent<int>`는 Inspector에서 동적 파라미터를 못 받으므로 코드로 연결.  
`OnEnable`/`OnDisable`에서 `AddListener`/`RemoveListener`.  
람다 리스너는 `RemoveListener` 시 인스턴스 불일치 문제가 있어 `UnityAction<PuzzleResult>`로 캐싱.  

### 에디터 설정 사항
 
**콤보 텍스트 TMP Material**
 
기본 TMP Material에서 Outline을 켜면 같은 Font Asset을 쓰는 모든 텍스트에 적용됨.
콤보 텍스트 전용 Material Preset을 별도로 생성하여 다른 텍스트에 영향 없도록 분리.
 
- Font Asset 우클릭 → Create Material Preset → 이름: `Font_Combo_Outline`
- Face Color: 노랑 계열
- Outline: 켜기, 두께 0.15, 검정
- Underlay(그림자): 가독성 보강용
- ComboDisplayUI의 TMP 컴포넌트 → Material에 `Font_Combo_Outline` 할당
 
**ComboDisplayUI 프리팹 배치**
 
- 퍼즐 보드 패널 자식으로 빈 GameObject 생성
- `ComboDisplayUI` + `ComboDisplayBinder` + `CanvasGroup` 부착
- 자식에 TextMeshPro 텍스트 추가 → `_comboText`에 할당
- `ComboDisplayBinder`의 `_boardManager`에 씬 상의 BoardManager 할당
- 텍스트 위치는 퍼즐판 중앙 배치, 앵커 Center

## Day 10 - 2026-04-02

### 퍼즐 보드 및 블록 최종 에셋 적용

기존 플레이스홀더 리소스를 최종 디자인 에셋으로 전면 교체하여 인게임 비주얼 완성.

- **BlockDataSO 데이터 변경**: SO에 등록된 블록별 이미지 에셋을 최종본으로 교체.
- **보드 배경 이미지 교체**: 석재 질감의 메인 백그라운드 이미지 적용.
- **그리드 셀 이미지 교체**: 블록이 배치되는 개별 슬롯의 가이드 이미지 적용.

---

### 블록 비주얼 최적화 및 레이아웃 조정

새로운 그리드 셀 디자인과의 조화를 위해 블록의 렌더링 규격 수정.

- **블록 겹침 방지**: 그리드 셀 이미지 변경 후 블록이 그리드셀 경계에 맞닿아 보이는 시각적 답답함 해소.
- **사이즈 축소 및 여백 추가**: 블록 이미지 상하좌우에 각각 **5씩 여백**을 주어 실질적인 출력 사이즈 축소.
- **결과**: 블록 간 경계가 명확해지고 그리드 내부에 안정적으로 안착된 비주얼 구현.

## Day 11 - 2026-04-03

---

### 퍼즐 리필 버그 수정

3매치 후 빈 슬롯에 블록이 채워지지 않고 영구적인 빈칸(유령 블록)이 남는 현상 수정.

**원인**

블록 파괴 이펙트 재생 시간이 `clearDelay`보다 길 때 발생하는 타이밍 문제.

1. 매치된 블록이 `Despawn` → 이펙트 재생 시작 (`activeSelf = true`, `Status = Destroying`)
2. 이펙트가 끝나기 전에 `clearDelay` 만료 → 낙하 로직 실행
3. 낙하 로직이 `activeSelf`만 체크해서 이펙트 재생 중인 블록을 정상 블록으로 오인, 낙하 대상에 포함
4. 이펙트 종료 콜백이 뒤늦게 `SetActive(false)` 호출 → 이미 낙하/리필된 블록이 증발, 영구 빈칸 발생

**수정 내역**

| 파일 | 수정 | 목적 |
|------|------|------|
| **UIFrameEffect** | `_runningCoroutine` 참조 추적 + `ForceStopEffect()` 추가 | 코루틴 중복 재생 방지 + 강제 중단 시 콜백 보장 |
| **Block** | `Despawn(Action onComplete)` 콜백 파라미터 추가 | 이펙트 완료 시점을 외부에 알림 |
| **Block** | `Init`/`Despawn`에 `DOTween.Kill(Rect)` 추가 | 재활용 시 잔여 Tween의 상태 오염 방지 |
| **Block** | `Init`에서 `ForceStopEffect()` + `_blockImage.enabled = true` | 이펙트 고아 코루틴 방지 + 숨겨진 이미지 복원 |
| **BoardProcessor** | `ClearMatches` → `remaining` 카운트다운 콜백 패턴 | 모든 Despawn 실제 완료 후 다음 단계 진행 |
| **BoardProcessor** | `clearDelay` + `WaitUntil(clearDone)` 이중 대기 | 최소 연출 대기 보장 + 이펙트가 더 길면 완료까지 추가 대기 |
| **BoardProcessor** | `DropBlocks`에 `Status == EBlockStatus.None` 조건 추가 | Destroying 상태 블록 낙하 제외 (방어적 이중 안전장치) |

**극한 테스트 (전체 통과)**

1. **달팽이 이펙트**: 이펙트 FPS 1~2 (3~5초), clearDelay 0 → 이펙트 완료까지 대기, 유령 블록 없음
2. **긴 딜레이**: 이펙트 FPS 60 (0.1초), clearDelay 3초 → 이펙트 종료 후 3초 정적 대기 후 낙하
3. **머신건 콤보**: clearDelay 0.05, dropDuration 0.01, refillDelay 0 → 고속 연쇄 중 상태 오염 없이 정상 완료

---

### 콤보 UI 애니메이션

기획 명세: 퍼즐판 중앙에 콤보 텍스트 출력, 커졌다가 작아지면서 위로 떠오르는 Floating 연출, 지속시간 1초.

**수정 전 상태**
 
- 콤보 중복 출력 방지 (`_currentSequence?.Kill()` 후 덮어쓰기) → OK
- 첫 매치 미표기, X2부터 출력 → BoardManager 쪽 책임 → OK
- `OnChainComplete`로 연쇄 종료 시 즉시 숨김 → OK
- Floating 연출 → **미구현**. `_originalPosition` 캐싱만 해놓고 Y 이동 Tween 없음
- 지속시간 → `_displayDuration` 1.5f + fadeIn/Out 합치면 약 2초. 명세 1초 초과
 
**Binder 람다 리스너 해제 버그**
 
`OnPuzzleComplete`에 람다 `_ => _display.OnChainComplete()`로 AddListener, OnDisable에서 새 람다로 RemoveListener → 다른 인스턴스라 해제 안 됨.
Awake에서 `UnityAction<PuzzleResult>` 래퍼 캐싱, 동일 참조로 Add/Remove하도록 수정.
 
**연출 시간 조정**
 
매칭 클리어 간격이 약 0.3초로 빠른 편이라, 기획서 1초 기준으로는 다음 콤보 갱신 전에 이전 텍스트가 채 사라지지 않음.
실제 게임 템포에 맞춰 총 0.5초(페이드인 0.05 / 유지 0.25 / 페이드아웃 0.2)로 조정.
다음 매치 진입 시 기존 연출 즉시 Kill → 갱신하므로 겹침 없음.
 
**최종 연출 흐름 (DOTween Sequence)**
 
```
1. 페이드인 (0.05s): alpha 0→1 + 스케일 0→1 (OutBack 오버슈트)
2. 대기 (0.25s): 선명한 상태 유지
3. 상승 + 페이드아웃: floatDistance만큼 Y 이동 (OutCubic, 0.2s)
   페이드아웃은 Insert로 등장+대기 이후 시점에 끼워넣어 상승 중 서서히 사라짐
```

---

### 콤보 TMP Raycast Target 수정
 
콤보 텍스트 TMP에 Raycast Target이 켜져 있어서 퍼즐판 위의 블록 드래그 입력을 가로채는 문제.
→ `_comboText`의 Raycast Target OFF 처리.

## Day 12 - 2026-04-06

### 퍼즐 SFX 개발

- 중첩 재생: PlayOneShot 사용으로 짧은 간격의 연쇄 폭발 시에도 사운드 잔향이 잘리지 않고 자연스럽게 레이어 재생됨.

- 오디오 소스 분리: _swapSource와 _comboSource를 분리하여 스왑 피드백과 콤보 연출음이 겹칠 때의 간섭 가능성 배제 및 개별 볼륨/피치 제어 기반 마련.

- 리소스 매칭:
  - 블록 스왑: switch puzzle.wav (0.2s)
  - 저단계 콤보 (1~2): combo6.wav (0.2s)
  - 고단계 콤보 (3 이상): combo4.wav (0.3s)

퍼즐 차원에서는 블록 바꾼 SFX와 낮은 단계 콤보 SFX, 높은 단계 SFX를 재생해주면 된다.

퍼즐이 매칭될때 매칭 숫자를 넘겨받아 그에 따른 사운드를 재생해주면 됨.

### PuzzleComponentsBinder

기존의 콤보 UI를 위해 인스펙터 상에서 보드 매니저를 할당받아 액션을 캐싱하고 리스너 추가 및 삭제를 관리하던 ComboDisplayBinder 스크립트를 PuzzleComponentsBinder로 변경

BoardManager의 이벤트를 UI와 SFX 컴포넌트에 연결하는 중계 스크립트.

- 의존성 최소화: 보드 매니저가 사운드나 UI의 구체적인 구현을 몰라도 되도록 설계 (느슨한 결합).
- 이벤트 매핑:
  - `OnSwapFinished` -> `PuzzleSFX.PlaySwapSfx`
  - `OnComboUpdated` -> `ComboDisplayUI.OnComboUpdated` & `PuzzleSFX.PlayComboSfx`
  - `OnPuzzleComplete` -> `ComboDisplayUI.OnChainComplete`

- 인스펙터 작업
  - 직렬화 참조: 모든 사운드 클립과 오디오 소스를 인스펙터에서 직접 할당하여 런타임 GetComponent 부하 제거 및 직관적인 데이터 관리.
  - 프리팹화: 캔버스 하위에 SFX와 바인더를 포함하여 퍼즐 시스템의 모듈화 및 재사용성 극대화.

**퍼즐 보드 프리팹 구조**
```
Canvas_PuzzleBoard_111
├── BG
├── GridsArea
├── Blocks
├── ComboUI
└── PuzzleSFX
   ├── SFX_Swap
   └── SFX_Combo
```

### 사운드 설정 개발(슬라이더바)

SoundManager가 Addressable로 AudioMixer를 동적 로드하고, 볼륨 제어 로직은 MixerVolumeController 순수 C# 클래스로 분리.

VolumeSlider 모노비헤이비어 스크립트가 SoundManager.Volume과 UI 슬라이더를 연결.
- SoundManager.IsReady 대기 후 저장된 볼륨 값으로 슬라이더 초기화
- 슬라이더 조작 시 MixerVolumeController.SetVolume()으로 실시간 반영

PlayerPrefs 영구 저장은 기획상 배제, 런타임에 MixerVolumeController 인스턴스에 보관.

**구조**
- SoundManager: AudioSource 생성 + Addressable 믹서 로드 + 재생 API
- MixerVolumeController: AudioMixer 볼륨 설정/조회 (순수 C#)
- VolumeSlider: UI Slider ↔ MixerVolumeController 바인딩 (MonoBehaviour)

## Day 13 - 2026-04-07

폴리싱 작업 서포트 및 문서정리

### 스프라이트 아틀라스 패킹 작업 진행

아틀라스로 묶을 수 있는 스프라이트들 아틀라스 패킹

아틀라스에 들어가는 파일은 이중 압축이 되니 원본 에셋 압축은 None으로 설정

AllowRotation 체크 해제  
UI가 회전되어 출력되는 현상 방지

외부 에셋스토어 에셋은 관리가 어려우니 아틀라스 패킹 고려 대상에서 제외

퍼즐과 스킬아이콘, 통상적으로 자주 사용되는 이미지들을 각자 아틀라스로 패킹함