using Unity.Mathematics;
using UnityEngine;

// 요약: 보드 레이아웃 정보 조회 인터페이스
// 작성자: 이성규
public interface IBoardQuery
{
    int2 GetGridIndex(Vector2 anchoredPosition);    // UI 좌표 -> 그리드 인덱스
    float GetStride();
    float GetCellSize();
    Vector2 GetBoardMin();
    Vector2 GetBoardMax();
}