https://catlikecoding.com/unity/tutorials/prototypes/match-3/

# Match 3 매칭 타일

2D 그리드에서 타일 시퀀스를 매칭합니다.

- 시각화와 게임 로직을 분리합니다.
- 상태 전환을 애니메이션합니다.
- 매치 점수를 누적합니다.
- 가능한 수를 탐색합니다.

이 튜토리얼은 프로토타입 시리즈의 여섯 번째입니다. 간단한 매치-3 게임을 만들어 봅니다.

이 튜토리얼은 Unity 2022.3.2f1로 제작되었습니다.

---

## 게임

Bejeweled, Puzzle Quest 등 매치-3 류 게임은 매우 다양합니다. 이번에는 같은 타일 3개 이상을 가로 또는 세로로 매칭하는 아주 간단한 게임을 만듭니다. 이름은 그냥 Match 3으로 하겠습니다. 이런 장르의 게임들은 주로 비주얼로 차별화하면서 게임플레이 자체는 대부분 비슷합니다. 그래서 이번에는 게임 로직과 시각화를 엄격하게 분리합니다. 이렇게 하면 간접 레이어가 추가되지만, 이런 게임은 규모가 작고 단순해서 성능 걱정을 크게 할 필요가 없습니다.

이번에도 Paddle Square 프로젝트를 복제해서 시작하고, 필요 없는 것은 모두 제거합니다. 글로벌 포스트 FX 볼륨, 카메라, 메인 라이트만 남겨둡니다. 그림자를 비활성화하고 카메라 회전을 초기화한 뒤 위치를 (0, 0, −10)으로 설정합니다. 게임이 XY 축에 배치되기 때문입니다. 텍스트 프리팹도 남겨둡니다.

또한 이번에는 핫 리로딩을 완전히 지원하여 게임 플레이 중에 코드를 변경할 수 있게 하여 디버깅과 테스트를 더 쉽게 합니다.

---

## Match 3 Skin

게임 로직을 동일하게 유지하면서 시각화를 쉽게 교체할 수 있도록, 실제 게임 로직의 프록시 역할을 하는 `Match3Skin` 컴포넌트 타입을 도입합니다. 메인 게임 오브젝트는 이 스킨하고만 상호작용합니다. 매치-3 게임이 진행 중인지를 나타내는 퍼블릭 프로퍼티, 현재 바쁜 상태인지를 나타내는 프로퍼티, 그리고 새 게임을 시작하는 메서드와 작업을 수행하는 메서드를 추가합니다. 처음에는 더미로, 항상 플레이 중이고 바쁘지 않은 상태를 나타냅니다. 이 컴포넌트를 가진 Match 3 게임 오브젝트를 생성합니다.

```csharp
using TMPro;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

public class Match3Skin : MonoBehaviour
{
    public bool IsPlaying => true;

    public bool IsBusy => false;

    public void StartNewGame () {}

    public void DoWork () { }
}
```

다음으로, 평소처럼 게임의 메인 컨트롤러 역할을 하는 `Game` 컴포넌트가 있는 게임 오브젝트를 생성합니다. 매치-3 게임에 대한 설정 필드를 추가하는데, 스킨이지만 실제 게임처럼 취급합니다. 깨어날 때 새 게임을 시작합니다.

```csharp
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField]
    Match3Skin match3;

    void Awake () => match3.StartNewGame();
}
```

매 업데이트마다, 매치-3 게임이 진행 중이면 게임이 바쁘지 않은 경우 플레이어 입력을 처리하고, 그 후에 작업을 수행하도록 합니다. 게임이 진행 중이 아니면 스페이스를 누를 때 새 게임을 시작합니다. 입력 처리는 별도 메서드에서 하며, 처음에는 아무것도 하지 않습니다.

```csharp
    void Update ()
    {
        if (match3.IsPlaying)
        {
            if (!match3.IsBusy)
            {
                HandleInput();
            }
            match3.DoWork();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            match3.StartNewGame();
        }
    }

    void HandleInput () { }
```

이 접근법을 사용하면 여러 스킨, 게임 모드, 또는 하나의 앱에서 여러 게임을 쉽게 지원할 수 있습니다. 활성 게임만 바꾸면 됩니다.

---

## 입력 처리

`Match3Skin`에 단일 입력 방식을 지원하기 위해 퍼블릭 `EvaluateDrag` 메서드를 추가합니다. 이 메서드는 진행 중인 드래그 동작을 시작 위치와 끝 위치로 평가합니다. 이들은 화면 공간에서의 마우스 또는 터치 위치를 나타내는 `Vector3` 값입니다. 메서드는 드래그를 계속할지 중단할지를 반환합니다. 아직 입력을 처리하지 않으므로 드래그를 유지할 이유가 없습니다.

```csharp
    public bool EvaluateDrag (Vector3 start, Vector3 end)
    {
        return false;
    }
```

드래그를 지원하기 위해 `Game`에서 드래그 시작 위치와 드래그 중인지를 추적해야 합니다. `HandleInput`에서, 아직 드래그 중이 아니고 기본 마우스 버튼이 눌렸으면 드래그를 시작합니다. 드래그 중이고 버튼이 아직 눌려 있으면 드래그를 평가하고 그 결과로 계속할지 결정합니다. 그 외에는 드래그를 종료합니다.

```csharp
    Vector3 dragStart;

    bool isDragging;

    …

    void HandleInput ()
    {
        if (!isDragging && Input.GetMouseButtonDown(0))
        {
            dragStart = Input.mousePosition;
            isDragging = true;
        }
        else if (isDragging && Input.GetMouseButton(0))
        {
            isDragging = match3.EvaluateDrag(dragStart, Input.mousePosition);
        }
        else
        {
            isDragging = false;
        }
    }
```

---

## 타일

매치-3 게임을 시각화하려면 타일을 표시해야 합니다. 한 변의 길이가 1유닛인 작은 정사각형 타일을 사용합니다. 이 타일들은 XY 평면에 정렬되며 두께는 0.2유닛으로 합니다. 평소처럼 큐브만 사용해서 시각화합니다. 색맹인 사람도 구분할 수 있도록 시각적으로 쉽게 구별되는 7개의 타일 프리팹을 만듭니다. 모든 타일의 루트 게임 오브젝트는 항등 변환(회전 없음, 스케일 1)을 가져야 합니다.

타일들이 시각적으로 충분히 구별되면 각각 다른 색상을 지정할 수 있습니다. 이 순서로 하면 색맹인 사람도 타일을 구분할 수 있습니다.

많은 타일 인스턴스를 다루게 되므로 풀링을 적용합니다. 나중에 다른 것도 풀링할 예정이므로, 중복 코드를 피하기 위해 `MonoBehaviour` 프리팹용 제네릭 `PrefabInstancePool<T>` 구조체를 도입합니다. 스택을 래핑하며, 주어진 프리팹을 인스턴스화하는 퍼블릭 `GetInstance` 메서드와 주어진 인스턴스의 게임 오브젝트를 파괴하는 `Recycle` 메서드를 제공합니다. 이 풀은 직렬화할 수 없습니다.

```csharp
using System.Collections.Generic;
using UnityEngine;

public struct PrefabInstancePool<T> where T : MonoBehaviour
{
    Stack<T> pool;

    public T GetInstance (T prefab)
    {
        return Object.Instantiate(prefab);
    }

    public void Recycle (T instance)
    {
        Object.Destroy(instance.gameObject);
    }
}
```

`GetInstance` 메서드를 풀을 사용하도록 수정합니다: 필요하면 풀을 생성하고, 사용 가능한 인스턴스가 있으면 재사용하고 게임 오브젝트를 다시 활성화하며, 없으면 새 인스턴스를 생성합니다.

```csharp
    public T GetInstance (T prefab)
    {
        if (pool == null)
        {
            pool = new();
        }

        if (pool.TryPop(out T instance))
        {
            instance.gameObject.SetActive(true);
        }
        else
        {
            instance = Object.Instantiate(prefab);
        }
        return instance;
    }
```

도메인 리로딩 비활성화를 지원하기 위해, 풀이 존재하면 파괴된 참조가 포함되어 있는지 확인합니다. 그렇다면 플레이 모드 종료로 인해 풀이 남아있는 것으로 간주하고 이전 참조를 제거하기 위해 클리어합니다. 이것은 에디터에서만 필요합니다.

```csharp
        if (pool == null)
        {
            pool = new();
        }
#if UNITY_EDITOR
        else if (pool.TryPeek(out T i) && !i)
        {
            // 인스턴스가 파괴됨, 플레이 모드 종료로 인한 것으로 추정.
            pool.Clear();
        }
#endif
```

재활용 시, 에디터에서는 풀이 없는지 확인합니다. 없다면 핫 리로드로 인해 참조가 손실된 것으로 간주하고 그때만 게임 오브젝트를 파괴합니다. 그렇지 않으면 풀에 인스턴스를 추가하고 게임 오브젝트를 비활성화합니다.

핫 리로드 중 아직 사용 중이던 게임 오브젝트만 제거됩니다. 이미 재활용되었던 것들은 참조하던 풀이 사라졌으므로 재사용되지 않습니다. 따라서 핫 리로드를 할 때마다 영구적으로 비활성화된 인스턴스가 서서히 늘어나며, 플레이 모드를 종료할 때까지 유지됩니다.

```csharp
    public void Recycle (T instance)
    {
#if UNITY_EDITOR
        if (pool == null)
        {
            // 풀 손실, 핫 리로드로 인한 것으로 추정.
            Object.Destroy(instance.gameObject);
            return;
        }
#endif
        pool.Push(instance);
        instance.gameObject.SetActive(false);
    }
```

이제 이 풀을 사용하는 `Tile` 컴포넌트 타입을 생성합니다. 자기 자신의 인스턴스를 풀에서 가져오고, 같은 풀을 부여하고, 주어진 위치에 배치하는 퍼블릭 `Spawn` 메서드를 추가합니다. 또한 자기 자신을 재활용하는 퍼블릭 `Despawn` 메서드도 추가합니다.

```csharp
using UnityEngine;

public class Tile : MonoBehaviour
{
    PrefabInstancePool<Tile> pool;

    public Tile Spawn (Vector3 position)
    {
        Tile instance = pool.GetInstance(this);
        instance.pool = pool;
        instance.transform.localPosition = position;
        return instance;
    }

    public void Despawn () => pool.Recycle(this);
}
```

7개 타일 프리팹의 루트에 모두 이 컴포넌트를 추가합니다. 그런 다음 `Match3Skin`에 타일 프리팹 설정 배열을 추가하고 타일들을 할당합니다.

```csharp
    [SerializeField]
    Tile[] tilePrefabs;
```

---

## 기본 게임플레이

기본 매치-3 게임플레이는 2D 그리드를 타일로 채운 뒤, 플레이어가 타일을 교환하여 매치를 만드는 것입니다. 매칭된 타일은 제거되고, 위의 타일들이 떨어져 빈 칸을 채우며, 필요에 따라 새 타일이 추가됩니다.

### 2D 그리드

게임 로직과 스킨 모두 2D 그리드를 다뤄야 하므로, 이를 쉽게 하기 위한 제네릭 직렬화 가능 `Grid2D<T>` 구조체를 도입합니다. 내부 셀 배열로 데이터를 저장하고 2D 크기를 `int2`로 추적하며, 생성자 메서드에 전달합니다.

```csharp
using Unity.Mathematics;

[System.Serializable]
public struct Grid2D<T>
{
    T[] cells;

    int2 size;

    public Grid2D (int2 size)
    {
        this.size = size;
        cells = new T[size.x * size.y];
    }
}
```

> **왜 다차원 배열을 쓰지 않나요?**

크기에 대한 퍼블릭 getter 프로퍼티와 개별 크기 컴포넌트 프로퍼티를 추가합니다.

```csharp
    public int2 Size => size;

    public int SizeX => size.x;

    public int SizeY => size.y;
```

Unity 직렬화 맥락에서 그리드가 정의되지 않았는지 편리하게 확인할 수 있도록 합니다. 배열이 없거나 배열 길이가 0이면 정의되지 않은 것입니다.

```csharp
    public bool IsUndefined => cells == null || cells.Length == 0;
```

그리드 요소를 가져오고 설정하기 위한 두 개의 인덱서를 추가합니다. 별도의 X, Y 좌표를 받는 것과 단일 `int2` 좌표 쌍을 받는 것입니다.

```csharp
    public T this[int x, int y]
    {
        get => cells[y * size.x + x];
        set => cells[y * size.x + x] = value;
    }

    public T this[int2 c]
    {
        get => cells[c.y * size.x + c.x];
        set => cells[c.y * size.x + c.x] = value;
    }
```

주어진 좌표가 유효한지 확인하는 메서드를 포함합니다. 단일 `int2` 파라미터 버전만 필요하지만, 별도 좌표 파라미터 버전도 추가할 수 있습니다.

```csharp
    public bool AreValidCoordinates (int2 c) =>
        0 <= c.x && c.x < size.x && 0 <= c.y && c.y < size.y;
```

마지막으로, 좌표가 주어진 두 요소를 교환하는 메서드를 추가합니다.

```csharp
    public void Swap (int2 a, int2 b) => (this[a], this[b]) = (this[b], this[a]);
```

---

### 게임 시작

게임 상태를 저장하기 위해 타일을 표현해야 하며, `TileState` 열거형을 만들어 A부터 G까지 7개의 상태로 이름을 붙입니다. 빈 타일 공간을 나타내기 위한 기본 0 상태 `None`도 포함합니다.

```csharp
public enum TileState
{
    None, A, B, C, D, E, F, G
}
```

게임 상태와 로직은 새로운 `Match3Game` 컴포넌트 타입이 담당합니다. `Match3Skin` 컴포넌트와 같은 게임 오브젝트에 추가할 수 있습니다. 기본값 8×8의 크기 설정 필드를 추가합니다. 또한 `TileState` 그리드와, 그리드의 인덱서 및 크기를 전달하는 getter 프로퍼티를 추가합니다. 이렇게 하면 `Match3Game`만 그리드 상태를 변경할 수 있습니다.

```csharp
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

using Random = UnityEngine.Random;

using static Unity.Mathematics.math;

public class Match3Game : MonoBehaviour
{
    [SerializeField]
    int2 size = 8;

    Grid2D<TileState> grid;

    public TileState this[int x, int y] => grid[x, y];

    public TileState this[int2 c] => grid[c];

    public int2 Size => size;
}
```

그리드가 정의되지 않은 경우 새 그리드를 생성한 다음 `FillGrid`를 호출하는 퍼블릭 `StartNewGame` 메서드를 추가합니다. `FillGrid`는 모든 그리드 행을 순회하며 랜덤 상태로 채웁니다.

```csharp
    public void StartNewGame ()
    {
        if (grid.IsUndefined)
        {
            grid = new(size);
        }
        FillGrid();
    }

    void FillGrid ()
    {
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                grid[x, y] = (TileState)Random.Range(1, 8);
            }
        }
    }
```

다음으로, `Match3Skin`에 `Match3Game` 설정 필드를 추가하고 연결합니다. 그런 다음 자체 `Tile` 요소 그리드와 월드 공간에서 타일을 배치하기 위한 2D 타일 오프셋을 추가합니다.

```csharp
    [SerializeField]
    Match3Game game;

    Grid2D<Tile> tiles;

    float2 tileOffset;
```

새 게임을 시작할 때 먼저 실제 게임으로 호출을 전달합니다. 그런 다음 원점 중심에 타일을 배치하도록 타일 오프셋을 설정합니다. 타일 그리드가 정의되지 않으면 새로 생성하고, 그렇지 않으면 모든 타일을 디스폰하고 참조를 클리어합니다. 참조 클리어는 꼭 필요하진 않지만 버그 탐지가 더 쉬워집니다.

```csharp
    public void StartNewGame () {
        …

        game.StartNewGame();
        tileOffset = -0.5f * (float2)(game.Size - 1);
        if (tiles.IsUndefined)
        {
            tiles = new(game.Size);
        }
        else
        {
            for (int y = 0; y < tiles.SizeY; y++)
            {
                for (int x = 0; x < tiles.SizeX; x++)
                {
                    tiles[x, y].Despawn();
                    tiles[x, y] = null;
                }
            }
        }
    }
```

그 후 모든 행을 순회하며 적절한 타일 인스턴스를 스폰하고 그리드에 추가합니다. 타일 상태와 좌표가 주어지면 단일 타일을 스폰하는 별도의 `SpawnTile` 메서드를 만듭니다.

```csharp
    public void StartNewGame () {
        …

        for (int y = 0; y < tiles.SizeY; y++)
        {
            for (int x = 0; x < tiles.SizeX; x++)
            {
                tiles[x, y] = SpawnTile(game[x, y], x, y);
            }
        }
    }

    Tile SpawnTile (TileState t, float x, float y) =>
        tilePrefabs[(int)t - 1].Spawn(new Vector3(x + tileOffset.x, y + tileOffset.y));
```

---

### 즉시 매치 방지

플레이 모드에 들어가면 랜덤 타일로 채워진 그리드를 볼 수 있습니다. 완전히 랜덤이므로 초기 상태에 이미 3개 이상의 수평 또는 수직 매칭 시퀀스가 포함될 수 있습니다. 이를 피하기 위해 `Match3Game.FillGrid`를 조정해야 합니다.

배치되는 각 타일은 최대 2개의 매치(수평 1개, 수직 1개)를 생성할 수 있습니다. 이를 감지해서 피해야 합니다. 타일 상태 A와 B를 추적하고, 감지한 잠재적 매치 수를 기록합니다. 왼쪽으로 2개 이상의 타일이 있으면 A를 그중 하나로 설정하고, 둘 다 같으면 잠재적 매치 수를 1로 설정합니다.

```csharp
    void FillGrid ()
    {
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                TileState a = TileState.None, b = TileState.None;
                int potentialMatchCount = 0;
                if (x > 1)
                {
                    a = grid[x - 1, y];
                    if (a == grid[x - 2, y])
                    {
                        potentialMatchCount = 1;
                    }
                }

                grid[x, y] = (TileState)Random.Range(1, 8);
            }
        }
    }
```

아래쪽 2개 타일에 대해서도 같은 작업을 합니다. B에 하나를 할당하고 잠재적 매치 수를 증가시킵니다. 단, 매치가 1개뿐이면 대신 A를 사용하고, 매치가 2개이면 A와 B가 오름차순이 되도록 합니다.

```csharp
                if (y > 1)
                {
                    b = grid[x, y - 1];
                    if (b == grid[x, y - 2])
                    {
                        potentialMatchCount += 1;
                        if (potentialMatchCount == 1)
                        {
                            a = b;
                        }
                        else if (b < a)
                        {
                            (a, b) = (b, a);
                        }
                    }
                }
```

이제 랜덤 범위를 매치 수만큼 줄이고 필요에 따라 A와 B를 건너뛰어 매치를 피할 수 있습니다.

```csharp
                TileState t = (TileState)Random.Range(1, 8 - potentialMatchCount);
                if (potentialMatchCount > 0 && t >= a)
                {
                    t += 1;
                }
                if (potentialMatchCount == 2 && t >= b)
                {
                    t += 1;
                }
                grid[x, y] = t;
```

---

### 이동 수행

이 게임에서 이동은 타일을 선택하고 인접한 이웃과 교환하는 것입니다. 허용되는 방향에 대한 `MoveDirection` 열거형을 도입합니다. 위, 오른쪽, 아래, 왼쪽이며, 잘못된 이동을 나타내는 기본 0 상태 `None`도 포함합니다.

```csharp
public enum MoveDirection
{
    None, Up, Right, Down, Left
}
```

또한 전체 이동 액션을 저장하기 위한 직렬화 가능 `Move` 구조체를 만듭니다. 방향, 출발 좌표, 도착 좌표 프로퍼티와, 방향이 설정되었는지에 기반한 유효성 프로퍼티를 가집니다. 프로퍼티는 생성자 메서드를 통해 설정되며, 시작 좌표와 방향만 필요하고 목적지 좌표는 자동으로 결정됩니다.

```csharp
using Unity.Mathematics;

using static Unity.Mathematics.math;

[System.Serializable]
public struct Move
{
    public MoveDirection Direction
    { get; private set; }

    public int2 From
    { get; private set; }

    public int2 To
    { get; private set; }

    public bool IsValid => Direction != MoveDirection.None;

    public Move (int2 coordinates, MoveDirection direction)
    {
        Direction = direction;
        From = coordinates;
        To = coordinates + direction switch
        {
            MoveDirection.Up => int2(0, 1),
            MoveDirection.Right => int2(1, 0),
            MoveDirection.Down => int2(0, -1),
            _ => int2(-1, 0)
        };
    }
}
```

이제 `Match3Game`에 이동을 받아 성공 여부(매치가 만들어졌는지)를 반환하는 퍼블릭 `TryMove` 메서드를 추가합니다. 우선 항상 from과 to 타일을 교환하고 성공을 나타냅니다.

```csharp
    public bool TryMove (Move move)
    {
        grid.Swap(move.From, move.To);
        return true;
    }
```

다음으로, `Match3Skin`에 주어진 이동을 시도하고, 성공하면 타일 위치와 타일 자체를 모두 교환하는 프라이빗 `DoMove` 메서드를 추가합니다.

```csharp
    void DoMove (Move move)
    {
        if (game.TryMove(move))
        {
            (
                tiles[move.From].transform.localPosition,
                tiles[move.To].transform.localPosition
            ) = (
                tiles[move.To].transform.localPosition,
                tiles[move.From].transform.localPosition
            );
            tiles.Swap(move.From, move.To);
        }
    }
```

어떤 타일을 교환해야 하는지 알기 위해 스크린 공간에서 스킨의 타일 공간으로 변환하는 메서드를 도입합니다.

```csharp
    float2 ScreenToTileSpace (Vector3 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Vector3 p = ray.origin - ray.direction * (ray.origin.z / ray.direction.z);
        return float2(p.x - tileOffset.x + 0.5f, p.y - tileOffset.y + 0.5f);
    }
```

이제 `EvaluateDrag`에서 드래그의 시작과 끝을 타일 좌표로 변환할 수 있습니다. 드래그 임계값 설정 필드를 추가합니다. 기본값은 타일의 반인 0.5입니다. 드래그 델타를 확인하여 드래그 방향을 결정합니다. 이동과 두 좌표 세트가 모두 유효하면 이동을 수행하고 false를 반환하여 드래그를 중지합니다. 그렇지 않으면 true를 반환하여 드래그를 계속할 수 있게 합니다.

```csharp
    [SerializeField, Range(0.1f, 1f)]
    float dragThreshold = 0.5f;

    …

    public bool EvaluateDrag (Vector3 start, Vector3 end)
    {
        float2 a = ScreenToTileSpace(start), b = ScreenToTileSpace(end);
        var move = new Move(
            (int2)floor(a), (b - a) switch
            {
                var d when d.x > dragThreshold => MoveDirection.Right,
                var d when d.x < -dragThreshold => MoveDirection.Left,
                var d when d.y > dragThreshold => MoveDirection.Up,
                var d when d.y < -dragThreshold => MoveDirection.Down,
                _ => MoveDirection.None
            }
        );
        if (
            move.IsValid &&
            tiles.AreValidCoordinates(move.From) && tiles.AreValidCoordinates(move.To)
            )
        {
            DoMove(move);
            return false;
        }
        return true;
    }
```

이제 드래그를 통해 타일을 이웃과 교환할 수 있게 되었습니다.

---

### 매치 찾기

매치가 만들어지는 이동만 허용하려면 그리드에서 매치를 스캔해야 합니다. 이를 위해 직렬화 가능한 `Match` 구조체를 도입합니다. 단순한 값 컨테이너이므로 모든 필드를 public으로 만듭니다. 매치의 첫 타일(왼쪽 하단) 좌표, 길이, 수평 매치인지 여부를 가집니다.

```csharp
using Unity.Mathematics;

[System.Serializable]
public struct Match
{
    public int2 coordinates;

    public int length;

    public bool isHorizontal;

    public Match (int x, int y, int length, bool isHorizontal)
    {
        coordinates.x = x;
        coordinates.y = y;
        this.length = length;
        this.isHorizontal = isHorizontal;
    }
}
```

`Match3Game`은 발견한 매치를 즉시 처리하지 않고 리스트에 저장합니다. 이렇게 하면 매치 감지와 처리 사이에 스킨이 다른 작업을 할 수 있습니다. 매치 자체는 `Match3Game`에 비공개로 유지되지만, 매치가 있는지 여부는 프로퍼티로 노출합니다. 게임 시작 시 필요하면 리스트도 생성합니다.

```csharp
    List<Match> matches;

    …

    public bool HasMatches => matches.Count > 0;

    public void StartNewGame ()
    {
        if (grid.IsUndefined)
        {
            grid = new(size);
            matches = new();
        }
        FillGrid();
    }
```

매치를 찾았는지 반환하는 `FindMatches` 메서드를 추가합니다. 프로퍼티를 호출하는 단축입니다.

```csharp
    bool FindMatches ()
    {
        return HasMatches;
    }
```

수평 매치부터 검색합니다. 각 행에서 시작 타일 상태를 첫 타일로 설정하고 매치 길이를 1로 설정합니다. 나머지 행을 순회하며 현재 타일이 시작과 일치하면 길이를 늘립니다. 일치하지 않으면서 길이가 3 이상이면 수평 매치를 리스트에 추가한 뒤 시작을 리셋합니다. 행 끝에서도 3+ 매치를 확인합니다.

```csharp
    bool FindMatches ()
    {
        for (int y = 0; y < size.y; y++)
        {
            TileState start = grid[0, y];
            int length = 1;
            for (int x = 1; x < size.x; x++)
            {
                TileState t = grid[x, y];
                if (t == start)
                {
                    length += 1;
                }
                else
                {
                    if (length >= 3)
                    {
                        matches.Add(new Match(x - length, y, length, true));
                    }
                    start = t;
                    length = 1;
                }
            }
            if (length >= 3)
            {
                matches.Add(new Match(size.x - length, y, length, true));
            }
        }

        return HasMatches;
    }
```

그런 다음 수직 매치에 대해서도 같은 작업을 합니다. 행 대신 열을 순회합니다.

```csharp
        for (int x = 0; x < size.x; x++)
        {
            TileState start = grid[x, 0];
            int length = 1;
            for (int y = 1; y < size.y; y++)
            {
                TileState t = grid[x, y];
                if (t == start)
                {
                    length += 1;
                }
                else
                {
                    if (length >= 3)
                    {
                        matches.Add(new Match(x, y - length, length, false));
                    }
                    start = t;
                    length = 1;
                }
            }
            if (length >= 3)
            {
                matches.Add(new Match(x, size.y - length, length, false));
            }
        }

        return HasMatches;
```

`TryMove`에서 교환 후 `FindMatches`를 호출하고 성공을 나타내면 true를 반환합니다. 그렇지 않으면 교환을 취소하고 false를 반환합니다.

```csharp
    public bool TryMove (Move move)
    {
        grid.Swap(move.From, move.To);
        if (FindMatches())
        {
            return true;
        }
        grid.Swap(move.From, move.To);
        return false;
    }
```

지금 플레이 모드에 들어가면 매치가 만들어지는 이동만 처음에 성공합니다. 하지만 그 후에는 자유롭게 이동할 수 있는데, 매치가 아직 클리어되지 않기 때문입니다.

---

### 매치 처리

매치가 발견되면 처리해야 합니다. 매치 처리란 매칭된 모든 타일을 제거하는 것입니다. 이를 스킨에 전달하기 위해, 제거된 타일 좌표 리스트를 노출하는 퍼블릭 프로퍼티를 `Match3Game`에 추가합니다. 또한 게임 상태를 채워야 하는지를 나타내는 프로퍼티도 추가합니다. 둘 다 비공개로 설정됩니다.

```csharp
    public List<int2> ClearedTileCoordinates
    { get; private set; }

    public bool NeedsFilling
    { get; private set; }

    public bool HasMatches => matches.Count > 0;

    public void StartNewGame ()
    {
        if (grid.IsUndefined)
        {
            grid = new(size);
            matches = new();
            ClearedTileCoordinates = new();
        }
        FillGrid();
    }
```

제거된 타일 좌표 리스트를 클리어한 다음 모든 매치를 순회하며 해당 타일을 모두 제거하고 좌표를 리스트에 추가하는 퍼블릭 `ProcessMatches` 메서드를 만듭니다. 수평과 수직 매치가 겹칠 수 있으므로 각 제거 타일을 한 번만 포함하도록 합니다. 완료되면 매치를 클리어하고 채움이 필요함을 표시합니다.

```csharp
    public void ProcessMatches ()
    {
        ClearedTileCoordinates.Clear();

        for (int m = 0; m < matches.Count; m++)
        {
            Match match = matches[m];
            int2 step = match.isHorizontal ? int2(1, 0) : int2(0, 1);
            int2 c = match.coordinates;
            for (int i = 0; i < match.length; c += step, i++)
            {
                if (grid[c] != TileState.None)
                {
                    grid[c] = TileState.None;
                    ClearedTileCoordinates.Add(c);
                }
            }
        }

        matches.Clear();
        NeedsFilling = true;
    }
```

이제 `Match3Skin`에 잠재적인 작업이 생겼습니다. 게임에 매치가 있으면 게임으로 전달하고 제거된 모든 타일을 디스폰하고 참조를 클리어하는 새 `ProcessMatches` 메서드를 호출합니다.

```csharp
    public void DoWork () {
        if (game.HasMatches)
        {
            ProcessMatches();
        }
    }

    void ProcessMatches ()
    {
        game.ProcessMatches();

        for (int i = 0; i < game.ClearedTileCoordinates.Count; i++)
        {
            int2 c = game.ClearedTileCoordinates[i];
            tiles[c].Despawn();
            tiles[c] = null;
        }
    }
```

매치를 만들면 빈 칸이 나타나기 시작합니다. 참조를 클리어했기 때문에 빈 칸을 교환하려고 하면 `NullReferenceException`이 발생합니다.

---

### 빈 칸 채우기

다음 단계는 매치로 생긴 빈 칸을 채우는 것입니다. 중력을 적용하여 떠 있는 타일을 아래로 떨어뜨립니다. 이를 전달하기 위해 직렬화 가능한 `TileDrop` 구조체를 도입합니다. 타일 좌표와 어디서 떨어졌는지를 나타내는 `fromY` 좌표를 포함합니다.

```csharp
using Unity.Mathematics;

[System.Serializable]
public struct TileDrop
{
    public int2 coordinates;

    public int fromY;

    public TileDrop (int x, int y, int distance)
    {
        coordinates.x = x;
        coordinates.y = y;
        fromY = y + distance;
    }
}
```

제거된 타일 리스트처럼 떨어진 타일 리스트를 `Match3Game`에 추가합니다.

```csharp
    public List<TileDrop> DroppedTiles
    { get; private set; }

    …

    public void StartNewGame ()
    {
        if (grid.IsUndefined)
        {
            grid = new(size);
            matches = new();
            ClearedTileCoordinates = new();
            DroppedTiles = new();
        }
        FillGrid();
    }
```

리스트를 클리어한 다음 모든 열을 순회하는 퍼블릭 `DropTiles` 메서드를 만듭니다. 아래에서 위로 가며 빈 칸 수를 추적합니다. 빈 칸을 만나면 수를 증가시킵니다. 그렇지 않으면서 아래에 빈 칸이 있으면 타일 상태를 적절한 거리만큼 아래로 떨어뜨리고 리스트에 항목을 추가합니다. 완료되면 채움이 더 이상 필요하지 않음을 표시합니다.

```csharp
    public void DropTiles ()
    {
        DroppedTiles.Clear();

        for (int x = 0; x < size.x; x++)
        {
            int holeCount = 0;
            for (int y = 0; y < size.y; y++)
            {
                if (grid[x, y] == TileState.None)
                {
                    holeCount += 1;
                }
                else if (holeCount > 0)
                {
                    grid[x, y - holeCount] = grid[x, y];
                    DroppedTiles.Add(new TileDrop(x, y - holeCount, holeCount));
                }
            }
        }

        NeedsFilling = false;
    }
```

이렇게 하면 기존 타일이 모두 떨어지며, 빈 칸이 효과적으로 위쪽으로 밀립니다. 이 빈 칸을 열당 랜덤 타일로 채웁니다. 새 타일에 대해서도 떨어진 항목을 추가하며, 원래 Y 좌표를 그리드 위 적절한 위치로 설정합니다.

```csharp
            for (int h = 1; h <= holeCount; h++)
            {
                grid[x, size.y - h] = (TileState)Random.Range(1, 8);
                DroppedTiles.Add(new TileDrop(x, size.y - h, holeCount));
            }
```

이제 `Match3Skin`에 할 작업이 더 생겼습니다. 처리할 매치가 없고 게임에 채움이 필요하면 자체 `DropTiles` 메서드를 호출하여 게임으로 전달합니다. 그런 다음 떨어진 모든 타일을 순회합니다. 타일이 그리드 내에서 떨어졌으면 위치를 조정합니다. 그렇지 않으면 적절한 위치에 새 타일을 스폰합니다. 그리고 타일 그리드를 업데이트합니다.

```csharp
    public void DoWork () {
        if (game.HasMatches)
        {
            ProcessMatches();
        }
        else if (game.NeedsFilling)
        {
            DropTiles();
        }
    }

    void DropTiles ()
    {
        game.DropTiles();

        for (int i = 0; i < game.DroppedTiles.Count; i++)
        {
            TileDrop drop = game.DroppedTiles[i];
            Tile tile;
            if (drop.fromY < tiles.SizeY)
            {
                tile = tiles[drop.coordinates.x, drop.fromY];
                tile.transform.localPosition = new Vector3(
                    drop.coordinates.x + tileOffset.x, drop.coordinates.y + tileOffset.y
                );
            }
            else
            {
                tile = SpawnTile(
                    game[drop.coordinates], drop.coordinates.x, drop.coordinates.y
                );
            }
            tiles[drop.coordinates] = tile;
        }
    }
```

떨어진 타일이 즉시 새 매치를 형성할 수 있으므로, `Match3Game.DropTiles` 끝에 `FindMatches`를 다시 호출해야 합니다. 이렇게 하면 매치가 없는 상태에 도달할 때까지 계속 연쇄됩니다.

```csharp
        NeedsFilling = false;
        FindMatches();
```

---

## 전환 애니메이션

게임이 최소한으로 동작하지만, 즉시 다음 상태로 전환되기 때문에 무슨 일이 일어나는지 보기 어렵습니다. 그래서 상태 전환을 도입하여 게임을 느리게 합니다. 이는 게임 로직에 영향을 주지 않고 스킨에만 영향을 줍니다.

### 타일 교환

이동으로 인한 타일 교환을 애니메이션합니다. 이 로직을 스킨에서 분리하기 위해 직렬화 가능한 `TileSwapper` 클래스를 만듭니다. 기본값 0.25초의 설정 가능한 duration을 제공하되, 전환을 쉽게 확인할 수 있도록 최대값을 10처럼 높게 설정합니다. 타일이 서로 관통하지 않도록 Z 차원에서 얼마나 이동할지를 제어하는 최대 깊이 오프셋 설정도 추가합니다. 기본값은 0.5유닛입니다.

```csharp
using UnityEngine;

[System.Serializable]
public class TileSwapper
{
    [SerializeField, Range(0.1f, 10f)]
    float duration = 0.25f;

    [SerializeField, Range(0f, 1f)]
    float maxDepthOffset = 0.5f;
}
```

교환을 시작하는 퍼블릭 `Swap` 메서드와 애니메이션하는 `Update` 메서드가 필요합니다. `Swap` 메서드는 교환할 타일 파라미터와 교환이 원래 위치로 돌아가야 하는지를 나타내는 핑퐁 파라미터를 받습니다. 교환 애니메이션의 duration을 반환합니다.

```csharp
    public float Swap (Tile a, Tile b, bool pingPong) {
        return duration;
    }

    public void Update () {}
```

`Match3Skin`에 스왑퍼 설정 필드와 스킨이 얼마나 오래 바쁜지를 나타내는 busy duration 필드를 추가합니다. 게임 시작 시 0으로 설정하고 `IsBusy`는 이 값이 0보다 큰지를 반환합니다.

```csharp
    [SerializeField]
    TileSwapper tileSwapper;

    float busyDuration;

    …

    public bool IsBusy => busyDuration > 0f;

    public void StartNewGame () {
        busyDuration = 0f;
        …
    }
```

이제 `DoWork`는 busy duration이 0보다 큰지 먼저 확인해야 합니다. 그렇다면 타일 스왑퍼를 업데이트하고 남은 시간을 줄인 뒤, 아직 시간이 남아있으면 리턴합니다. 이렇게 하면 스왑퍼가 끝날 때까지 게임 상태 진행이 지연됩니다.

```csharp
    public void DoWork () {
        if (busyDuration > 0f)
        {
            tileSwapper.Update();
            busyDuration -= Time.deltaTime;
            if (busyDuration > 0f)
            {
                return;
            }
        }

        …
    }
```

`DoMove`를 수정하여 더 이상 타일 위치를 직접 조정하지 않고 대신 스왑퍼를 활성화하고 busy duration을 설정합니다. 이동이 성공하지 못하면 핑퐁하게 합니다.

```csharp
    void DoMove (Move move)
    {
        bool success = game.TryMove(move);
        Tile a = tiles[move.From], b = tiles[move.To];
        busyDuration = tileSwapper.Swap(a, b, !success);
        if (success)
        {
            tiles[move.From] = b;
            tiles[move.To] = a;
        }
    }
```

`TileSwapper`로 돌아가서 기능을 구현합니다. 타일, 초기 위치, 기본값 −1의 진행도, 핑퐁 여부를 저장하는 필드를 추가합니다. `Swap`에서 이 모든 필드를 설정하고 진행도를 0으로 설정하며, 핑퐁의 경우 duration의 2배를 반환합니다.

```csharp
    Tile tileA, tileB;

    Vector3 positionA, positionB;

    float progress = -1f;

    bool pingPong;

    public float Swap (Tile a, Tile b, bool pingPong)
    {
        tileA = a;
        tileB = b;
        positionA = a.transform.localPosition;
        positionB = b.transform.localPosition;
        this.pingPong = pingPong;
        progress = 0f;
        return pingPong ? 2f * duration : duration;
    }
```

진행도 −1은 스왑퍼가 비활성 상태임을 나타냅니다. `Update`는 먼저 비활성인지 확인하고 비활성이면 리턴합니다. 그렇지 않으면 진행도를 증가시킵니다. duration을 초과하면 두 가지 중 하나를 수행합니다. 핑퐁의 경우 진행도에서 duration을 빼고 핑퐁을 비활성화하며 타일을 교환합니다. 그렇지 않으면 진행도를 −1로 설정하고 타일을 최종 위치로 설정한 뒤 리턴합니다.

```csharp
    public void Update ()
    {
        if (progress < 0f)
        {
            return;
        }

        progress += Time.deltaTime;
        if (progress >= duration)
        {
            if (pingPong)
            {
                progress -= duration;
                pingPong = false;
                (tileA, tileB) = (tileB, tileA);
            }
            else
            {
                progress = -1f;
                tileA.transform.localPosition = positionB;
                tileB.transform.localPosition = positionA;
                return;
            }
        }
    }
```

그 다음 두 타일 위치를 선형 보간합니다. 보간자에 π를 곱한 사인값에 최대 깊이 오프셋을 곱하여 Z 차원에서 타일을 변위시킵니다. 첫 번째 타일은 음수, 두 번째 타일은 양수로 합니다.

```csharp
        float t = progress / duration;
        float z = Mathf.Sin(Mathf.PI * t) * maxDepthOffset;
        Vector3 p = Vector3.Lerp(positionA, positionB, t);
        p.z = -z;
        tileA.transform.localPosition = p;
        p = Vector3.Lerp(positionA, positionB, 1f - t);
        p.z = z;
        tileB.transform.localPosition = p;
```

타일 교환이 이제 애니메이션되며 게임은 애니메이션이 끝날 때까지 이동 결과를 보여주기 전에 기다립니다. 성공적인 이동이었으면 타일이 바뀌고 게임이 진행되며, 그렇지 않으면 타일이 다시 돌아갑니다. 스킨이 바쁜 동안에는 새 교환을 시작할 수 없습니다.

---

### 타일 사라짐

사라지는 타일에도 애니메이션을 추가합니다. 각 타일마다 다른 애니메이션을 지원하기 위해 `Tile`에 설정 가능한 사라짐 duration을 추가합니다. 기본값은 0.25초입니다. 사라짐 진행도를 추적하며 스폰 시 −1로 초기화합니다. 타일 스케일을 0으로 줄이는 방식이므로 스폰 시 1로 되돌립니다. 전환 중이 아닐 때 불필요한 업데이트를 피하기 위해 컴포넌트를 비활성화합니다.

```csharp
    [SerializeField, Range(0f, 1f)]
    float disappearDuration = 0.25f;

    PrefabInstancePool<Tile> pool;

    float disappearProgress;

    public Tile Spawn (Vector3 position)
    {
        Tile instance = pool.GetInstance(this);
        instance.pool = pool;
        instance.transform.localPosition = position;
        instance.transform.localScale = Vector3.one;
        instance.disappearProgress = -1f;
        instance.enabled = false;
        return instance;
    }
```

진행도를 0으로 설정하고 자신을 활성화하며 duration을 반환하는 퍼블릭 `Disappear` 메서드를 추가합니다. 진행 중이면 스케일을 0으로 줄이고 끝나면 자신을 디스폰하는 `Update` 메서드를 추가합니다.

```csharp
    public float Disappear ()
    {
        disappearProgress = 0f;
        enabled = true;
        return disappearDuration;
    }

    void Update ()
    {
        if (disappearProgress >= 0f)
        {
            disappearProgress += Time.deltaTime;
            if (disappearProgress >= disappearDuration)
            {
                Despawn();
                return;
            }
            transform.localScale =
                Vector3.one * (1f - disappearProgress / disappearDuration);
        }
    }
```

사라지는 타일을 지원하기 위해 `Match3Skin`에서 `ProcessMatches`의 `Despawn` 대신 `Disappear`를 호출하기만 하면 됩니다. 모든 애니메이션이 끝날 때까지 기다리기 위해 busy duration을 각 타일의 사라짐 duration과 자신의 최대값으로 설정합니다.

```csharp
    void ProcessMatches ()
    {
        game.ProcessMatches();

        for (int i = 0; i < game.ClearedTileCoordinates.Count; i++)
        {
            int2 c = game.ClearedTileCoordinates[i];
            busyDuration = Mathf.Max(tiles[c].Disappear(), busyDuration);
            tiles[c] = null;
        }
    }
```

---

### 타일 떨어짐

세 번째이자 마지막으로 애니메이션할 전환은 떨어지는 타일입니다. 각 타일이 자체 낙하 애니메이션을 처리하도록 합니다. from/to Y 위치, 낙하 duration, 진행도를 추적해야 합니다. 이 필드들을 내부 `FallingState` 구조체로 그룹화합니다. 다시 한번 진행도 −1은 전환이 비활성임을 나타냅니다.

```csharp
    [System.Serializable]
    struct FallingState
    {
        public float fromY, toY, duration, progress;
    }

    FallingState falling;

    public Tile Spawn (Vector3 position)
    {
        …
        instance.disappearProgress = -1f;
        instance.falling.progress = -1f;
        instance.enabled = false;
        return instance;
    }
```

목적지 Y 위치와 속도를 파라미터로 받는 퍼블릭 `Fall` 메서드를 추가하고 낙하 상태를 설정합니다. 낙하 duration을 반환합니다.

```csharp
    public float Fall (float toY, float speed)
    {
        falling.fromY = transform.localPosition.y;
        falling.toY = toY;
        falling.duration = (falling.fromY - toY) / speed;
        falling.progress = 0f;
        enabled = true;
        return falling.duration;
    }
```

업데이트 시 떨어지고 있는지도 확인합니다. 그렇다면 선형 보간을 수행하고 끝나면 중지합니다.

```csharp
    void Update ()
    {
        …

        if (falling.progress >= 0f)
        {
            Vector3 position = transform.localPosition;
            falling.progress += Time.deltaTime;
            if (falling.progress >= falling.duration)
            {
                falling.progress = -1f;
                position.y = falling.toY;
                enabled = disappearProgress >= 0f;
            }
            else
            {
                position.y = Mathf.Lerp(
                    falling.fromY, falling.toY, falling.progress / falling.duration
                );
            }
            transform.localPosition = position;
        }
    }
```

낙하가 끝났을 때 이미 사라지고 있지 않은 경우에만 컴포넌트를 비활성화할 수 있습니다.

모든 타일은 같은 속도로 떨어지므로 `Match3Skin`에 속도 설정 필드를 추가합니다. 기본값은 8입니다. 또한 새로 생성된 떨어지는 타일에 추가할 추가 오프셋 설정 필드도 추가합니다. 기본값은 2입니다. 0으로 설정하면 새 타일이 그리드 바로 위에 스폰됩니다. 값을 늘리면 더 높이 스폰되어 시야 밖에서 위쪽으로부터 떨어져 들어올 수 있습니다.

```csharp
    [SerializeField, Range(0.1f, 20f)]
    float dropSpeed = 8f;

    [SerializeField, Range(0f, 10f)]
    float newDropOffset = 2f;
```

`DropTiles`에서, 이미 그리드에 있던 타일의 위치를 더 이상 직접 조정하지 않습니다. 그리드 위에서 떨어지는 타일에 수직 드롭 오프셋을 추가합니다. 그런 다음 타일을 떨어뜨리고 busy duration을 가장 긴 낙하 duration으로 설정합니다.

```csharp
            if (drop.fromY < tiles.SizeY)
            {
                tile = tiles[drop.coordinates.x, drop.fromY];
            }
            else
            {
                tile = SpawnTile(
                    game[drop.coordinates], drop.coordinates.x, drop.fromY + newDropOffset
                );
            }
            tiles[drop.coordinates] = tile;
            busyDuration = Mathf.Max(
                tile.Fall(drop.coordinates.y + tileOffset.y, dropSpeed), busyDuration
            );
```

---

## 점수

이제 무슨 일이 일어나는지 볼 수 있으니 점수를 추가하겠습니다.

### 총점

`Match3Game`에 총점에 대한 퍼블릭 프로퍼티를 추가합니다. 비공개로 설정됩니다. 새 게임 시작 시 0으로 설정하고 `ProcessMatches`에서 각 매치 길이만큼 증가시킵니다.

```csharp
    public int TotalScore
    { get; private set; }

    public bool HasMatches => matches.Count > 0;

    public void StartNewGame ()
    {
        TotalScore = 0;
        …
    }

    …

    public void ProcessMatches ()
    {
        ClearedTileCoordinates.Clear();

        for (int m = 0; m < matches.Count; m++)
        {
            …
            TotalScore += match.length;
        }

        …
    }
```

`Match3Skin`에 총점 텍스트 표시 설정 필드를 추가하고, 게임 시작 시 텍스트를 0으로 설정하며, 매치 처리 후 업데이트합니다.

```csharp
    [SerializeField]
    TextMeshPro totalScoreText;

    …

    public void StartNewGame () {
        busyDuration = 0f;
        totalScoreText.SetText("0");
        …
    }

    …

    void ProcessMatches ()
    {
        …

        totalScoreText.SetText("{0}", game.TotalScore);
    }
```

텍스트 게임 오브젝트를 만들어 총점 텍스트로 사용합니다. 게임 왼쪽, X 위치 −6에 배치합니다. 너비 3, 높이 2를 줍니다. Auto Size 옵션을 활성화하고 최소 6, 최대 12로 설정합니다. 이렇게 하면 매우 높은 점수에서도 텍스트가 지정 영역에 맞게 줄어듭니다.

---

### 플로팅 점수

개별 매치에 대해 그리드 앞에 일시적으로 뜨는 점수도 추가합니다. 게임에서 스킨으로 이 점수를 전달하기 위해 위치와 값에 대한 퍼블릭 필드가 있는 직렬화 가능 `SingleScore` 구조체를 도입합니다. 위치는 `float2`인데, 매치 길이가 4인 경우 두 타일 사이에 위치할 수 있기 때문입니다.

```csharp
using Unity.Mathematics;

[System.Serializable]
public struct SingleScore
{
    public float2 position;

    public int value;
}
```

다른 공개 리스트처럼 `Match3Game`에 점수 리스트 프로퍼티를 추가합니다. `ProcessMatches` 시작에서 클리어하고 각 매치의 중간에 점수를 추가합니다.

```csharp
    public List<SingleScore> Scores
    { get; private set; }

    …

    public void StartNewGame ()
    {
        TotalScore = 0;
        if (grid.IsUndefined)
        {
            …
            Scores = new();
        }
        FillGrid();
    }

    …

    public void ProcessMatches ()
    {
        ClearedTileCoordinates.Clear();
        Scores.Clear();

        for (int m = 0; m < matches.Count; m++)
        {
            …

            var score = new SingleScore
            {
                position = match.coordinates + (float2)step * (match.length - 1) * 0.5f,
                value = match.length
            };
            Scores.Add(score);
            TotalScore += score.value;
        }

        …
    }
```

빈 게임 오브젝트에 텍스트 자식을 추가하여 프리팹을 만듭니다. 자식에 Z 오프셋 −0.25를 줘서 타일 앞에 뜨게 합니다. 너비와 높이 1, 폰트 크기 8로 설정합니다. 보기 쉽게 하기 위해 검은 색상에 밝은 노란 아웃라인을 가진 조정된 머티리얼을 사용합니다.

`FloatingScore` 컴포넌트 타입을 만들고 프리팹 루트에 할당합니다. 텍스트 설정 필드를 추가하고 연결합니다. 위치와 값을 파라미터로 받아 풀에서 인스턴스를 표시하는 퍼블릭 `Show` 메서드를 추가합니다.

```csharp
using TMPro;
using UnityEngine;

public class FloatingScore : MonoBehaviour
{
    [SerializeField]
    TextMeshPro displayText;

    PrefabInstancePool<FloatingScore> pool;

    public void Show (Vector3 position, int value)
    {
        FloatingScore instance = pool.GetInstance(this);
        instance.pool = pool;
        instance.displayText.SetText("{0}", value);
        instance.transform.localPosition = position;
    }
}
```

텍스트가 자동으로 사라지게 하고 보이는 동안 위쪽으로 올라가게 합니다. 표시 시간과 상승 속도 설정 필드를 추가합니다. 기본값은 0.5와 2입니다. 표시될 때 0으로 설정되는 나이를 부여합니다. `Update` 메서드에서 나이를 증가시키고, 시간이 다 되면 재활용하고, 그렇지 않으면 위쪽으로 이동합니다.

```csharp
    [SerializeField, Range(0.1f, 1f)]
    float displayDuration = 0.5f;

    [SerializeField, Range(0f, 4f)]
    float riseSpeed = 2f;

    float age;

    PrefabInstancePool<FloatingScore> pool;

    public void Show (int score, Vector3 position)
    {
        …
        instance.age = 0f;
    }

    void Update ()
    {
        age += Time.deltaTime;
        if (age >= displayDuration)
        {
            pool.Recycle(this);
        }
        else
        {
            Vector3 p = transform.localPosition;
            p.y += riseSpeed * Time.deltaTime;
            transform.localPosition = p;
        }
    }
```

`Match3Skin`에 플로팅 점수 프리팹 설정 필드를 추가하고 연결합니다. 그런 다음 `ProcessMatches` 끝에서 모든 점수를 표시합니다.

```csharp
    [SerializeField]
    FloatingScore floatingScorePrefab;

    …

    void ProcessMatches ()
    {
        …

        for (int i = 0; i < game.Scores.Count; i++)
        {
            SingleScore score = game.Scores[i];
            floatingScorePrefab.Show(
                new Vector3(
                    score.position.x + tileOffset.x,
                    score.position.y + tileOffset.y
                ),
                score.value
            );
        }
    }
```

플로팅 점수가 겹칠 수 있습니다. 사라질 때까지 기다리지 않기 때문이고, 수평과 수직 매치가 겹칠 수도 있기 때문입니다. 겹치는 점수의 Z-파이팅을 피하기 위해 연속 점수에 미세한 깊이 오프셋을 추가하여 각각 0.001유닛씩 더 가깝게 당깁니다. −0.02를 지나면 오프셋을 0으로 리셋합니다.

```csharp
    float floatingScoreZ;

    …

    void ProcessMatches ()
    {
        …

        for (int i = 0; i < game.Scores.Count; i++)
        {
            SingleScore score = game.Scores[i];
            floatingScorePrefab.Show(
                new Vector3(
                    score.position.x + tileOffset.x,
                    score.position.y + tileOffset.y,
                    floatingScoreZ
                ),
                score.value
            );
            floatingScoreZ = floatingScoreZ <= -0.02f ? 0f : floatingScoreZ - 0.001f;
        }
    }
```

---

### 점수 배율

매치-3 게임은 보통 콤보, 연쇄, 또는 동시 매치에 보상을 줍니다. 우리도 `Match3Game`에 점수 배율을 추가하여 이를 구현합니다. 이동을 시도할 때마다 1로 설정하고, 각 개별 점수 값에 배율을 곱한 뒤 증가시킵니다. 따라서 한 번의 이동에서 매치가 많을수록 보상이 커집니다.

```csharp
    int scoreMultiplier;

    …

    public bool TryMove (Move move)
    {
        scoreMultiplier = 1;
        …
    }

    public void ProcessMatches ()
    {
        …

        for (int m = 0; m < matches.Count; m++)
        {
            …

            var score = new SingleScore
            {
                position = match.coordinates + (float2)step * (match.length - 1) * 0.5f,
                value = match.length * scoreMultiplier++
            };
            …
        }

        …
    }
```

---

## 가능한 수 찾기

게임의 목표는 매치를 만드는 것이므로, 더 이상 매치가 불가능할 때 게임이 끝나야 합니다. 이 게임 오버 상태를 감지하려면 그리드에서 가능한 수를 탐색해야 합니다.

### 수 탐색

게임이 주어지면 이동을 반환하는 퍼블릭 정적 `FindMove` 메서드를 `Move`에 추가합니다. 게임의 모든 행을 순회하며 현재 타일에 대한 유효한 이동을 찾습니다. 유효한 이동을 찾지 못하면 기본 이동(유효하지 않음)을 반환합니다. 현재 타일만 가져오는 이중 루프부터 시작합니다. 루프에 `int2` 좌표 변수를 사용하고, 코드를 짧게 유지하기 위해 게임 크기를 로컬 `s` 변수에 저장합니다.

```csharp
    public static Move FindMove (Match3Game game)
    {
        int2 s = game.Size;
        for (int2 c = 0; c.y < s.y; c.y++)
        {
            for (c.x = 0; c.x < s.x; c.x++)
            {
                TileState t = game[c];
            }
        }

        return default;
    }
```

잠재적 매치를 찾는 다양한 방법이 있지만, 현재 타일을 이동할 타일로 취급합니다. 첫 번째 고려하는 경우는 타일을 왼쪽으로 이동하여 같은 행에서 매치를 만들 수 있는 경우입니다. 개략도에서 X는 타일, ?는 확인할 잠재적 매치입니다:

```
?? X
```

왼쪽으로 2칸과 3칸의 타일이 모두 존재하고 현재 타일과 일치하면 적절한 이동을 반환합니다.

```csharp
                if (c.x >= 3 && game[c.x - 2, c.y] == t && game[c.x - 3, c.y] == t)
                {
                    return new Move(c, MoveDirection.Left);
                }
```

반대 방향에서도 같은 것을 시도합니다:

```
?? X ??
```

```csharp
                if (c.x + 3 < s.x && game[c.x + 2, c.y] == t && game[c.x + 3, c.y] == t)
                {
                    return new Move(c, MoveDirection.Right);
                }
```

그 후 수직으로도 같은 검사를 수행합니다:

```csharp
                if (c.y >= 3 && game[c.x, c.y - 2] == t && game[c.x, c.y - 3] == t)
                {
                    return new Move(c, MoveDirection.Down);
                }

                if (c.y + 3 < s.y && game[c.x, c.y + 2] == t && game[c.x, c.y + 3] == t)
                {
                    return new Move(c, MoveDirection.Up);
                }
```

아직 진행 중이면 대각선을 살펴봐야 합니다. 먼저 한 칸 아래 왼쪽에 있는 타일과 관련된 모든 경우를 고려합니다. 그 타일과 그 왼쪽 타일이 아래로 이동한 후 매치를 형성하는 경우부터 시작합니다:

```
   X
??
```

한 칸 아래로 갈 수 있는지 확인합니다. 그런 다음 아래-왼쪽 타일이 존재하고 일치하는지 확인합니다. 그 왼쪽 타일도 일치하면 이동을 반환합니다.

```csharp
                if (c.y > 1)
                {
                    if (c.x > 1 && game[c.x - 1, c.y - 1] == t)
                    {
                        if (c.x >= 2 && game[c.x - 2, c.y - 1] == t)
                        {
                            return new Move(c, MoveDirection.Down);
                        }
                    }
                }
```

실패하면 아래-오른쪽 타일도 일치하는 경우 여전히 아래로 이동이 가능합니다:

```csharp
                        if (
                            c.x >= 2 && game[c.x - 2, c.y - 1] == t ||
                            c.x + 1 < s.x && game[c.x + 1, c.y - 1] == t
                        )
                        {
                            return new Move(c, MoveDirection.Down);
                        }
```

아래로 이동이 안 되면 대신 왼쪽으로 이동하여 아래-왼쪽 타일과 매치할 수 있을지 확인합니다. X와 Y를 바꾼 같은 검사입니다.

```csharp
                        if (
                            c.y >= 2 && game[c.x - 1, c.y - 2] == t ||
                            c.y + 1 < s.y && game[c.x - 1, c.y + 1] == t
                        )
                        {
                            return new Move(c, MoveDirection.Left);
                        }
```

이 시점에서 아래-왼쪽 타일과의 매치는 불가능합니다. 아래-오른쪽 타일을 확인합니다. 같은 코드이지만 왼쪽 대신 오른쪽입니다. 하나의 경우를 건너뛸 수 있습니다.

```csharp
                    if (c.x + 1 < s.x && game[c.x + 1, c.y - 1] == t)
                    {
                        if (c.x + 2 < s.x && game[c.x + 2, c.y - 1] == t)
                        {
                            return new Move(c, MoveDirection.Down);
                        }
                        if (
                            c.y >= 2 && game[c.x + 1, c.y - 2] == t ||
                            c.y + 1 < s.y && game[c.x + 1, c.y + 1] == t
                        )
                        {
                            return new Move(c, MoveDirection.Right);
                        }
                    }
```

아직 매치가 없으면 아래 대신 위 두 대각선 타일에 대해 같은 검사를 수행해야 합니다. `if (c.y > 1) { … }` 코드 블록 전체를 복제하고 수직 방향을 뒤집고 두 경우를 제거합니다.

```csharp
                if (c.y + 1 < s.y)
                {
                    if (c.x > 1 && game[c.x - 1, c.y + 1] == t)
                    {
                        if (
                            c.x >= 2 && game[c.x - 2, c.y + 1] == t ||
                            c.x + 1 < s.x && game[c.x + 1, c.y + 1] == t
                        )
                        {
                            return new Move(c, MoveDirection.Up);
                        }
                        if (c.y + 2 < s.y && game[c.x - 1, c.y + 2] == t)
                        {
                            return new Move(c, MoveDirection.Left);
                        }
                    }

                    if (c.x + 1 < s.x && game[c.x + 1, c.y + 1] == t)
                    {
                        if (c.x + 2 < s.x && game[c.x + 2, c.y + 1] == t)
                        {
                            return new Move(c, MoveDirection.Up);
                        }
                        if (c.y + 2 < s.y && game[c.x + 1, c.y + 2] == t)
                        {
                            return new Move(c, MoveDirection.Right);
                        }
                    }
                }
```

---

### 더 이상의 수 없음

이동을 찾을 수 있으므로, 가능한 이동에 대한 공개 프로퍼티를 `Match3Game`에 추가합니다. 새 게임을 시작할 때 그리드를 채운 후, 그리고 타일을 떨어뜨린 후 매치가 없으면 새로운 가능한 이동을 찾습니다.

```csharp
    public Move PossibleMove
    { get; private set; }

    …

    public void StartNewGame ()
    {
        …
        FillGrid();
        PossibleMove = Move.FindMove(this);
    }

    …

    public void DropTiles ()
    {
        …

        NeedsFilling = false;
        if (!FindMatches())
        {
            PossibleMove = Move.FindMove(this);
        }
    }
```

이제 `Match3Skin`이 게임 오버 상태를 감지할 수 있습니다. 새 게임 시작 시 비활성화되는 설정 가능한 게임 오버 텍스트를 추가합니다. `IsPlaying` 프로퍼티를 변경하여 바쁘거나 유효한 가능한 이동이 있는지를 반환합니다. 그런 다음 `DoWork` 끝에서, 매치도 없고, 채움도 필요 없고, 플레이 중이 아니면 게임 오버 텍스트를 활성화합니다.

```csharp
    [SerializeField]
    TextMeshPro gameOverText, totalScoreText;

    …

    public bool IsPlaying => IsBusy || game.PossibleMove.IsValid;

    public void StartNewGame () {
        busyDuration = 0f;
        totalScoreText.SetText("0");
        gameOverText.gameObject.SetActive(false);

        …
    }

    …

    public void DoWork () {
        …

        if (game.HasMatches)
        {
            ProcessMatches();
        }
        else if (game.NeedsFilling)
        {
            DropTiles();
        }
        else if (!IsPlaying)
        {
            gameOverText.gameObject.SetActive(true);
        }
    }
```

게임 오버 텍스트 게임 오브젝트를 만들고 X 위치를 6으로 설정합니다. 너비 3, 높이 5, 폰트 크기 8을 줍니다. 더 이상 유효한 수가 없음을 나타내고, 스페이스를 눌러 새 게임을 시작할 수 있다는 작은 힌트도 표시합니다:

```
NO MORE MOVES

<size=50%>PRESS SPACE
```

게임은 이제 유효한 이동이 더 이상 없을 때 자동으로 멈춥니다. 드물지만, 게임 시작 시부터 이 상태일 수 있습니다. 이를 피하기 위해 `Match3Game`의 `StartNewGame`에서 유효한 가능한 이동이 있을 때까지 그리드를 계속 채웁니다.

```csharp
        do
        {
            FillGrid();
            PossibleMove = Move.FindMove(this);
        }
        while (!PossibleMove.IsValid);
```

---

## 자동 플레이

이 튜토리얼을 마무리하며 게임이 스스로 플레이하는 기능을 추가합니다. 가능한 이동을 사용하는 퍼블릭 `DoAutomaticMove` 메서드를 `Match3Skin`에 추가하기만 하면 됩니다.

```csharp
    public void DoAutomaticMove () => DoMove(game.PossibleMove);
```

`Game`에 자동 플레이를 토글하는 설정 옵션을 추가합니다. 활성화되면 `HandleInput`이 플레이어 입력을 확인하는 대신 자동 이동을 수행합니다.

```csharp
    [SerializeField]
    bool automaticPlay;

    …

    void HandleInput ()
    {
        if (automaticPlay)
        {
            match3.DoAutomaticMove();
        }
        else if (!isDragging && Input.GetMouseButtonDown(0))
        {
            dragStart = Input.mousePosition;
            isDragging = true;
        }
        …
    }
```

게임이 이제 스스로 플레이할 수 있습니다. AI는 발견된 첫 번째 이동을 단순히 수행하는 매우 기본적인 것입니다. 스킨이 바쁘지 않은 순간 가능한 한 빨리 이동을 발행합니다.

---

## 마무리

매치-3 게임 프로토타입이 완성되었습니다. 더 많은 비주얼과 애니메이션 추가, 고유한 동작을 가진 특수 타일 추가, 완전히 새로운 게임 메커니즘 도입, AI 개선, 멀티플레이어 지원 등으로 개선할 수 있습니다. 이러한 변경이 이 튜토리얼을 기반으로 한 프로젝트의 토대가 될 것입니다.

다음 튜토리얼은 **Bouncy Ball Shooter**입니다.