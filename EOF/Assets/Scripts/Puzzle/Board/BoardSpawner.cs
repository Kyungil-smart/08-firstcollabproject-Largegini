using System.Collections.Generic;
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
    private readonly RectTransform _startRect;
    private readonly RectTransform _boardPanel;
    private readonly BlockDataSO[] _blockDatas;
    private readonly int _columns;
    private readonly int _rows;
    private readonly int _bufferRows;

    public BoardSpawner(IBoard board, BoardLayout layout,
        Block blockPrefab, RectTransform startRect, RectTransform boardPanel, BlockDataSO[] blockDatas,
        int columns, int rows, int bufferRows)
    {
        _board = board;
        _layout = layout;
        _blockPrefab = blockPrefab;
        _startRect = startRect;
        _boardPanel = boardPanel;
        _blockDatas = blockDatas;
        _columns = columns;
        _rows = rows;
        _bufferRows = bufferRows;
    }

    // 단일 블록 설정 및 반환
    public void CreateBlock(int2 pos, BlockDataSO data)
    {
        // 프리팹 생성 및 부모 설정
        Block newBlock = Object.Instantiate(_blockPrefab, _boardPanel);
        // 인스펙터 상에서 쉬운 디버깅을 위해 블록에 위치값으로 이름 설정
        newBlock.name = $"Block_{pos.y}_{pos.x}";
        
        // 블록 데이터 할당 및 초기화
        newBlock.Init(pos, data, _board);
        
        // UI 좌표 설정 - Init 이후 Rect 캐싱됨
        newBlock.Rect.anchoredPosition = _layout.GetPosition(pos);
        
        // 블록 사이즈 설정 - 첫번째 그리드 사이즈와 동일하게
        newBlock.Rect.sizeDelta = _startRect.sizeDelta;
        
        // 스크린 -> 로컬 좌표 변환
        newBlock.DragHandler.SetBoardPanel(_boardPanel);
        
        // IBoardData로 그리드에 등록
        _board.SetBlock(pos, newBlock);
    }
    
    // 단일 블록 명시적 생성
    public void SpawnBlock(int2 pos)
    {
        var data = _blockDatas[Random.Range(0, _blockDatas.Length)];
        CreateBlock(pos, data);
    }

    // 전체 블록 스폰 (초기 생성)
    public void SpawnAll()
    {
        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _columns; x++)
            {
                var pos = new int2(x, y);
                var data = pos.y < _bufferRows
                    ? _blockDatas[Random.Range(0, _blockDatas.Length)]
                    : GetNonMatchingData(pos);
                CreateBlock(pos, data);
            }
        }
    }

    // 기존 오브젝트를 유지하되 데이터만 교체 (리셋)
    public void ResetAll()
    {
        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _columns; x++)
            {
                var pos = new int2(x, y);
                var block = _board.GetBlock(pos);
                if (block == null) continue;
                InitBlockSafe(pos, block);
            }
        }
    }
    
    /// <summary>
    /// 3매치 방지 데이터 선택 후 블록 초기화
    /// 블록 데이터 리셋 사용
    /// </summary>
    private void InitBlockSafe(int2 pos, Block block)
    {
        BlockDataSO data;
        
        // 버퍼 영역은 3매치 방지 배치 불필요 (버퍼 내 매치 허용)
        if (pos.y < _bufferRows)
            data = _blockDatas[Random.Range(0, _blockDatas.Length)];
        else
            data = GetNonMatchingData(pos);
        
        block.Init(pos, data, _board);
        block.Rect.anchoredPosition = _layout.GetPosition(pos);
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
    
    // 매칭 가능 여부 체크
    private bool WouldCauseMatch(int2 pos, EBlockType type)
    {
        // 가로 체크: 왼쪽 2칸이 같은 타입인지
        if (pos.x >= 2)
        {
            var b1 = _board.GetBlock(new int2(pos.x - 1, pos.y));
            var b2 = _board.GetBlock(new int2(pos.x - 2, pos.y));
            if (b1 != null && b2 != null && b1.Type == type && b2.Type == type)
                return true;
        }

        // 세로 체크: 위쪽 2칸이 같은 타입인지
        if (pos.y >= 2)
        {
            var b1 = _board.GetBlock(new int2(pos.x, pos.y - 1));
            var b2 = _board.GetBlock(new int2(pos.x, pos.y - 2));
            if (b1 != null && b2 != null && b1.Type == type && b2.Type == type)
                return true;
        }

        return false;
    }
    
    // 겹치지 않는 블럭 데이터 선택 보정
    private BlockDataSO GetNonMatchingData(int2 pos)
    {
        // 매치를 유발하는 타입을 제외한 후보 리스트 생성
        List<BlockDataSO> candidates = new List<BlockDataSO>();
        foreach (var data in _blockDatas)
        {
            if (!WouldCauseMatch(pos, data.Type))
                candidates.Add(data);
        }

        // 후보가 있으면 그 중 랜덤, 없으면 전체에서 랜덤 (안전장치)
        if (candidates.Count > 0)
            return candidates[Random.Range(0, candidates.Count)];
        return _blockDatas[Random.Range(0, _blockDatas.Length)];
    }
}