using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

// 요약: 에디터 전용 보드 테스트 메서드 모음 (빌드에 포함되지 않음)
// BoardManager 오브젝트와 같이 배치하거나 별도 오브젝트에 부착
// Inspector 우클릭 -> ContextMenu로 개별 테스트
// 작성자: 이성규
#if UNITY_EDITOR
public class BoardTestHelper : MonoBehaviour
{
    [SerializeField] private BoardManager _boardManager;

    [ContextMenu("Test: Toggle Input Lock")]
    public void TestToggleInputLock()
    {
        var handler = _boardManager.TutorialHandler;
        handler.InputLocked = !handler.InputLocked;
        Debug.Log($"입력 잠금: {handler.InputLocked}");
    }
    
    [ContextMenu("Test: Highlight (1,10)(2,10)")]
    public void TestHighlight()
    {
        var positions = new[] { new int2(1, 10), new int2(2, 10) };
        _boardManager.SetBlockHighlights(positions, Color.yellow);
    }
    
    [ContextMenu("Test: Clear Highlights")]
    public void TestClearHighlights()
    {
        _boardManager.ClearAllHighlights();
    }
    
    [ContextMenu("Test: Filter Only (1,10)")]
    public void TestFilter()
    {
        var allowed = new int2(1, 10);
        _boardManager.SetInteractionFilter(pos => pos.Equals(allowed));
        Debug.Log("필터 설정: (1,10)만 조작 가능");
    }
    
    [ContextMenu("Test: Clear Filter")]
    public void TestClearFilter()
    {
        _boardManager.SetInteractionFilter(null);
        Debug.Log("필터 해제");
    }
    
    // 매칭 파이프라인 인터셉터 테스트: 3초 후 자동 재개
    [ContextMenu("Test: Chain Interceptor (3초 후 재개)")]
    public void TestChainInterceptor()
    {
        _boardManager.SetChainInterceptor((matches, proceed) =>
        {
            Debug.Log($"인터셉트: {matches.Count}개 매치 일시정지. 3초 후 재개.");
            StartCoroutine(DelayedProceed(proceed, 3f));
        });
    }
    
    private IEnumerator DelayedProceed(Action proceed, float delay)
    {
        yield return new WaitForSeconds(delay);
        _boardManager.SetChainInterceptor(null);
        proceed();
    }
    
    [ContextMenu("Test: Deadlock Board")]
    public void TestDeadlock()
    {
        _boardManager.CreateDeadlockBoard();
    }
}
#endif