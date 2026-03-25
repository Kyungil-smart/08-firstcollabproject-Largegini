using Unity.Mathematics;
using DG.Tweening;
using System;

// 요약 : 블록 스왑 실행을 담당
// 작성자 : 이성규
public class BoardSwapper
{
    private SGrid2D<Block> _blocks;
    private readonly BoardLayout _layout;
    private const float SWAP_DURATION = 0.5f;
    private readonly Action _onSwapStart;
    private readonly Action _onSwapEnd;

    private int completed;

    public BoardSwapper(SGrid2D<Block> blocks, BoardLayout layout, 
        Action onSwapStart, Action onSwapEnd)
    {
        _blocks = blocks;
        _layout = layout;
        _onSwapStart = onSwapStart;
        _onSwapEnd = onSwapEnd;
    }
    
    void OnSwapStart()
    {
        completed = 0;
        _onSwapStart?.Invoke();
    }
    void CheckComplete()
    {
        completed++;
        if (completed >= 2) _onSwapEnd?.Invoke();
    }
    
    // 블록 교체(스왑)
    public void Swap(int2 posA, int2 posB)
    {
        OnSwapStart();
        
        // 그리드 데이터 교환
        _blocks.Swap(posA, posB);

        Block blockA = _blocks[posA];
        Block blockB = _blocks[posB];

        // 논리 좌표 교환
        blockA.SetPosition(posA);
        blockB.SetPosition(posB);

        // UI 좌표 교환
        // blockA.Rect.anchoredPosition = _layout.GetPosition(posA);
        // blockB.Rect.anchoredPosition = _layout.GetPosition(posB);
        
        // 연출 중 상태로 전환
        blockA.SetStatus(EBlockStatus.Moving);
        blockB.SetStatus(EBlockStatus.Moving);
        
        // DoTween 슬라이딩 연출
        blockA.Rect.DOAnchorPos(_layout.GetPosition(posA), SWAP_DURATION)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => { blockA.SetStatus(EBlockStatus.None); CheckComplete(); });

        blockB.Rect.DOAnchorPos(_layout.GetPosition(posB), SWAP_DURATION)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => { blockB.SetStatus(EBlockStatus.None); CheckComplete(); });
    }
    
    // 드래그 드롭용 스왑 — 드래그한 블록은 이미 위치에 있으므로 상대 블록만 이동
    public void SwapFromDrag(int2 draggedPos, int2 targetPos)
    {
        OnSwapStart();
        
        // 그리드 데이터 교환
        _blocks.Swap(draggedPos, targetPos);

        // Swap 후 참조 — draggedPos에는 원래 target이, targetPos에는 원래 dragged가 있음
        Block nowAtDragged = _blocks[draggedPos]; // 원래 target
        Block nowAtTarget = _blocks[targetPos];   // 원래 dragged
        
        // 논리 좌표 교환 — Swap 후 dragged는 targetPos에, target은 draggedPos에 있음
        // 논리 좌표를 현재 그리드 위치와 일치시킴
        nowAtDragged.SetPosition(draggedPos);
        nowAtTarget.SetPosition(targetPos);
        
        // 드래그한 블록도 목표 위치로 스냅 연출
        // 원래 dragged -> targetPos로 스냅 연출
        nowAtTarget.SetStatus(EBlockStatus.Moving);
        nowAtTarget.Rect.DOAnchorPos(_layout.GetPosition(targetPos), SWAP_DURATION)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => { nowAtTarget.SetStatus(EBlockStatus.None); CheckComplete(); });
         
        // 상대 블록은 드래그한 블록이 있던 자리로 슬라이딩
        // 원래 target -> draggedPos로 슬라이딩 연출
        nowAtDragged.SetStatus(EBlockStatus.Moving);
        nowAtDragged.Rect.DOAnchorPos(_layout.GetPosition(draggedPos), SWAP_DURATION)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => { nowAtDragged.SetStatus(EBlockStatus.None); CheckComplete(); });
    }
}