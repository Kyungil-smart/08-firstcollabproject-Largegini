using System;
using UnityEngine;
using UnityEngine.EventSystems;

// 요약 : 블록에 부착되어 유저의 터치/드래그 방향을 감지하는 역할
// 작성자 : 이성규
[RequireComponent(typeof(Block))]
public class BlockDragHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private Block _block;
    private Vector2 _startPos;
    private const float DRAG_THRESHOLD = 30f; // 이 픽셀 이상 움직여야 스와이프로 인정
    private bool _isDragging;
    
    private void Awake()
    {
        if(_block == null)
            _block = GetComponent<Block>();
    }

    // 마우스/터치를 누르는 순간 (시작점 저장)
    public void OnPointerDown(PointerEventData eventData)
    {
        _isDragging = false;
        // _boardManager.CanInteract 대신 블록이 들고 있는 매니저 참조 사용
        // 터치하는 순간부터 조작 가능 여부 검사 (버퍼 영역이거나 연출 중이면 무시)
        if (!_block.Board.CanInteract(_block.GridPos)) return;
        _startPos = eventData.position;
        _isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 드래그 이벤트
    }

    // 손을 떼는 순간 (끝점 저장 및 방향 계산)
    public void OnPointerUp(PointerEventData eventData)
    {
        if(!_isDragging) return;
        _isDragging = false;
        
        // 드래그 도중 상태가 변했을 수 있으니 한 번 더 검사
        if (!_block.Board.CanInteract(_block.GridPos)) return;
        
        Vector2 endPos = eventData.position;
        Vector2 diff = endPos - _startPos;

        // 그냥 클릭만 한 거면 무시 (미세한 터치를 드래그 이벤트로 처리하는 것을 방지)
        if (diff.magnitude < DRAG_THRESHOLD) return;
        
        // 매니저에게 넘겨줄 방향 벡터 변수 선언
        Vector2Int swipeDir = Vector2Int.zero;
        
        // 가로로 밀었나, 세로로 밀었나 판별(가로 스와이프 절대값이 )
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
        {
            // 가로 스와이프
            if (diff.x > 0)
            {
                Debug.Log($"{_block.GridPos} 블록 -> 오른쪽으로 밀었음!");
                swipeDir = Vector2Int.right;
            }
            else
            {
                Debug.Log($"{_block.GridPos} 블록 -> 왼쪽으로 밀었음!");
                swipeDir = Vector2Int.left;
            }
        }
        else
        {
            // 세로 스와이프
            if (diff.y > 0)
            {
                Debug.Log($"{_block.GridPos} 블록 -> 위로 밀었음!");
                swipeDir = Vector2Int.up;
            }
            else
            {
                Debug.Log($"{_block.GridPos} 블록 -> 아래로 밀었음!");
                swipeDir = Vector2Int.down;
            }
        }
        
        // 매니저에게 스왑 요청
        _block.Board.OnSwipeBlock(_block.GridPos, swipeDir);
    }
}
