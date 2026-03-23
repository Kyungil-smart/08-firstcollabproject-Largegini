using Unity.Mathematics;

// 요약 : 퍼즐 보드의 각 칸에 정보를 저장하고 좌표로 읽고 쓸 수 있게 해주는 격자 데이터 구조체
// 작성자 : 이성규
public struct SGrid2D<T>
{
    // 내부 데이터
    T[] cells;
    int2 size;

    // 생성자
    public SGrid2D(int2 size)
    {
        this.size = size;
        cells = new T[size.x * size.y];
    }

    // 외부 접근용 프로퍼티
    public int2 Size => size;
    public int SizeX => size.x;
    public int SizeY => size.y;

    // 상태 확인
    public bool IsUndefined => cells == null || cells.Length == 0;
    public bool AreValidCoordinates(int2 c) =>
        0 <= c.x && c.x < size.x && 0 <= c.y && c.y < size.y;

    // 인덱서: 2차원 좌표를 1차원 배열 인덱스(y * width + x)로 변환하여 접근
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

    // 두 좌표의 데이터를 서로 교체
    public void Swap(int2 a, int2 b) => (this[a], this[b]) = (this[b], this[a]);
}