using UnityEngine;

// 요약: ComboDisplayUI를 BoardManager 이벤트에 코드로 연결
// 작성자: 이성규
[RequireComponent(typeof(ComboDisplayUI))]
public class ComboDisplayBinder : MonoBehaviour
{
    [SerializeField] private BoardManager _boardManager;

    private ComboDisplayUI _display;

    private void Awake()
    {
        _display = GetComponent<ComboDisplayUI>();
    }
    
    private void OnEnable()
    {
        _boardManager.OnComboUpdated.AddListener(_display.OnComboUpdated);
        _boardManager.OnPuzzleComplete.AddListener(_ => _display.OnChainComplete());
    }

    private void OnDisable()
    {
        _boardManager.OnComboUpdated.RemoveListener(_display.OnComboUpdated);
        _boardManager.OnPuzzleComplete.RemoveListener(_ => _display.OnChainComplete());
    }
}