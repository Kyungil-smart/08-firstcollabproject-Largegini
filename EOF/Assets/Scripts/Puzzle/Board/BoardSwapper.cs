using Unity.Mathematics;
using DG.Tweening;
using System;

// 요약 : 블록 스왑 실행을 담당
// 작성자 : 이성규
public class BoardSwapper
{
    private readonly IBoardData _data;
    private readonly BoardLayout _layout;
    private readonly BoardAnimationSettings _anim;
    private readonly Action _onSwapStart;
    private readonly Action _onSwapEnd;
    
    private int _completed;
    
    public BoardSwapper(IBoardData data, BoardLayout layout, 
        BoardAnimationSettings anim, Action onSwapStart, Action onSwapEnd)
    {
        _data = data;
        _layout = layout;
        _anim = anim;
        _onSwapStart = onSwapStart;
        _onSwapEnd = onSwapEnd;
    }
    
    // 블록 교체 (스와이프용)
    public void SwipeSwap(int2 posA, int2 posB)
    {
        ExecuteSwap(posA, posB);
    }
    
    // 드래그 드롭용 스왑
    public void SwapFromDrag(int2 draggedPos, int2 targetPos)
    {
        ExecuteSwap(draggedPos, targetPos);
    }

    // ====== 내부 공통 로직 ======

    private void ExecuteSwap(int2 posA, int2 posB)
    {
        _completed = 0;
        _onSwapStart?.Invoke();
        
        // 그리드 데이터 교환
        _data.SwapBlocks(posA, posB);
        
        // Swap 후 참조
        Block blockA = _data.GetBlock(posA);
        Block blockB = _data.GetBlock(posB);
        
        // 논리 좌표 갱신
        blockA.SetPosition(posA);
        blockB.SetPosition(posB);
        
        // 양쪽 블록 슬라이딩 연출
        AnimateBlock(blockA, posA);
        AnimateBlock(blockB, posB);
    }

    private void AnimateBlock(Block block, int2 targetPos)
    {
        block.SetStatus(EBlockStatus.Moving);
        block.Rect.DOAnchorPos(_layout.GetPosition(targetPos), _anim.swapDuration)
            .SetEase(_anim.swapEase)
            .OnComplete(() =>
            {
                block.SetStatus(EBlockStatus.None);
                _completed++;
                if (_completed >= 2) _onSwapEnd?.Invoke();
            });
    }
}