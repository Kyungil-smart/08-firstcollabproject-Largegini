using System;
using Unity.Mathematics;
using UnityEngine;

// 요약: 튜토리얼 스테이지용 사전 정의 보드 배치 데이터
// Inspector에서 그리드 크기, 블록 배치, 조작 대상 좌표, 하이라이트 좌표 설정
// 작성자: 이성규
[CreateAssetMenu(menuName = "Scriptable Objects/TutorialBoardPreset")]
public class TutorialBoardPreset : ScriptableObject
{
    [Header("그리드 크기 (BoardManager와 일치해야 함)")]
    [SerializeField] private int _columns = 5;
    [SerializeField] private int _rows = 12;

    [Header("블록 배치 (좌상단부터 행 우선, 크기 = columns * rows)")]
    [SerializeField] private BlockDataSO[] _layout;

    [Header("조작 대상")]
    [SerializeField] private Vector2Int _dragSource;
    [SerializeField] private Vector2Int _dragDirection;

    [Header("하이라이트 대상")]
    [SerializeField] private Vector2Int[] _highlightPositions;

    // 드래그 시작 좌표
    public int2 DragSource => new int2(_dragSource.x, _dragSource.y);
    
    // 드래그 도착 좌표 (소스 + 방향)
    public int2 DragTarget => new int2(
        _dragSource.x + _dragDirection.x,
        _dragSource.y + _dragDirection.y);

    // 1D 배열 -> 2D BlockDataSO 배열 변환 (LoadPresetBoard에 전달)
    // null인 칸은 기존 블록 유지
    public BlockDataSO[,] ToGrid()
    {
        var grid = new BlockDataSO[_columns, _rows];
        for (int y = 0; y < _rows; y++)
            for (int x = 0; x < _columns; x++)
            {
                int i = y * _columns + x;
                if (i < _layout.Length)
                    grid[x, y] = _layout[i];
            }
        return grid;
    }

    // 하이라이트 좌표 배열 반환
    public int2[] GetHighlightPositions()
    {
        if (_highlightPositions == null) return Array.Empty<int2>();
        var result = new int2[_highlightPositions.Length];
        for (int i = 0; i < _highlightPositions.Length; i++)
            result[i] = new int2(_highlightPositions[i].x, _highlightPositions[i].y);
        return result;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_layout != null && _layout.Length != _columns * _rows)
            Debug.LogWarning($"[TutorialBoardPreset] layout 크기({_layout.Length}) != {_columns}x{_rows}({_columns * _rows})", this);
    }
#endif
}