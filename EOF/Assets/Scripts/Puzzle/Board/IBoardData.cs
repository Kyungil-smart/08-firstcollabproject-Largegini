using Unity.Mathematics;

// 요약 : 보드 그리드 데이터 접근 인터페이스
// 작성자 : 이성규
public interface IBoardData
{
    Block GetBlock(int2 pos);
    void SetBlock(int2 pos, Block block);
    void SwapBlocks(int2 posA, int2 posB);
}
