using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// 인스펙터 애니메이션 설정 조절
[System.Serializable]
public class BoardAnimationSettings
{
    [Header("Swap")]
    public float swapDuration = 0.2f;
    public Ease swapEase = Ease.OutQuad;
    
    [Header("Drop")]
    public float dropDurationPerCell = 0.08f;
    public Ease dropEase = Ease.InQuad;
    
    [Header("Match")]
    public float clearDelay = 0.3f;
    public float refillDelay = 0.2f;
}

// 요약 : 퍼즐 보드 전체를 관리하는 컨트롤러 스크립트
// 작성자 : 이성규
public class BoardManager : MonoBehaviour, IBoard
{
    [Header("Board Settings")]
    [SerializeField, Tooltip("행/높이")] private int _rows = 12;
    [SerializeField, Tooltip("버퍼 높이")] private int _bufferRows = 6;
    [SerializeField, Tooltip("열/길이")] private int _columns = 5;
    [SerializeField, Tooltip("그리드 시작 위치")] private RectTransform _startRect;
    [SerializeField, Tooltip("그리드 한칸당 크기")] private float _cellSize;
    [SerializeField, Tooltip("그리드간 간격")] private float _spacing;
    
    [Header("Animation")]
    [SerializeField] private BoardAnimationSettings _animSettings;
    
    [Header("References")]
    [SerializeField] private Block _blockPrefab;
    [SerializeField] private RectTransform _boardPanel;
    [SerializeField] private BlockDataSO[] _blockDatas;
    
    [Header("Events")]
    [SerializeField] private UnityEvent<PuzzleResult> _onPuzzleComplete;
    [SerializeField] private UnityEvent _onDeadlock;
    // 스왑이 끝났을 때
    [SerializeField] private UnityEvent _onSwapFinished;
    
    [SerializeField] private GraphicRaycaster _raycaster;
    
    // 외부에서 블럭 상호작용 가능 여부 조절용 함수
    public void SetInteractable(bool value)
    {
        _raycaster.enabled = value;
    }

    public UnityEvent<PuzzleResult> OnPuzzleComplete => _onPuzzleComplete;
    public UnityEvent OnDeadlock => _onDeadlock;
    // 스왑이 끝났을 때
    public UnityEvent OnSwapFinished => _onSwapFinished;
    
    private SGrid2D<Block> _blocks;
    private BoardLayout _layout;
    private BoardSwapper _swapper;
    private BoardSpawner _spawner;
    private MatchFinder _matchFinder;
    private BoardProcessor _processor;
    private BoardValidator _validator;
    
    private bool _isProcessing;
    
    // ====== IBoardData ======
    
    public Block GetBlock(int2 pos) => _blocks[pos];
    public void SetBlock(int2 pos, Block block) => _blocks[pos] = block;
    public void SwapBlocks(int2 posA, int2 posB) => _blocks.Swap(posA, posB);
    
    // ====== 초기화 ======
    
    private void Awake()
    {
        _layout = new BoardLayout(
            _startRect.anchoredPosition,
            _cellSize,
            _spacing,
            _bufferRows
        );
        
        _blocks = new SGrid2D<Block>(new int2(_columns, _rows));
        _matchFinder = new MatchFinder(this, _columns, _rows, _bufferRows);

        _spawner = new BoardSpawner(this, _layout, _blockPrefab, _startRect, _boardPanel, _blockDatas, _columns, _rows,
            _bufferRows);
        _swapper = new BoardSwapper(this, _layout, _animSettings,
            () => _isProcessing = true, OnSwapComplete);
        _processor = new BoardProcessor(this, _layout, _matchFinder, _spawner,
            _animSettings, _columns, _rows, _bufferRows);
        _validator = new BoardValidator(this, _matchFinder, _columns, _rows, _bufferRows);
        
        _spawner.SpawnAll();
    }

    public void ResetBoard()
    {
        _spawner.ResetAll();
    }

    // ====== 상호작용 검증 ======

    public bool CanInteract(int2 pos)
    {
        if (_isProcessing) return false;
        if (pos.y < _bufferRows) return false;
        
        Block targetBlock = _blocks[pos];
        if (targetBlock == null || targetBlock.Status != EBlockStatus.None) return false;
        
        return true;
    }

    private bool IsValidPlayArea(int2 pos)
    {
        return pos.x >= 0 && pos.x < _columns && pos.y >= _bufferRows && pos.y < _rows;
    }

    public bool IsValidSwapTarget(int2 from, int2 to)
    {
        if (!IsValidPlayArea(to)) return false;
        
        int2 diff = to - from;
        if (math.abs(diff.x) + math.abs(diff.y) != 1) return false;
        
        if (!CanInteract(to)) return false;
        
        return true;
    }

    // ====== 스와이프 ======

    public void OnSwipeBlock(int2 pos, Vector2Int direction)
    {
        int2 targetPos = new int2(pos.x + direction.x, pos.y - direction.y);
        
        if (!IsValidPlayArea(targetPos)) return;
        if (!CanInteract(targetPos)) return;
        
        _swapper.SwipeSwap(pos, targetPos);
    }

    // ====== 드래그 앤 드롭 ======

    public int2 GetGridIndex(Vector2 anchoredPosition)
    {
        return _layout.GetGridIndex(anchoredPosition);
    }

    public void OnDragSwapBlock(int2 from, int2 to)
    {
        _swapper.SwapFromDrag(from, to);
    }

    // ====== 레이아웃 조회 ======

    public float GetStride()
    {
        return _layout.Stride;
    }

    public float GetCellSize()
    {
        return _cellSize;
    }

    public Vector2 GetBoardMin()
    {
        Vector2 min = _layout.GetPosition(0, _rows - 1);
        float half = _cellSize * 0.5f;
        return new Vector2(min.x - half, min.y - half);
    }
    
    public Vector2 GetBoardMax()
    {
        Vector2 max = _layout.GetPosition(_columns - 1, _bufferRows);
        float half = _cellSize * 0.5f;
        return new Vector2(max.x + half, max.y + half);
    }
    
    // ====== 이벤트 처리 ======
    
    private void OnSwapComplete()
    {
        _onSwapFinished?.Invoke();
        
        var matches = _matchFinder.FindAllMatches();
        if (matches.Count > 0)
        {
            StartCoroutine(_processor.ProcessMatches(matches, HandlePuzzleComplete));
        }
        else
        {
            _isProcessing = false;
            
            if (CheckDeadlock())
                OnDeadlockDetected();
        }
    }

    private void HandlePuzzleComplete(PuzzleResult result)
    {
        _isProcessing = false;
        _onPuzzleComplete?.Invoke(result);
        Debug.Log($"콤보: {result.comboCount}, 타입별: {string.Join(", ", result.matchedCounts)}");
    }
    
    // ====== 데드락 ======
    // 현재 자유 스왑 기획상 자연 발동은 없음
    // 적 스킬 등으로 의도적으로 데드락 패턴을 생성하는 용도로 활용 가능
    
    public bool CheckDeadlock() => _validator.IsDeadlocked();
    
    public void OnDeadlockDetected()
    {
        _onDeadlock?.Invoke();
    }

#if UNITY_EDITOR
    // 테스트용: 강제로 데드락 보드 생성 (3타입 대각선 순환 패턴)
    public void CreateDeadlockBoard()
    {
        for (int y = _bufferRows; y < _rows; y++)
        {
            for (int x = 0; x < _columns; x++)
            {
                var pos = new int2(x, y);
                var block = _blocks[pos];
                if (block == null) continue;

                int typeIndex = (x + y) % 3;
                block.Init(pos, _blockDatas[typeIndex], this);
                block.Rect.anchoredPosition = _layout.GetPosition(pos);
            }
        }
    }
#endif
}