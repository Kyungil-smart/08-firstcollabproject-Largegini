using UnityEngine;
using TMPro;
using DG.Tweening;

// 요약: 퍼즐 보드 위에 콤보 텍스트를 표시하는 UI 컴포넌트
// 작성자: 이성규
public class ComboDisplayUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI _comboText;
    [SerializeField] private CanvasGroup _canvasGroup;
    
    [Header("Settings")]
    [Tooltip("화면에 유지되는 대기 시간. (초 단위)")]
    [SerializeField] private float _displayDuration = 0.25f;
    [Tooltip("콤보 텍스트가 팝업되며 나타나는 데 걸리는 시간")]
    [SerializeField] private float _fadeInDuration = 0.05f;
    [Tooltip("콤보 텍스트가 서서히 사라지는 데 걸리는 시간")]
    [SerializeField] private float _fadeOutDuration = 0.2f;
    
    [Header("Floating")]
    [Tooltip("위로 떠오르는 거리 (px)")]
    [SerializeField] private float _floatDistance = 60f;
    
    private Sequence _currentSequence;
    private Vector2 _originalPosition; // 콤보 애니메이션 적용 전 원래 위치
    
    private void Awake()
    {
        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();
        
        // 플로팅 연출 후 다음 콤보 때 원래 위치로 되돌리기 위해 캐싱
        _originalPosition = _comboText.rectTransform.anchoredPosition;
        
        // 초기 상태: 숨김
        _canvasGroup.alpha = 0f;
        _comboText.text = "";
    }

    /// <summary>
    /// BoardManager.OnComboUpdated 이벤트에 연결.
    /// 연출 흐름: 페이드인 + 스케일 등장 -> 유지하며 상승 시작 -> 마지막 구간에서 페이드아웃
    /// </summary>

    public void OnComboUpdated(int comboCount)
    {
        // 1콤보 텍스트 연출 생략
        if (comboCount < 2) return;
        
        // 기존 연출 중이면 즉시 종료
        _currentSequence?.Kill();
        
        // 텍스트 갱신
        _comboText.text = $"X{comboCount}";
        
        // 연출 시작(콤보 텍스트 등장)시 위치, 스케일, 투명도 강제 초기화
        _comboText.rectTransform.anchoredPosition = _originalPosition;
        _comboText.transform.localScale = Vector3.zero;
        _canvasGroup.alpha = 0f; 
        
        // 연출: 페이드인 + 펀치 스케일 -> 유지하며 서서히 상승 -> 마지막에 페이드아웃
        _currentSequence = DOTween.Sequence()
            // 1. 투명도는 0에서 1로 서서히 켜짐 (페이드인)
            .Append(_canvasGroup.DOFade(1f, _fadeInDuration))
            // 2. 스케일은 0에서 1로 커지는데, Ease.OutBack 덕분에 살짝 커졌다가 1로 돌아옴 (펀치 느낌)
            .Join(
                _comboText.transform
                    .DOScale(Vector3.one, _fadeInDuration)
                    .SetEase(Ease.OutBack)
            )
            // 3. 화면에 머무는 시간 (대기)
            .AppendInterval(_displayDuration)
            // 4. 대기 직후부터 floatDistance만큼 상승 (대기+페이드아웃 전체 구간)
            // OutCubic: 처음에 빠르게 올라가고 끝에서 감속
            .Append(
                _comboText.rectTransform
                    .DOAnchorPosY(_originalPosition.y + _floatDistance, _fadeOutDuration)
                    .SetEase(Ease.OutCubic)
            ) 
            // 5. 등장+대기 이후 시점에 페이드아웃을 끼워넣어 상승 중 서서히 사라지게 함
            .Insert(
                _fadeInDuration + _displayDuration, 
                _canvasGroup.DOFade(0f, _fadeOutDuration)
            )
            .OnKill(() => _currentSequence = null);
    }

    // 연쇄 완료 시 즉시 숨김 (BoardManager.OnPuzzleComplete에 연결)
    public void OnChainComplete()
    {
        _currentSequence?.Kill();
        _canvasGroup.alpha = 0f;
        _comboText.text = "";
    }
}