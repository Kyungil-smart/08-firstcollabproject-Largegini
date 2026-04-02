using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

// 요약: 튜토리얼 제어 로직을 BoardManager에서 분리한 하위 시스템
// 기존 Spawner/Swapper/Processor/Validator와 동일한 패턴의 순수 C# 클래스
// 작성자: 이성규
public class BoardTutorialHandler
{
    private readonly IBoardData _data;
    private readonly BoardLayout _layout;
    private readonly IBoard _board;
    private readonly RectTransform _boardPanel;
    private readonly Image _highlightPrefab;
    private readonly Vector2 _blockSize;
    
    // 하이라이트 오브젝트 풀
    private readonly List<GameObject> _highlightPool = new List<GameObject>();
    
    // 입력 필터링
    public bool InputLocked;
    public Func<int2, bool> InteractionFilter;  // 블록 잡기 제한
    public Func<int2, int2, bool> SwapFilter;   // 스왑 방향 제한
    
    // 매칭 파이프라인 인터셉트
    public Action<List<SMatch>, Action> ChainInterceptor;
    
    // 이벤트
    public event Action OnBoardTapped;
    public event Action<int2, int2> OnBlockSwapped;

    public BoardTutorialHandler(IBoardData data, BoardLayout layout, IBoard board,
        RectTransform boardPanel, Image highlightPrefab, Vector2 blockSize)
    {
        _data = data;
        _layout = layout;
        _board = board;
        _boardPanel = boardPanel;
        _highlightPrefab = highlightPrefab;
        _blockSize = blockSize;
    }

    // ====== 프리셋 보드 배치 ======
    // null인 칸은 기존 블록 유지
    public void LoadPresetBoard(BlockDataSO[,] preset, int columns, int rows)
    {
        int cols = preset.GetLength(0);
        int rowCount = preset.GetLength(1);
        for (int y = 0; y < rowCount && y < rows; y++)
        for (int x = 0; x < cols && x < columns; x++)
        {
            var pos = new int2(x, y);
            var block = _data.GetBlock(pos);
            var blockData = preset[x, y];
            if (block != null && blockData != null)
            {
                block.Init(pos, blockData, _board);
                block.Rect.anchoredPosition = _layout.GetPosition(pos);
            }
        }
    }

    // ====== 하이라이트 ======
    // 보드 패널 하위에 별도 오브젝트로 생성 (블록 프리팹 하위 아님)
    // 블록이 드래그돼도 하이라이트는 그리드 좌표에 고정
    // 기존 드래그 하이라이트(_highlight)와 완전히 독립
    public void SetBlockHighlights(IEnumerable<int2> positions, Color? color = null)
    {
        ClearAllHighlights();

        foreach (var pos in positions)
        {
            var hlObj = GetOrCreateHighlight();
            var rt = hlObj.GetComponent<RectTransform>();
            rt.anchoredPosition = _layout.GetPosition(pos);
            rt.sizeDelta = _blockSize;

            if (color.HasValue)
                hlObj.GetComponent<Image>().color = color.Value;

            hlObj.SetActive(true);
        }
    }

    public void ClearAllHighlights()
    {
        foreach (var hl in _highlightPool)
            hl.SetActive(false);
    }

    // 풀에서 비활성 오브젝트를 꺼내거나, 없으면 새로 생성
    private GameObject GetOrCreateHighlight()
    {
        foreach (var hl in _highlightPool)
        {
            if (!hl.activeSelf)
                return hl;
        }
        
        GameObject newObj;
        if (_highlightPrefab != null)
        {
            newObj = UnityEngine.Object.Instantiate(_highlightPrefab.gameObject, _boardPanel);
        }
        else
        {
            newObj = new GameObject("TutorialHighlight", typeof(RectTransform), typeof(Image));
            newObj.transform.SetParent(_boardPanel, false);
            var img = newObj.GetComponent<Image>();
            img.color = new Color(1f, 1f, 0f, 0.4f);
        }
        
        newObj.GetComponent<Image>().raycastTarget = false;
        newObj.SetActive(false);
        _highlightPool.Add(newObj);
        return newObj;
    }

    // ====== 이벤트 발사 ======
    public void NotifyTapped() => OnBoardTapped?.Invoke();
    public void NotifySwapped(int2 a, int2 b) => OnBlockSwapped?.Invoke(a, b);
}