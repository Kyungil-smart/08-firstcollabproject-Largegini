using Unity.Mathematics;
using UnityEngine;

// 요약 : 블록 입력 검증 및 스왑 요청 인터페이스
// 작성자: 이성규
public interface IBoardInteraction
{
    bool CanInteract(int2 pos);
    void OnSwipeBlock(int2 pos, Vector2Int dir);
    bool IsValidSwapTarget(int2 from, int2 to);     // 인접 1칸 + 플레이 영역 검증
    void OnDragSwapBlock(int2 from, int2 to);       // 드래그 스왑 요청
}