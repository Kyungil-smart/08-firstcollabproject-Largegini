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
    [SerializeField] private float _displayDuration = 1.5f;
    [Tooltip("콤보 텍스트가 팝업되며 나타나는 데 걸리는 시간")]
    [SerializeField] private float _fadeInDuration = 0.15f;
    [Tooltip("콤보 텍스트가 서서히 사라지는 데 걸리는 시간")]
    [SerializeField] private float _fadeOutDuration = 0.3f;
    
    private Sequence _currentSequence;

    private void Awake()
    {
        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();

        // 초기 상태: 숨김
        _canvasGroup.alpha = 0f;
        _comboText.text = "";
    }

    // BoardManager.OnComboUpdated 이벤트에 연결
    public void OnComboUpdated(int comboCount)
    {
        // 기존 연출 중이면 즉시 종료
        _currentSequence?.Kill();
        
        // 텍스트 갱신
        _comboText.text = $"X{comboCount}";
        
        // 연출: 페이드인 + 펀치 스케일 -> 유지 -> 페이드아웃
        // 콤보 텍스트 등장시 스케일 0, 투명도 0으로 세팅
        _comboText.transform.localScale = Vector3.zero;
        _canvasGroup.alpha = 0f; 
        
        _currentSequence = DOTween.Sequence()
            // 1. 투명도는 0에서 1로 서서히 켜짐
            .Append(_canvasGroup.DOFade(1f, _fadeInDuration))
            // 2. 스케일은 0에서 1로 커지는데, Ease.OutBack 덕분에 1.2 정도까지 커졌다가 1로 돌아옴 (펀치 느낌)
            .Join(_comboText.transform.DOScale(Vector3.one, _fadeInDuration).SetEase(Ease.OutBack))
            // 3. 화면에 머무는 시간
            .AppendInterval(_displayDuration)
            // 4. 서서히 사라짐
            .Append(_canvasGroup.DOFade(0f, _fadeOutDuration))
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