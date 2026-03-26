# TEMFKing_작업 노트

**작성자**: 이성규  
**게임명**: 스러진 왕의 영원한 행진(The Eternal March of the Fallen King)  
**작성일**: 2026-03-23  
**최종 수정**: 2026-03-24  

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

### 진행 작업 리스트
- 콤보 카운트 및 퍼즐 결과 아웃풋
- 이후 폴리싱으로 DoTween 연출, 셀 하이라이트, 초기 보드 3매치 방지, 데드락 판정 

- 드래그 중 하이라이트: 놓을 위치에 임시 스왑 후 FindAllMatches로 매치 가능 여부 확인