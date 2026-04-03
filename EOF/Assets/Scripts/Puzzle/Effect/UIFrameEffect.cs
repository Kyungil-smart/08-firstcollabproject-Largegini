using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// 요약: 이미지 교체 애니메이션 재생 스크립트
// 작성자: 이성규
public class UIFrameEffect : MonoBehaviour
{
    [SerializeField] private Image _effectImage;
    [SerializeField] private Sprite[] _frameSprites;
    [SerializeField] private float _fps = 15f;
    [SerializeField] private UnityEvent _onFinish;
    
    // 현재 재생 중인 코루틴 참조 (중복 재생 방지 + 강제 중단용)
    private Coroutine _runningCoroutine;
    
    // 이미지 할당
    public void SetFrames(Sprite[] frames)
    {
        _frameSprites = frames;
    }
    
    // 애니메이션 재생 호출
    public void PlayEffect(Vector2 anchoredPos, System.Action onComplete = null)
    {
        // 이전 재생이 남아있으면 정리 (중복 재생 방지)
        if (_runningCoroutine != null)
            StopCoroutine(_runningCoroutine);
        
        _effectImage.rectTransform.anchoredPosition = anchoredPos;
        _effectImage.gameObject.SetActive(true);
        _runningCoroutine = StartCoroutine(PlayFrames(onComplete));
    }

    /// <summary>
    /// 외부에서 강제 중단 시 콜백 보장
    /// 블록이 재활용(Init)되거나 비정상 상황에서 코루틴이 고아가 되는 것을 방지
    /// </summary>
    public void ForceStopEffect(System.Action onComplete = null)
    {
        if (_runningCoroutine != null)
        {
            StopCoroutine(_runningCoroutine);
            _runningCoroutine = null;
        }
        _effectImage.gameObject.SetActive(false);
        onComplete?.Invoke();
    }
    
    // 정해진 간격마다 다음 이미지로 교체하는 애니메이션 재생
    private IEnumerator PlayFrames(System.Action onComplete)
    {
        // 반복문 밖에서 한 번만 객체를 생성하여 메모리 할당 최소화.
        WaitForSeconds interval = new WaitForSeconds(1f / _fps);
        foreach (var frame in _frameSprites)
        {
            _effectImage.sprite = frame;
            yield return interval;
        }
        _effectImage.gameObject.SetActive(false);
        _runningCoroutine = null;
        onComplete?.Invoke();
    }
}