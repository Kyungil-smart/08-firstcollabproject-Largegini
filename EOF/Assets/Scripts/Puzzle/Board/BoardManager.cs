using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

// 요약 : 퍼즐 보드 전체를 관리하고 블록을 스폰하는 관리자 스크립트
// 작성자 : 이성규
public class BoardManager : MonoBehaviour
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

    private void Awake()
    {
        // ResetBoard에서는 _layout을 안 쓰지만 낙하 로직에서 좌표 조회할 때 필요하니 Awake에서 한 번만 생성
        _layout = new BoardLayout(
            _startRect.anchoredPosition, 
            _cellSize, 
            _spacing,
            _bufferRows
        );
        
        InitializeBoard();
    }

    private void InitializeBoard()
    {
        // 블록으로 그리드 구조 데이터 생성
        _blocks = new SGrid2D<Block>(new int2(_columns, _rows));
        
        // 2. 물리적 블록 스폰 및 배치
        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _columns; x++)
            {
                SpawnBlock(new int2(x, y));
            }
        }
    }

    // 기존 오브젝트를 유지하되 데이터만 교체
    public void ResetBoard()
    {
        for (int y = 0; y < _blocks.Size.y; y++)
        {
            for (int x = 0; x < _blocks.Size.x; x++)
            {
                var pos = new int2(x, y);
                var block = _blocks[pos];
                int randomIndex = Random.Range(0, _blockDatas.Length);
                block.Init(pos, _blockDatas[randomIndex]); // 데이터만 교체
            }
        }
    }

    private void SpawnBlock(int2 pos)
    {
        // 프리팹 생성 및 부모 설정
        Block newBlock = Instantiate(_blockPrefab, _boardPanel);
        // 인스펙터 상에서 쉬운 디버깅을 위해 블록에 위치값으로 이름 설정
        newBlock.name = $"Block_{pos.y}_{pos.x}";
        
        // UI 좌표 설정
        RectTransform rect = newBlock.GetComponent<RectTransform>();
        rect.anchoredPosition = _layout.GetPosition(pos);

        // 랜덤 데이터 할당 및 초기화
        int randomIndex = Random.Range(0, _blockDatas.Length);
        newBlock.Init(pos, _blockDatas[randomIndex]);
        
        // 그리드 데이터에 등록
        _blocks[pos] = newBlock;
    }
}
