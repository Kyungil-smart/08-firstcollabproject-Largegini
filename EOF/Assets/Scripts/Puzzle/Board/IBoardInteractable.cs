using Unity.Mathematics;
using UnityEngine;

public interface IBoardInteractable
{
    bool CanInteract(int2 pos);
    void OnSwipeBlock(int2 pos, Vector2Int dir);
}