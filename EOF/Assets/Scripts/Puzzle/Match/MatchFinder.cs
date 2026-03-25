using System.Collections.Generic;
using Unity.Mathematics;

// 요약 : 보드에서 3매치를 탐색하는 역할
// 작성자 : 이성규
public class MatchFinder
{
    private readonly IBoardData _data;
    private readonly int _columns;
    private readonly int _rows;
    private readonly int _bufferRows;

    public MatchFinder(IBoardData data, int columns, int rows, int bufferRows)
    {
        _data = data;
        _columns = columns;
        _rows = rows;
        _bufferRows = bufferRows;
    }

    // 보이는 영역 전체에서 매치를 탐색하여 반환
    public List<SMatch> FindAllMatches()
    {
        // TODO: 내일 구현
        return new List<SMatch>();
    }
}