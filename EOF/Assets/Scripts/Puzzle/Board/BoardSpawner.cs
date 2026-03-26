using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

// 요약 : 블록 생성 및 리필을 담당
// 작성자 : 이성규
public class BoardSpawner
{
    private readonly IBoard _board;
    private readonly BoardLayout _layout;
    private readonly Block _blockPrefab;
    private readonly RectTransform _boardPanel;
    private readonly BlockDataSO[] _blockDatas;
    private readonly int _columns;
    private readonly int _rows;

    public BoardSpawner(IBoard board, BoardLayout layout,
        Block blockPrefab, RectTransform boardPanel, BlockDataSO[] blockDatas,
        int columns, int rows)
    {
        _board = board;
        _layout = layout;
        _blockPrefab = blockPrefab;
        _boardPanel = boardPanel;
        _blockDatas = blockDatas;
        _columns = columns;
        _rows = rows;
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

        // IBoardData로 그리드에 등록
        _board.SetBlock(pos, newBlock);
    }

    // 전체 블록 스폰
    public void SpawnAll(int columns, int rows)
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
                SpawnBlock(new int2(x, y));
        }
    }

    // 기존 오브젝트를 유지하되 데이터만 교체
    public void ResetAll(int columns, int rows)
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                var pos = new int2(x, y);
                var block = _board.GetBlock(pos);
                if (block == null) continue;
                int randomIndex = Random.Range(0, _blockDatas.Length);
                block.Init(pos, _blockDatas[randomIndex], _board);
            }
        }
    }
    
    /// <summary>
    /// 빈 칸에 비활성 블록을 찾아 새 데이터로 재활용 배치
    /// Destroy/Instantiate 없이 기존 오브젝트를 재사용하는 구조
    /// </summary>
    public void RefillBlock(int2 pos, Block recycled)
    {
        // 새 랜덤 데이터 부여 — Init이 데이터 + 상태 + 비주얼 + SetActive(true) 전부 복원
        int randomIndex = Random.Range(0, _blockDatas.Length);
        recycled.Init(pos, _blockDatas[randomIndex], _board);
        
        // 지정된 그리드 좌표의 UI 위치에 배치
        recycled.Rect.anchoredPosition = _layout.GetPosition(pos);
        
        // 그리드 데이터에 등록
        _board.SetBlock(pos, recycled);
    }
}