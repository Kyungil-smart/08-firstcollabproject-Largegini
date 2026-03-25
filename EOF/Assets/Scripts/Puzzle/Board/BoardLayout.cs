using Unity.Mathematics;
using UnityEngine;

// 요약 : 보드가 생성될때 초기 레이아웃 정보를 계산하기 위한 스크립트
// 작성자 : 이성규
public class BoardLayout
{
    private readonly Vector2 _startPos;
    private readonly float _stride;
    private readonly int _bufferRows;
    
    public float Stride => _stride;

    public BoardLayout(Vector2 startPos, float cellSize, float spacing, int bufferRows)
    {
        _startPos = startPos;
        _bufferRows = bufferRows;
        _stride = cellSize + spacing;
    }

    public Vector2 GetPosition(int x, int y)
        => new(_startPos.x + x * _stride, _startPos.y - (y - _bufferRows) * _stride);
    public Vector2 GetPosition(int2 pos)
        => new(_startPos.x + pos.x * _stride, _startPos.y - (pos.y - _bufferRows) * _stride);
    
    // UI 좌표 → 그리드 인덱스
    public int2 GetGridIndex(Vector2 position)
    {
        int x = Mathf.RoundToInt((position.x - _startPos.x) / _stride);
        int y = _bufferRows - Mathf.RoundToInt((position.y - _startPos.y) / _stride);
        return new int2(x, y);
    }
}
