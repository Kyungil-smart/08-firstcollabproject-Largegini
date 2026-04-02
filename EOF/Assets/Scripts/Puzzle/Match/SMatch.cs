using Unity.Mathematics;

// 요약: 매치 정보를 담기 위한 기본 단위가 될 매칭 구조체
// 작성자: 이성규
[System.Serializable]
public struct SMatch
{
    public int2 coordinates;  // 매치 시작 좌표
    public int length;        // 매치된 블록 수
    public bool isHorizontal; // 가로/세로 방향
    public EBlockType type;   // 매치된 블록 타입

    public SMatch(int x, int y, int length, bool isHorizontal, EBlockType type)
    {
        coordinates = new int2(x, y);
        this.length = length;
        this.isHorizontal = isHorizontal;
        this.type = type;
    }
}