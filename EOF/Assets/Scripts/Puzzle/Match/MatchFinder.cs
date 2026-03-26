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

    /// <summary>
    /// 플레이 영역 전체에서 3개 이상 연속된 동일 타입 블록을 탐색
    /// 가로/세로 독립 탐색으로 L자, T자 매치도 자연스럽게 잡힘
    /// </summary>
    public List<SMatch> FindAllMatches()
    {
        List<SMatch> matches = new List<SMatch>();

        FindHorizontalMatches(matches);
        FindVerticalMatches(matches);

        return matches;
    }
    
    /// <summary>
    /// 가로 매치 탐색 — 각 행을 왼쪽부터 스캔하며 연속 동일 타입 카운트
    /// 연속 끝까지 건너뛰어 중복 탐색 방지
    /// </summary>
    private void FindHorizontalMatches(List<SMatch> matches)
    {
        // _bufferRows-1까지 버퍼 행이니 _bufferRows행부터 시작하면 플레이 보드 행
        for (int y = _bufferRows; y < _rows; y++)
        {
            int x = 0;
            while (x < _columns)
            {
                var block = _data.GetBlock(new int2(x, y));
                
                // 빈 칸이거나 비활성이면 스킵
                if (block == null || !block.gameObject.activeSelf
                    || block.Type == EBlockType.None)
                {
                    x++;
                    continue;
                }
                
                // 오른쪽으로 동일 타입 연속 카운트
                int count = 1;
                while (x + count < _columns)
                {
                    var next = _data.GetBlock(new int2(x + count, y));
                    if (next == null || !next.gameObject.activeSelf
                        || next.Type != block.Type) break;
                    count++;
                }
                
                // 3개 이상이면 매치 등록
                if (count >= 3)
                    matches.Add(new SMatch(x, y, count, true, block.Type));
                
                // 연속 끝까지 건너뛰기
                x += count;
            }
        }
    }

    /// <summary>
    /// 세로 매치 탐색 — 각 열을 위에서부터 스캔하며 연속 동일 타입 카운트
    /// </summary>
    private void FindVerticalMatches(List<SMatch> matches)
    {
        for (int x = 0; x < _columns; x++)
        {
            int y = _bufferRows;
            while (y < _rows)
            {
                var block = _data.GetBlock(new int2(x, y));

                if (block == null || !block.gameObject.activeSelf
                    || block.Type == EBlockType.None)
                {
                    y++;
                    continue;
                }

                // 아래로 동일 타입 연속 카운트
                int count = 1;
                while (y + count < _rows)
                {
                    var next = _data.GetBlock(new int2(x, y + count));
                    if (next == null || !next.gameObject.activeSelf
                        || next.Type != block.Type) break;
                    count++;
                }

                if (count >= 3)
                    matches.Add(new SMatch(x, y, count, false, block.Type));

                y += count;
            }
        }
    }
}