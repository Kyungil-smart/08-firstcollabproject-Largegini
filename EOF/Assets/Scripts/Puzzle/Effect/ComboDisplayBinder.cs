using UnityEngine;

// 요약: ComboDisplayUI를 BoardManager 이벤트에 코드로 연결
// 작성자: 이성규
[RequireComponent(typeof(ComboDisplayUI))]
public class ComboDisplayBinder : MonoBehaviour
{
    [SerializeField] private BoardManager _boardManager;

    private ComboDisplayUI _display;
    
    // OnPuzzleComplete에 맞춘 래퍼 - 람다 대신 캐싱하여 Remove 가능
    private UnityEngine.Events.UnityAction<PuzzleResult> _onPuzzleComplete;
    
    private void Awake()
    {
        _display = GetComponent<ComboDisplayUI>();
        _onPuzzleComplete = _ => _display.OnChainComplete();
    }
    
    private void OnEnable()
    {
        _boardManager.OnComboUpdated.AddListener(_display.OnComboUpdated);
        _boardManager.OnPuzzleComplete.AddListener(_onPuzzleComplete);
    }

    private void OnDisable()
    {
        _boardManager.OnComboUpdated.RemoveListener(_display.OnComboUpdated);
        _boardManager.OnPuzzleComplete.RemoveListener(_onPuzzleComplete);
    }
}