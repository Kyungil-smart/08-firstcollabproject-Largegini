using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

// 요약 : 블록에 부착되어 유저의 터치/드래그 방향을 감지하는 역할
// 작성자 : 이성규
[RequireComponent(typeof(Block))]
public class BlockDragHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private Block _block;
    private RectTransform _boardPanel;
    private Vector2 _originalAnchoredPos; // 드래그 시작 전 원래 UI 좌표
    private Vector2 _dragOffset;          // 터치 지점과 블록 중심의 오프셋
    private bool _isDragging;
    private Camera _uiCamera;
    private Block _highlightedBlock;
    
    private void Awake()
    {
        if(_block == null)
            _block = GetComponent<Block>();
    }
    
    // Block.Init 이후 BoardPanel 참조를 전달받아야 스크린 -> 로컬 좌표 변환 가능
    public void SetBoardPanel(RectTransform boardPanel, Camera uiCamera = null)
    {
        _boardPanel = boardPanel;
        _uiCamera = uiCamera;
    }

    // 마우스/터치를 누르는 순간 (원래 UI 좌표)
    public void OnPointerDown(PointerEventData eventData)
    {
        _isDragging = false;
        
        // 터치하는 순간부터 조작 가능 여부 검사 (버퍼 영역이거나 연출 중이면 무시)
        if (!_block.Board.CanInteract(_block.GridPos)) return;
        
        // 드래그 시작 전 원래 위치 저장 (실패 시 복귀용)
        _originalAnchoredPos = _block.Rect.anchoredPosition;
        
        // 터치 지점과 블록 중심 오프셋 계산 (블록 중심이 아닌 터치한 지점 기준으로 따라가게)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _boardPanel, eventData.position, _uiCamera, out Vector2 localPoint);
        _dragOffset = _originalAnchoredPos - localPoint;

        // 드래그 중 다른 블록 위에 렌더링되도록 최상단으로
        _block.Rect.SetAsLastSibling();
        
        _isDragging = true;
    }

    // 드래그 중 블록이 손가락/마우스를 따라 이동
    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;

        // 마우스 좌표를 canvas내에서의 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _boardPanel, eventData.position, _uiCamera, out Vector2 localPoint);

        Vector2 targetPos = localPoint + _dragOffset;

        // 원래 위치에서 1칸(stride) 범위 내로 클램핑 (셀 외곽 근처까지 허용)
        // 셀 외곽 허용치를 0.3으로 제한
        // UI 좌표 → 그리드 인덱스로 반환할때 0.5면 RoundToInt 반올림 시 근처 첫번째 칸이 아닌 두번째칸 인덱스가 반환될 수 있음
        float stride = _block.Board.GetStride();
        float halfCell = _block.Board.GetCellSize() * 0.3f;
        targetPos.x = Mathf.Clamp(targetPos.x, _originalAnchoredPos.x - stride - halfCell, _originalAnchoredPos.x + stride + halfCell);
        targetPos.y = Mathf.Clamp(targetPos.y, _originalAnchoredPos.y - stride - halfCell, _originalAnchoredPos.y + stride + halfCell);
        
        // 보드 밖으로 나가지 않도록 보정
        Vector2 boardMin = _block.Board.GetBoardMin();
        Vector2 boardMax = _block.Board.GetBoardMax();
        targetPos.x = Mathf.Clamp(targetPos.x, boardMin.x, boardMax.x);
        targetPos.y = Mathf.Clamp(targetPos.y, boardMin.y, boardMax.y);
        
        _block.Rect.anchoredPosition = targetPos;

        UpdateHighlight();
    }

    // 손을 떼는 순간 (끝점 저장 및 방향 계산)
    public void OnPointerUp(PointerEventData eventData)
    {
        if(!_isDragging) return;
        _isDragging = false;
        
        // 하이라이트 해제
        ClearHighlight();
        
        // 드래그 도중 상태가 변했을 수 있으니 한 번 더 검사
        if (!_block.Board.CanInteract(_block.GridPos)) return;
        
        // 놓은 위치 -> 그리드 인덱스 역변환
        
        // 클램핑된 블록의 현재 위치로 그리드 인덱스 계산
        int2 dropGridPos = _block.Board.GetGridIndex(_block.Rect.anchoredPosition);
        int2 originGridPos = _block.GridPos;

        // 인접 1칸 + 유효한 스왑 대상인지 검증
        if (_block.Board.IsValidSwapTarget(originGridPos, dropGridPos))
        {
            // 드래그한 블록은 즉시 스냅, 상대 블록만 연출
            _block.Board.OnDragSwapBlock(originGridPos, dropGridPos);
        }
        else
        {
            // 유효하지 않으면 원위치 복귀
            _block.Rect.anchoredPosition = _originalAnchoredPos;
        }
    }
    
    // 하이라이트 업데이트
    private void UpdateHighlight()
    {
        int2 hoverGrid = _block.Board.GetGridIndex(_block.Rect.anchoredPosition);
    
        // 자기 자신 위치면 하이라이트 불필요
        if (hoverGrid.Equals(_block.GridPos))
        {
            ClearHighlight();
            return;
        }

        Block hoverBlock = _block.Board.GetBlock(hoverGrid);
    
        // 이전 하이라이트와 같으면 스킵
        if (hoverBlock == _highlightedBlock) return;
    
        // 이전 하이라이트 끄기
        ClearHighlight();
    
        if (hoverBlock == null) return;
    
        // 인접 1칸 + 대각선 여부 판별
        int2 diff = hoverGrid - _block.GridPos;
        bool isDiagonal = math.abs(diff.x) + math.abs(diff.y) != 1;
    
        // 대각선이면 빨간색, 유효한 스왑이면 초록색
        Color highlightColor = isDiagonal ? Color.red : Color.green;
    
        _highlightedBlock = hoverBlock;
        _highlightedBlock.SetHighlight(true, highlightColor);
    }
    
    private void ClearHighlight()
    {
        if (_highlightedBlock != null)
        {
            _highlightedBlock.SetHighlight(false);
            _highlightedBlock = null;
        }
    }
}
