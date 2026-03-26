using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

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
    [SerializeField] private Block _blockPrefab;        // 스폰할 블록 프리팹
    [SerializeField] private RectTransform _boardPanel; // 블록들이 생성될 부모 캔버스 패널
    [SerializeField] private BlockDataSO[] _blockDatas; // 스폰 시 랜덤으로 부여할 SO 데이터 풀
    
    [Header("Events")]
    [SerializeField] private UnityEvent<PuzzleResult> _onPuzzleComplete; // 퍼즐 동작 완료시 결과를 반환해주는 유니티 이벤트

    private SGrid2D<Block> _blocks;
    private BoardLayout _layout;
    private BoardSwapper _swapper;
    private BoardSpawner _spawner;
    private MatchFinder _matchFinder;
    private BoardProcessor _processor;
    
    private bool _isProcessing;
    
    public Block GetBlock(int2 pos) => _blocks[pos];
    public void SetBlock(int2 pos, Block block) => _blocks[pos] = block;
    public void SwapBlocks(int2 posA, int2 posB) => _blocks.Swap(posA, posB);
    
    private void Awake()
    {
        // 레이아웃은 보드 초기화와 무관하게 고정값이므로 Awake에서 한 번만 생성
        _layout = new BoardLayout(
            _startRect.anchoredPosition,
            _cellSize,
            _spacing,
            _bufferRows
        );

        // 블록으로 그리드 구조 데이터 생성
        _blocks = new SGrid2D<Block>(new int2(_columns, _rows));
        // 매칭 탐색기 생성
        _matchFinder = new MatchFinder(this, _columns, _rows, _bufferRows);
        
        // 블록 Spawner 생성
        _spawner = new BoardSpawner(this, _layout, _blockPrefab, _boardPanel, _blockDatas, _columns, _rows);
        // 블록 Swapper 생성
        _swapper = new BoardSwapper(this, _layout, _animSettings, 
            () => _isProcessing = true, OnSwapComplete);
        // 매칭 연쇄 루프 프로세서 생성
        _processor = new BoardProcessor(this, _layout, _matchFinder, _spawner, 
            _animSettings, _columns, _rows, _bufferRows);
        
        // 초기 블록 스폰
        _spawner.SpawnAll(_columns, _rows);
    }

    // 기존 오브젝트를 유지하되 데이터만 교체
    public void ResetBoard()
    {
        _spawner.ResetAll(_columns, _rows);
    }

    // 블록 상호작용 가능 여부 체크
    public bool CanInteract(int2 pos)
    {
        // 스왑, 드랍 등의 프로세스 진행중엔 상호작용 금지
        if (_isProcessing) return false;
        
        // 버퍼 구역이면 터치 금지
        if (pos.y < _bufferRows) return false;
        
        // 해당 블록이 연출 중이거나 비활성이면 금지
        Block targetBlock = _blocks[pos];
        if (targetBlock == null || targetBlock.Status != EBlockStatus.None) return false;
        
        return true;
    }

    // ====== 스와이프 ======
    
    // 드래그 방향이 결정되면 핸들러가 이 함수를 요청
    public void OnSwipeBlock(int2 pos, Vector2Int direction)
    {
        int2 targetPos = new int2(pos.x + direction.x, pos.y - direction.y); // UI Y축 반전 보정
        
        // 범위 체크
        if (!IsValidPlayArea(targetPos)) return;
        if (!CanInteract(targetPos)) return;
        
        // 스왑 실행
        _swapper.SwipeSwap(pos, targetPos);
    }

    // ====== 드래그 앤 드롭 ======
    
    // 스크린 -> 로컬 좌표 변환을 통해 블록을 놓을 그리드 번호 반환
    public int2 GetGridIndex(Vector2 anchoredPosition)
    {
        return _layout.GetGridIndex(anchoredPosition);
    }
    
    // 스왑 가능한 칸인지 체크
    public bool IsValidSwapTarget(int2 from, int2 to)
    {
        // 플레이 영역 내인지
        if (!IsValidPlayArea(to)) return false;
    
        // 인접 1칸인지 (대각선 불가)
        int2 diff = to - from;
        // 맨해튼 거리가 1이 아니면 인접하지 않음 (대각선은 거리 2이므로 자동 차단)
        if (math.abs(diff.x) + math.abs(diff.y) != 1) return false;
    
        // 대상 블록이 상호작용 가능한지
        if (!CanInteract(to)) return false;
    
        return true;
    }

    public void OnDragSwapBlock(int2 from, int2 to)
    {
        _swapper.SwapFromDrag(from, to);
    }
    
    public float GetStride()
    {
        return _layout.Stride;
    }
    
    public float GetCellSize()
    {
        return _cellSize;
    }
    
    // 블록 중심이 외곽선에 걸치도록 셀 반칸(cellSize/2)만큼 바깥으로 확장
    
    public Vector2 GetBoardMin()
    {
        // 보이는 영역 좌하단 (x=0, y=rows-1)
        Vector2 min = _layout.GetPosition(0, _rows - 1);
        float half = _cellSize * 0.5f;
        return new Vector2(min.x - half, min.y - half);
    }

    public Vector2 GetBoardMax()
    {
        // 보이는 영역 우상단 (x=columns-1, y=bufferRows)
        Vector2 min = _layout.GetPosition(_columns - 1, _bufferRows);
        float half = _cellSize * 0.5f;
        return new Vector2(min.x + half, min.y + half);
    }
    
    // 스왑 완료 이벤트
    private void OnSwapComplete()
    {
        var matches = _matchFinder.FindAllMatches();
        if (matches.Count > 0)
        {
            // 제거 → 낙하 → 리필 → 연쇄 루프 실행
            // 코루틴 완료 콜백에서 _isProcessing = false 처리
            StartCoroutine(_processor.ProcessMatches(matches, OnPuzzleComplete));
        }
        else
        {
            // 매치 없으면 되돌리기 스왑은 기획상 배제로 필요 없음
            // 추후 요청시 여기에 기능 개발 가능
            _isProcessing = false;
        }
    }
    
    // 퍼즐 완료 이벤트
    private void OnPuzzleComplete(PuzzleResult result)
    {
        _isProcessing = false;
        _onPuzzleComplete?.Invoke(result);
        Debug.Log($"콤보: {result.comboCount}, 타입별: {string.Join(", ", result.matchedCounts)}");
    }

    // 플레이 가능한 영역에서만 스왑이 가능하도록 설정
    private bool IsValidPlayArea(int2 pos)
    {
        return pos.x >= 0 && pos.x < _columns && pos.y >= _bufferRows && pos.y < _rows;
    }
}