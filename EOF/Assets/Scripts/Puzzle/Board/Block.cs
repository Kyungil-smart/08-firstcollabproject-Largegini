using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

// 요약 : 퍼즐 보드 위에 배치되는 실제 블록 객체
// 블럭의 데이터를 가지고 런타임 동작을 실행한다.
// 작성자 : 이성규
public class Block : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image _blockImage; // 블록의 비주얼을 담당할 UI Image
    [SerializeField] private Image _highlight;  // 셀 하이라이트 페이크 이미지

    [Header("Runtime Data")]
    private BlockDataSO _blockData;
    private int2 _gridPosition;
    private EBlockStatus _status = EBlockStatus.None;

    // 외부(매니저)에서 읽어갈 프로퍼티들
    public int2 GridPos => _gridPosition;
    public BlockDataSO Data => _blockData;
    public EBlockType Type => _blockData != null ? _blockData.Type : EBlockType.None;
    public EBlockStatus Status => _status;

    public IBoard Board { get; private set; }
    public RectTransform Rect { get; private set; }

    public BlockDragHandler DragHandler { get; private set; }

    // 처음 생성될 때, 또는 화면 밖에서 재배치되어 내려올 때 호출 
    public void Init(int2 pos, BlockDataSO data, IBoard board)
    {
        if (Rect == null)
            Rect = GetComponent<RectTransform>();
        if(_blockImage == null)
            _blockImage = GetComponent<Image>();
        if (DragHandler == null)
            DragHandler = GetComponent<BlockDragHandler>();
        
        Board = board;
        _gridPosition = pos;
        _blockData = data;
        _status = EBlockStatus.None;
        
        RefreshVisuals(); // 시각적 갱신은 따로 위임
        gameObject.SetActive(true);
    }
    
    // 데이터에 맞춰 UI 갱신
    private void RefreshVisuals()
    {
        if (_blockImage == null || _blockData == null) return;
        
        // 재사용되는 경우를 위한 상태 초기화
         _blockImage.sprite = _blockData.Sprite;
        // _blockImage.color = _blockData.Color;
        
        // TODO (차후 구현): 상태에 따른 시각적 변화 처리
        // 예: if (_status == EBlockStatus.Freeze) { 얼음막 UI 활성화 }
        //     else { 얼음막 UI 비활성화 }
    }

    /// <summary> 
    /// 위치가 스왑되거나 아래로 떨어질 때 논리적 좌표 갱신 
    /// (실제 Transform 이동은 BoardManager나 별도 Tween 연출에서 처리)
    /// </summary>
    public void SetPosition(int2 newPos)
    {
        _gridPosition = newPos;
    }
    
    /// <summary>
    /// 외부(기믹, 적의 스킬 등)에서 블록의 런타임 상태를 변경할 때 호출
    /// 런타임 중 Block 객체의 상태 변경 가능
    /// SO는 변하지 않는 원본 데이터이기에 런타임 객체와는 별개라 런타임 중 런타임 객체의 상태를 바꾸어도 상관없음
    /// </summary>
    public void SetStatus(EBlockStatus newStatus)
    {
        _status = newStatus;
        RefreshVisuals();
    }
    
    // 매칭되어 터질 때 호출 (파괴 대신 비활성화)
    public void Despawn()
    {
        // 추후 이곳에 DOTween 축소 연출이나 파티클 재생 로직 추가
        // 비활성화된 직후에 이 블록에 접근하지 않도록 주의 필요
        _blockData = null;
        _status = EBlockStatus.None;
        gameObject.SetActive(false);
        SetHighlight(false);
    }
    
    public void SetHighlight(bool active, Color? color = null)
    {
        if (_highlight == null) return;
        _highlight.gameObject.SetActive(active);
        if (active && color.HasValue)
            _highlight.color = color.Value;
    }
}
