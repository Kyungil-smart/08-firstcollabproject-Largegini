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

## 테스트용 UI 및 프리팹 제작

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

## Grid2D 스크립트 작성

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


## 임시 메모
BlockType (enum) — None, Attack, Defense, Heal, Special

BlockDataSO (ScriptableObject) — 타입별 데이터
├── BlockType type
├── Color color          ← 임시 컬러 (나중에 스프라이트로 교체)
├── Sprite sprite        ← 나중에 픽셀 아트 들어오면
└── float effectValue    ← 블록당 효과 수치 (공격 5, 방어 2 등)

Block (MonoBehaviour) — 그리드 위의 블록 오브젝트
├── BlockDataSO 참조
├── 자기 그리드 좌표 (x, y)
├── Image 컴포넌트 참조
└── SetBlock(BlockDataSO) — 타입 세팅 + 비주얼 반영

Scripts/Puzzle/Core/
├── BlockType.cs        ← enum: None, Attack, Defense, Heal, Special
├── BlockDataSO.cs      ← SO: 타입별 색상, 스프라이트, 효과 수치
└── Grid2D.cs           ← 이미 완성

Scripts/Puzzle/Board/
├── Block.cs            ← MonoBehaviour: BlockType 세팅, 이미지 교체, 드래그
└── BoardManager.cs     ← 보드 초기화, 블록 배치