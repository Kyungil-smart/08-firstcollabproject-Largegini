using Unity.Mathematics;
using UnityEngine;

public interface IBoardInteractable
{
    bool CanInteract(int2 pos);
    void OnSwipeBlock(int2 pos, Vector2Int dir);
    int2 GetGridIndex(Vector2 anchoredPosition);       // UI 좌표 -> 그리드 인덱스
    bool IsValidSwapTarget(int2 from, int2 to);         // 인접 1칸 + 플레이 영역 검증
    void OnDragSwapBlock(int2 from, int2 to);           // 드래그 스왑 요청
    float GetStride();
    float GetCellSize();
    Vector2 GetBoardMin();
    Vector2 GetBoardMax();
}