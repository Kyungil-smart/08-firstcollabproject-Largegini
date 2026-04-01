using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// 인스펙터 애니메이션 설정 조절
[Serializable]
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
public class BoardManager : MonoBehaviour, IBoard, ITutorialBoardControl
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
    
    [Header("Tutorial")]
    [SerializeField, Tooltip("튜토리얼 하이라이트 프리팹 (Image, RaycastTarget Off)")]
    private Image _tutorialHighlightPrefab;
    
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
    
    // 하위 시스템
    private SGrid2D<Block> _blocks;
    private BoardLayout _layout;
    private BoardSwapper _swapper;
    private BoardSpawner _spawner;
    private MatchFinder _matchFinder;
    private BoardProcessor _processor;
    private BoardValidator _validator;
    private BoardTutorialHandler _tutorial;
    
    private bool _isProcessing;
    
    // BoardTestHelper에서 접근용
    public BoardTutorialHandler TutorialHandler => _tutorial;
    public bool IsProcessing { 
        get => _isProcessing; 
        private set => _isProcessing = value; 
    }

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

        _spawner = new BoardSpawner(this, _layout, _blockPrefab, _startRect, _boardPanel, _blockDatas,
            _columns, _rows, _bufferRows);
        _swapper = new BoardSwapper(this, _layout, _animSettings,
            () => _isProcessing = true, OnSwapComplete);
        _processor = new BoardProcessor(this, _layout, _matchFinder, _spawner,
            _animSettings, _columns, _rows, _bufferRows);
        _validator = new BoardValidator(this, _matchFinder, _columns, _rows, _bufferRows);
        _tutorial = new BoardTutorialHandler(this, _layout, this,
            _boardPanel, _tutorialHighlightPrefab, _startRect.sizeDelta);
        
        _spawner.SpawnAll();
    }

    public void ResetBoard()
    {
        _spawner.ResetAll();
    }

    // ====== 상호작용 검증 ======

    // 블록을 "잡을 수 있는가" 검증 (OnPointerDown 시점)
    // InteractionFilter는 여기서만 적용 — 잡기 전용
    public bool CanInteract(int2 pos)
    {
        if (_isProcessing) return false;
        if (_tutorial.InputLocked) return false;
        if (pos.y < _bufferRows) return false;
        
        Block targetBlock = _blocks[pos];
        if (targetBlock == null || targetBlock.Status != EBlockStatus.None) return false;
        
        // InteractionFilter: 잡을 수 있는 블록 제한 (튜토리얼 4단계)
        if (_tutorial.InteractionFilter != null && !_tutorial.InteractionFilter(pos)) return false;
        
        return true;
    }

    private bool IsValidPlayArea(int2 pos)
    {
        return pos.x >= 0 && pos.x < _columns && pos.y >= _bufferRows && pos.y < _rows;
    }

    // 스왑 "대상"이 유효한가 검증 (OnPointerUp 시점)
    // InteractionFilter를 타지 않음 — 타겟 블록은 잡는 게 아니라 놓는 대상
    // SwapFilter로 스왑 방향을 제한
    public bool IsValidSwapTarget(int2 from, int2 to)
    {
        if (!IsValidPlayArea(to)) return false;
        
        int2 diff = to - from;
        if (math.abs(diff.x) + math.abs(diff.y) != 1) return false;
        
        // 타겟 블록 기본 검증 (InteractionFilter 제외)
        if (_isProcessing) return false;
        if (_tutorial.InputLocked) return false;
        Block targetBlock = _blocks[to];
        if (targetBlock == null || targetBlock.Status != EBlockStatus.None) return false;
        
        // SwapFilter: 스왑 방향 제한
        return _tutorial.SwapFilter == null || _tutorial.SwapFilter(from, to);
    }

    // ====== 스와이프 ======

    public void OnSwipeBlock(int2 pos, Vector2Int direction)
    {
        int2 targetPos = new int2(pos.x + direction.x, pos.y - direction.y);
        
        if (!IsValidPlayArea(targetPos)) return;
        if (!CanInteract(targetPos)) return;
        
        _swapper.SwipeSwap(pos, targetPos);
        _tutorial.NotifySwapped(pos, targetPos);
    }

    // ====== 드래그 앤 드롭 ======

    public int2 GetGridIndex(Vector2 anchoredPosition)
    {
        return _layout.GetGridIndex(anchoredPosition);
    }

    public void OnDragSwapBlock(int2 from, int2 to)
    {
        _swapper.SwapFromDrag(from, to);
        _tutorial.NotifySwapped(from, to);
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
            // _chainInterceptor가 null이면 기존 로직 그대로 진행.
            if (_tutorial.ChainInterceptor != null)
            {
                // 인터셉터가 설정되면 matches와 Proceed 지역 함수를 외부에 전달하고 return.
                // `_isProcessing`이 true인 채로 멈추므로 유저 입력은 자동 차단.
                // 외부(튜토리얼 컨트롤러)가 `proceed()`를 호출하면 연쇄 처리 재개.
                void Proceed() => StartCoroutine(_processor.ProcessMatches(matches, HandlePuzzleComplete));
                _tutorial.ChainInterceptor.Invoke(matches, Proceed);
                return;
            }
            StartCoroutine(_processor.ProcessMatches(matches, HandlePuzzleComplete));
        }
        else
        {
            _isProcessing = false;
            if (CheckDeadlock()) OnDeadlockDetected();
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
    
    // ====== ITutorialBoardControl 구현 (BoardTutorialHandler에 위임) ======
    
    public void SetInteractionFilter(Func<int2, bool> filter) => _tutorial.InteractionFilter = filter;
    public void SetSwapFilter(Func<int2, int2, bool> filter) => _tutorial.SwapFilter = filter;
    public void SetInputLocked(bool locked) => _tutorial.InputLocked = locked;
    public void SetChainInterceptor(Action<List<SMatch>, Action> interceptor) => _tutorial.ChainInterceptor = interceptor;
    public void LoadPresetBoard(BlockDataSO[,] preset) => _tutorial.LoadPresetBoard(preset, _columns, _rows);
    public void SetBlockHighlights(IEnumerable<int2> positions, Color? color = null) => _tutorial.SetBlockHighlights(positions, color);
    public void ClearAllHighlights() => _tutorial.ClearAllHighlights();
    public void NotifyBoardTapped() => _tutorial.NotifyTapped();
    
    // 이벤트 위임
    public event Action OnBoardTapped
    {
        add => _tutorial.OnBoardTapped += value;
        remove => _tutorial.OnBoardTapped -= value;
    }
    public event Action<int2, int2> OnBlockSwapped
    {
        add => _tutorial.OnBlockSwapped += value;
        remove => _tutorial.OnBlockSwapped -= value;
    }
    
    // ====== 테스트용 (BoardTestHelper에서 호출) ======
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