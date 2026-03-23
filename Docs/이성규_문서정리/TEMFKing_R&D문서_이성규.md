# TEMFKing_R&D문서_이성규

프로젝트명: 스러진 왕의 영원한 행진(The Eternal March of the Fallen King)
개발 환경: Unity 6 LTS  C#  URP 2D  
연구 목표: 3매치 퍼즐의 구현

---

작성일 2026-03-23  
최종 수정 2026-03-23  
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
- 2단계: TileState enum + 보드 초기화
  - TileState를 None, Attack, Defense, Heal, Special로 정의하고, Grid2D<TileState>로 5x6 보드를 만들기.
  - 랜덤 채울 때 왼쪽 2칸, 아래 2칸 체크해서 초기 3매치 방지 로직.
  - 콘솔 로그로 보드 상태 찍어서 검증.
- 3단계: 매치 판정
  - 행 스캔(가로) + 열 스캔(세로)으로 연속 3개 이상 같은 타입을 찾는 FindMatches 구현.
  - Match 구조체에 시작 좌표, 길이, 방향을 담아서 리스트로 모으기.
- 4단계: 소멸 + 드롭 + 리필
  - 매칭된 셀을 None으로 바꾸고, 열 단위로 빈 칸 위의 블록을 내리고, 상단에 새 랜덤 블록 채우기.
- 5단계: 체인 루프
  - 드롭 후 다시 FindMatches 호출해서 연쇄가 있으면 반복. 콤보 카운트 누적.
