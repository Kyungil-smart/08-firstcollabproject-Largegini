# TEMFKing_R&D문서_이성규

프로젝트명: 스러진 왕의 영원한 행진(The Eternal March of the Fallen King)
개발 환경: Unity 6 LTS  C#  URP 2D  
연구 목표: 3매치 퍼즐의 구현

---

작성일 2026-03-23  
최종 수정 2026-03-24  
작성자 이성규

---

코드 컨밴션
```cs
메서드클래스, , // 클래스: PascalCase
public class PlayerController { }

// 메서드: PascalCase
void MovePlayer() { }

// 변수: camelCase
float moveSpeed;

// private 필드: _camelCase (선택)
private float _currentSpeed;

// 상수: UPPER_CASE
const float MAX_SPEED = 10f;

// 프로퍼티: PascalCase
public bool IsGrounded { get; private set; }

// Enum : E + PascalCase
enum EType;

// 구조체 : S + PascalCase
struct SItemInfo;

// 인터페이스 : I + PascalCase
interface IMove;
```


---
## 3매치 구현 방식 학습

Catlike Coding 사이트에 기술 된 3 매치 구현 방식을 연구했다.
https://catlikecoding.com/unity/tutorials/prototypes/match-3/

### 구현 순서 구상
- 1단계: Grid2D 구조체 만들기
  - 제네릭 Grid2D 구조체를 먼저 만든다.
  - 1차원 배열 내부에 y * width + x로 인덱싱하고, Swap, 좌표 유효성 체크, 인덱서를 넣는다.
- 2단계: BlockState enum + 보드 초기화
  - BlockState를 None, Attack, Defense, Heal, Special로 정의하고, Grid2D<BlockState>로 5x6 보드를 만들기.
  - 랜덤 채울 때 왼쪽 2칸, 아래 2칸 체크해서 초기 3매치 방지 로직.
  - 콘솔 로그로 보드 상태 찍어서 검증.
- 3단계: 매치 판정
  - 행 스캔(가로) + 열 스캔(세로)으로 연속 3개 이상 같은 타입을 찾는 FindMatches 구현.
  - Match 구조체에 시작 좌표, 길이, 방향을 담아서 리스트로 모으기.
- 4단계: 소멸 + 드롭 + 리필
  - 매칭된 셀을 None으로 바꾸고, 열 단위로 빈 칸 위의 블록을 내리고, 상단에 새 랜덤 블록 채우기.
- 5단계: 체인 루프
  - 드롭 후 다시 FindMatches 호출해서 연쇄가 있으면 반복. 콤보 카운트 누적.

### 퍼즐 로직 추가

---

**보드 구조**

전체 그리드를 12행 x 5열(보이는 6행 + 버퍼 6행)로 통합 관리한다. `SGrid2D<Block>`은 하나만 쓰고, y 0~5가 플레이 영역, y 6~11이 버퍼 영역이다. 그리드 슬롯 오브젝트는 60개(보드 30 + 버퍼 30)를 배치하되, 보드 패널에 `RectMask2D`를 걸어서 버퍼 행은 시각적으로 가린다.

**블록 라이프사이클**

블록 GameObject는 초기 생성 후 파괴하지 않는다. 매치 소멸 시 연출(알파 off 등) 후 비활성 상태로 전환하고, 버퍼 최상단 슬롯으로 논리적 재배치한 뒤 새 SO 데이터로 `Init()`하여 재사용한다.

**낙하 로직**

매치 제거 후 각 열별로 빈 칸을 아래부터 탐색한다. 빈 칸 위에 있는 블록들을 순차적으로 아래로 당기고, 버퍼 행의 블록들이 보드 영역으로 내려온다. 이동은 DoTween `DOAnchorPos`로 목표 슬롯 좌표까지 연출하며, 열별로 약간의 delay를 주어 캐스케이드 느낌을 준다.

**블록 상태 관리**

블록에 상태 구분을 둔다(활성 / 연출중 / 비활성 등). 연출중·비활성 블록은 입력 및 매칭 판정에서 제외한다. 낙하 완료 후 전체 블록이 활성 상태가 되면 보드 전체 매칭 스캔을 수행한다.

**시드 기반 초기화**

`UnityEngine.Random` 대신 `System.Random` 인스턴스를 사용한다. 스테이지 SO에 시드값 필드를 두고, CSV 파이프라인으로 세팅한다. 시드가 있으면 해당 시드로, 없으면 순수 랜덤으로 보드를 생성한다.

**매칭 판정 범위**

매칭 체크는 보이는 보드 영역(y 0~5)에서만 수행한다. 버퍼 행 블록은 내려와서 보드에 안착한 시점부터 판정 대상이 된다.

수직선: │

중간 가지: ├──

끝 가지: └──


Assets/Scripts/Puzzle/
├── Core/
│   ├── Grid2D.cs          ← 제네릭 그리드 구조체
│   ├── BlockType.cs       ← enum
│   ├── Match.cs           ← 매치 결과 구조체
│   └── MatchResult.cs     ← 전투에 넘길 집계 구조체
├── Board/
│   ├── BoardManager.cs    ← 보드 초기화, 매치/드롭 관리
│   └── BoardVisual.cs     ← 그리드 슬롯 색상 반영
└── Input/
    └── BlockDragHandler.cs ← 드래그 앤 드롭 처리

Scripts/Puzzle/Core/
├── BlockType.cs        ← enum: None, Attack, Defense, Heal, Special
├── BlockDataSO.cs      ← SO: 타입별 색상, 스프라이트, 효과 수치
└── Grid2D.cs           ← 이미 완성

Scripts/Puzzle/Board/
├── Block.cs            ← MonoBehaviour: BlockType 세팅, 이미지 교체, 드래그
└── BoardManager.cs     ← 보드 초기화, 블록 배치