using Unity.Mathematics;

// 요약 : 블록 스왑 실행을 담당
// 작성자 : 이성규
public class BoardSwapper
{
    private SGrid2D<Block> _blocks;
    private readonly BoardLayout _layout;

    public BoardSwapper(SGrid2D<Block> blocks, BoardLayout layout)
    {
        _blocks = blocks;
        _layout = layout;
    }
    
    // 블록 교체(스왑)
    public void Swap(int2 posA, int2 posB)
    {
        // 그리드 데이터 교환
        _blocks.Swap(posA, posB);

        Block blockA = _blocks[posA];
        Block blockB = _blocks[posB];

        // 논리 좌표 교환
        blockA.SetPosition(posA);
        blockB.SetPosition(posB);

        // UI 좌표 교환
        blockA.Rect.anchoredPosition = _layout.GetPosition(posA);
        blockB.Rect.anchoredPosition = _layout.GetPosition(posB);
    }
}