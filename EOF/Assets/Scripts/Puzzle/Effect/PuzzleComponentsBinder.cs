using UnityEngine;

// 요약: ComboDisplayUI를 BoardManager 이벤트에 코드로 연결
// 2026-04-06 수정 (ComboDisplayBinder 스크립트를 PuzzleComponentsBinder로 변경)
// 요약: BoardManager(로직)와 UI/SFX(표현) 컴포넌트들을 이벤트로 연결하는 바인딩 스크립트
// 작성자: 이성규
public class PuzzleComponentsBinder : MonoBehaviour
{
    [Header("Core Reference")]
    [SerializeField] private BoardManager _boardManager;
    
    [Header("Target Components")]
    [SerializeField] private ComboDisplayUI _display;
    [SerializeField] private PuzzleSFX _puzzleSfx;
    
    // OnPuzzleComplete에 맞춘 래퍼 - 람다 대신 캐싱하여 Remove 가능
    private UnityEngine.Events.UnityAction<PuzzleResult> _onPuzzleComplete;
    
    private void Awake()
    {
        // 수동 할당을 잊었을 때를 대비한 안전장치
        if (_display == null) _display = FindFirstObjectByType<ComboDisplayUI>();
        if (_puzzleSfx == null) _puzzleSfx = FindFirstObjectByType<PuzzleSFX>();
        
        _onPuzzleComplete = _ => _display.OnChainComplete();
    }
    
    private void OnEnable()
    {
        // 콤보 UI 갱신 연결
        _boardManager.OnComboUpdated.AddListener(_display.OnComboUpdated);
        
        // 콤보 사운드 재생 연결
        _boardManager.OnComboUpdated.AddListener(_puzzleSfx.PlayComboSfx);
        
        // 블록 교체 사운드 재생 연결
        _boardManager.OnSwapFinished.AddListener(_puzzleSfx.PlaySwapSfx);
        
        // 퍼즐 완료 -> UI 초기화
        _boardManager.OnPuzzleComplete.AddListener(_onPuzzleComplete);
    }

    private void OnDisable()
    {
        if (_boardManager == null) return;
        
        _boardManager.OnComboUpdated.RemoveListener(_display.OnComboUpdated);
        _boardManager.OnComboUpdated.RemoveListener(_puzzleSfx.PlayComboSfx);
        _boardManager.OnSwapFinished.RemoveListener(_puzzleSfx.PlaySwapSfx);
        _boardManager.OnPuzzleComplete.RemoveListener(_onPuzzleComplete);
    }
}