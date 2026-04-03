using DG.Tweening;
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
    [SerializeField] private UIFrameEffect _matchEffect; // 이펙트 연출
    
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
        
        // 재활용 시 잔여 Tween 정리 (낙하/스왑 중 재활용될 경우 이전 Tween의 OnComplete가 상태 오염하는 것 방지)
        DOTween.Kill(Rect);
        
        // 재활용 시 이펙트 코루틴 강제 정리 (고아 코루틴 방지)
        if (_matchEffect != null)
            _matchEffect.ForceStopEffect();
        
        // 이펙트 재생 중 숨겨진 이미지 복원
        if (_blockImage != null)
            _blockImage.enabled = true;
        
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
    // onComplete: 이펙트 포함 모든 연출이 끝난 뒤 호출되는 콜백
    // BoardProcessor가 모든 블록의 클리어 완료를 대기하는 데 사용
    public void Despawn(System.Action onComplete = null)
    {
        // 진행 중인 낙하/스왑 Tween 정리
        DOTween.Kill(Rect);
        SetHighlight(false);
    
        // 이펙트가 있으면 재생 후 비활성화
        if (_matchEffect != null && _blockData?.MatchEffectFrames != null)
        {
            // 블록 이미지만 먼저 숨김
            _blockImage.enabled = false;
            _status = EBlockStatus.Destroying;
        
            _matchEffect.SetFrames(_blockData.MatchEffectFrames);
            _matchEffect.PlayEffect(Vector2.zero, () =>
            {
                _blockData = null;
                _status = EBlockStatus.None;
                _blockImage.enabled = true;
                gameObject.SetActive(false);
                onComplete?.Invoke();
            });
        }
        else
        {
            _blockData = null;
            _status = EBlockStatus.None;
            gameObject.SetActive(false);
            onComplete?.Invoke();
        }
    }
    
    public void SetHighlight(bool active, Color? color = null)
    {
        if (_highlight == null) return;
        _highlight.gameObject.SetActive(active);
        if (active && color.HasValue)
            _highlight.color = color.Value;
    }
}