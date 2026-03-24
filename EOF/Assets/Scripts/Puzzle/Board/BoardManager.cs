using Unity.Mathematics;
using UnityEngine;

// 요약 : 퍼즐 보드 전체를 관리하는 컨트롤러 스크립트
// 작성자 : 이성규
public class BoardManager : MonoBehaviour, IBoardInteractable
{
    [Header("Board Settings")]
    [SerializeField, Tooltip("행/높이")] private int _rows = 12;
    [SerializeField, Tooltip("버퍼 높이")] private int _bufferRows = 6;
    [SerializeField, Tooltip("열/길이")] private int _columns = 5;
    [SerializeField, Tooltip("그리드 시작 위치")] private RectTransform _startRect;
    [SerializeField, Tooltip("그리드 한칸당 크기")] private float _cellSize;
    [SerializeField, Tooltip("그리드간 간격")] private float _spacing;

    [Header("References")]
    [SerializeField] private Block _blockPrefab;        // 스폰할 블록 프리팹
    [SerializeField] private RectTransform _boardPanel; // 블록들이 생성될 부모 캔버스 패널
    [SerializeField] private BlockDataSO[] _blockDatas; // 스폰 시 랜덤으로 부여할 SO 데이터 풀

    private SGrid2D<Block> _blocks;
    private BoardLayout _layout;
    private BoardSwapper _swapper;
    private BoardSpawner _spawner;

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

        // 하위 시스템 생성
        _spawner = new BoardSpawner(_blocks, _layout, _blockPrefab, _boardPanel, _blockDatas, this);
        _swapper = new BoardSwapper(_blocks, _layout);

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
        // 버퍼 구역이면 터치 금지
        if (pos.y < _bufferRows) return false;

        // 해당 블록이 연출 중이거나 비활성이면 금지
        Block targetBlock = _blocks[pos];
        if (targetBlock == null || targetBlock.Status != EBlockStatus.None) return false;

        // TODO (나중에 추가): 보드 전체가 낙하(Fall) 중이거나 터지는 중이면 return false;

        return true;
    }

    // 드래그 방향이 결정되면 핸들러가 이 함수를 요청
    public void OnSwipeBlock(int2 pos, Vector2Int direction)
    {
        int2 targetPos = new int2(pos.x + direction.x, pos.y - direction.y); // UI Y축 반전 보정

        // 범위 체크
        if (!IsValidPlayArea(targetPos)) return;
        if (!CanInteract(targetPos)) return;

        // 스왑 실행
        _swapper.Swap(pos, targetPos);
    }

    // 플레이 가능한 영역에서만 스왑이 가능하도록 설정
    private bool IsValidPlayArea(int2 pos)
    {
        return pos.x >= 0 && pos.x < _columns && pos.y >= _bufferRows && pos.y < _rows;
    }
}