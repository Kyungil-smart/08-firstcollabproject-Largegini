using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

// 요약 : 블록 생성 및 리필을 담당
// 작성자 : 이성규
public class BoardSpawner
{
    private SGrid2D<Block> _blocks;
    private readonly BoardLayout _layout;
    private readonly Block _blockPrefab;
    private readonly RectTransform _boardPanel;
    private readonly BlockDataSO[] _blockDatas;
    private readonly IBoardInteractable _board;

    public BoardSpawner(SGrid2D<Block> blocks, BoardLayout layout,
        Block blockPrefab, RectTransform boardPanel, BlockDataSO[] blockDatas,
        IBoardInteractable board)
    {
        _blocks = blocks;
        _layout = layout;
        _blockPrefab = blockPrefab;
        _boardPanel = boardPanel;
        _blockDatas = blockDatas;
        _board = board;
    }

    // 단일 블록 생성
    public void SpawnBlock(int2 pos)
    {
        // 프리팹 생성 및 부모 설정
        Block newBlock = Object.Instantiate(_blockPrefab, _boardPanel);
        // 인스펙터 상에서 쉬운 디버깅을 위해 블록에 위치값으로 이름 설정
        newBlock.name = $"Block_{pos.y}_{pos.x}";

        // 랜덤 데이터 할당 및 초기화
        int randomIndex = Random.Range(0, _blockDatas.Length);
        newBlock.Init(pos, _blockDatas[randomIndex], _board);

        // UI 좌표 설정 - Init 이후 Rect 캐싱됨
        newBlock.Rect.anchoredPosition = _layout.GetPosition(pos);
        // 스크린 -> 로컬 좌표 변환
        newBlock.DragHandler.SetBoardPanel(_boardPanel);

        // 그리드 데이터에 등록
        _blocks[pos] = newBlock;
    }

    // 전체 블록 스폰
    public void SpawnAll(int columns, int rows)
    {
        for (int y = 0; y < rows; y++)
        for (int x = 0; x < columns; x++)
            SpawnBlock(new int2(x, y));
    }

    // 기존 오브젝트를 유지하되 데이터만 교체
    public void ResetAll(int columns, int rows)
    {
        for (int y = 0; y < rows; y++)
        for (int x = 0; x < columns; x++)
        {
            var pos = new int2(x, y);
            var block = _blocks[pos];
            int randomIndex = Random.Range(0, _blockDatas.Length);
            block.Init(pos, _blockDatas[randomIndex], _board);
        }
    }
}