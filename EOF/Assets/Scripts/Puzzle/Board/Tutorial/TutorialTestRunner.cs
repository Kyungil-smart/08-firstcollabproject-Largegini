#if UNITY_EDITOR
using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

// 요약: UI 없이 튜토리얼 흐름을 에디터에서 단계별 테스트
// 작성자: 이성규
public class TutorialTestRunner : MonoBehaviour
{
    [SerializeField] private BoardManager _boardManager;
    [SerializeField] private TutorialBoardPreset _preset;
    
    private ITutorialBoardControl _board;
    private Action _savedProceed;
    private int _currentStep = 0;

    private void Awake()
    {
        _board = _boardManager as ITutorialBoardControl;
    }

    private void Start()
    {
        Init();
    }

    void Init()
    {
        // 프리셋 보드는 게임 시작 시 바로 로드 (랜덤 보드가 보이지 않도록)
        if (_preset != null)
        {
            _board.LoadPresetBoard(_preset.ToGrid());
            Debug.Log("프리셋 보드 로드 완료.");
        }
        
        // 시작 시 전체 입력 차단
        _board.SetInputLocked(true);
    }

    private void Update()
    {
        if (Keyboard.current == null) return;
        
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            NextStep();
        
        if (Keyboard.current.rKey.wasPressedThisFrame)
            ResetTutorial();
    }

    [ContextMenu("Tutorial: Next Step")]
    public void NextStep()
    {
        _currentStep++;
        Debug.Log($"=== 튜토리얼 {_currentStep}단계 ===");
        
        switch (_currentStep)
        {
            case 1: Step1_Prologue(); break;
            case 2: Step2_PuzzleUI(); break;
            case 3: Step3_BasicControl(); break;
            case 4: Step4_ActionPoint(); break;
            case 5: Step5_Combo(); break;
            case 6: Step6_BattleUI(); break;
            case 7: StepEnd(); break;
        }
    }

    // 1단계: 전체 입력 차단 (프롤로그 팝업 대기)
    private void Step1_Prologue()
    {
        _board.SetInputLocked(true);
        Debug.Log("입력 잠금. Space로 다음 단계.");
    }

    // 2단계: 탭만 감지, 드래그 차단
    private void Step2_PuzzleUI()
    {
        _board.SetInputLocked(false);
        _board.SetInteractionFilter(_ => false);
        _board.OnBoardTapped += OnBoardTapped;
        Debug.Log("퍼즐 영역 클릭 대기 중. 블록을 탭하세요.");
    }

    private void OnBoardTapped()
    {
        Debug.Log("보드 탭 감지됨!");
        _board.OnBoardTapped -= OnBoardTapped;
    }

    // 3단계: 하이라이트 표시
    private void Step3_BasicControl()
    {
        if (_preset != null)
            _board.SetBlockHighlights(_preset.GetHighlightPositions(), Color.yellow);
        else
            _board.SetBlockHighlights(new[] { new int2(1, 10), new int2(2, 10) }, Color.yellow);
        
        Debug.Log("하이라이트 표시. Space로 다음 단계.");
    }

    // 4단계: 특정 블록만 조작 + 스왑 방향 제한 + 인터셉터
    private void Step4_ActionPoint()
    {
        if (_preset != null)
        {
            // 잡기 제한: 드래그 소스만 잡을 수 있음
            _board.SetInteractionFilter(pos => pos.Equals(_preset.DragSource));
            
            // 스왑 방향 제한: 소스 → 타겟 방향으로만 놓을 수 있음
            _board.SetSwapFilter((from, to) =>
                from.Equals(_preset.DragSource) && to.Equals(_preset.DragTarget));
            
            Debug.Log($"({_preset.DragSource.x},{_preset.DragSource.y})만 잡기 가능, " +
                      $"({_preset.DragTarget.x},{_preset.DragTarget.y}) 방향으로만 스왑 가능.");
        }
        else
        {
            _board.SetInteractionFilter(null);
            _board.SetSwapFilter(null);
            Debug.Log("프리셋 없음. 필터 해제.");
        }

        _board.SetChainInterceptor((matches, proceed) =>
        {
            _savedProceed = proceed;
            Debug.Log($"인터셉트: {matches.Count}개 매치 일시정지. Space로 재개.");
        });
    }

    // 5단계: 파이프라인 재개 + 하이라이트 해제
    private void Step5_Combo()
    {
        _board.SetChainInterceptor(null);
        _board.ClearAllHighlights();

        if (_savedProceed != null)
        {
            _savedProceed.Invoke();
            _savedProceed = null;
            Debug.Log("매칭 파이프라인 재개.");
        }
        else
        {
            Debug.Log("일시정지된 파이프라인 없음. Space로 다음 단계.");
        }
    }

    // 6단계: 안내만
    private void Step6_BattleUI()
    {
        Debug.Log("전투 UI 안내 단계. Space로 종료.");
    }

    // 종료: 모든 제한 해제
    private void StepEnd()
    {
        _board.SetInteractionFilter(null);
        _board.SetSwapFilter(null);
        _board.SetInputLocked(false);
        _board.SetChainInterceptor(null);
        _board.ClearAllHighlights();
        _savedProceed = null;
        _currentStep = 0;
        Debug.Log("튜토리얼 종료. 자율 플레이. R키로 리셋.");
    }

    [ContextMenu("Tutorial: Reset")]
    public void ResetTutorial()
    {
        _board.SetInteractionFilter(null);
        _board.SetSwapFilter(null);
        _board.SetInputLocked(false);
        _board.SetChainInterceptor(null);
        _board.ClearAllHighlights();
        _savedProceed = null;
        _currentStep = 0;
        _boardManager.ResetBoard();
        Debug.Log("튜토리얼 리셋 완료.");
    }
}
#endif