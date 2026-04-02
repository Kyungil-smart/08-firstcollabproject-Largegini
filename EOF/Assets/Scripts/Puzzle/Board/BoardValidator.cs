using Unity.Mathematics;

// 요약 : 보드 상태 검증 (데드락 판정 등)
// 작성자 : 이성규
public class BoardValidator
{
    private readonly IBoardData _data;
    private readonly MatchFinder _matchFinder;
    private readonly int _columns;
    private readonly int _rows;
    private readonly int _bufferRows;

    public BoardValidator(IBoardData data, MatchFinder matchFinder,
        int columns, int rows, int bufferRows)
    {
        _data = data;
        _matchFinder = matchFinder;
        _columns = columns;
        _rows = rows;
        _bufferRows = bufferRows;
    }

    /// <summary>
    /// 플레이 영역에서 어떤 인접 스왑도 매치를 만들 수 없으면 데드락
    /// 오른쪽/아래쪽만 체크 — 왼쪽/위쪽은 이전 칸에서 이미 체크됨
    /// </summary>
    public bool IsDeadlocked()
    {
        for (int y = _bufferRows; y < _rows; y++)
        {
            for (int x = 0; x < _columns; x++)
            {
                var pos = new int2(x, y);
                var block = _data.GetBlock(pos);
                if (block == null || !block.gameObject.activeSelf) continue;

                if (x + 1 < _columns && WouldMatchAfterSwap(pos, new int2(x + 1, y)))
                    return false;

                if (y + 1 < _rows && WouldMatchAfterSwap(pos, new int2(x, y + 1)))
                    return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 두 위치를 임시 스왑 후 매치 발생 여부 체크, 즉시 되돌림
    /// </summary>
    private bool WouldMatchAfterSwap(int2 posA, int2 posB)
    {
        _data.SwapBlocks(posA, posB);
        bool hasMatch = _matchFinder.FindAllMatches().Count > 0;
        _data.SwapBlocks(posA, posB);
        return hasMatch;
    }
}