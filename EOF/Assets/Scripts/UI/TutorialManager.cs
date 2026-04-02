using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System;

// 작성자 : 홍정옥
// 기능 : 튜토리얼 씬 텍스트 및 버튼 제어

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject storyUI;
    [SerializeField] private GameObject dimPanel;
    [SerializeField] private GameObject popup;
    [SerializeField] private GameObject puzzleArea;
    [SerializeField] private GameObject btnReset;
    [SerializeField] private GameObject btnEndTurn;

    [SerializeField] private TMP_Text storyText;
    [SerializeField] private Image storyImage;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;
    

    [SerializeField] private Sprite[] popupImages;
    [SerializeField] private Sprite iconNext;
    [SerializeField] private Sprite iconClose;

    [SerializeField] private BoardManager boardManager;
    [SerializeField] private TutorialBoardPreset preset;

    private ITutorialBoardControl _board;
    private Action _savedProceed;

    private int currentIndex = 0;
    private UnityEngine.EventSystems.EventTrigger puzzleTrigger;

    void Start()
    {
        _board = boardManager as ITutorialBoardControl;
        _board.LoadPresetBoard(preset.ToGrid());
        _board.SetInputLocked(true);
        boardManager.OnPuzzleComplete.AddListener(OnPuzzleComplete);

        puzzleTrigger = puzzleArea.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        var entry = new UnityEngine.EventSystems.EventTrigger.Entry();
        entry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerClick;
        entry.callback.AddListener((_) => OnPuzzleAreaClick());
        puzzleTrigger.triggers.Add(entry);
        puzzleTrigger.enabled = false;

        UpdatePopup();
        popup.SetActive(true);
    }

    private void UpdatePopup()
    {
        btnReset.SetActive(false);
        btnEndTurn.SetActive(false);

        // Todo: 데이터 연동 후 교체
        storyText.text = testTexts[currentIndex];

        if (popupImages != null && currentIndex < popupImages.Length)
            storyImage.sprite = popupImages[currentIndex];

        prevButton.gameObject.SetActive(currentIndex > 0);
        
        bool isLast = (currentIndex == testTexts.Length - 1);
        nextButton.image.sprite = isLast ? iconClose : iconNext;
    }

    public void OnClickNext()
    {
        if (currentIndex == 0)
        {
            _board.SetInputLocked(false);
            _board.SetInteractionFilter(_ => false);
            _board.OnBoardTapped += OnBoardTapped;

            popup.SetActive(false);
            dimPanel.SetActive(true);
            btnReset.SetActive(true);
            btnEndTurn.SetActive(true);
            puzzleTrigger.enabled = true;
            currentIndex = 1;
        }
        else if (currentIndex == 2)
        {
            currentIndex++;
            UpdatePopup();
            popup.SetActive(true);
        }
        else if (currentIndex == 3)
        {
            _board.SetInteractionFilter(pos => pos.Equals(preset.DragSource));
            _board.SetSwapFilter((from, to) =>
                from.Equals(preset.DragSource) && to.Equals(preset.DragTarget));
            _board.SetChainInterceptor((matches, proceed) =>
            {
                _savedProceed = proceed;
                _board.SetChainInterceptor(null);
                currentIndex = 4;
                StartCoroutine(ShowPopupNextFrame());
            });

            popup.SetActive(false);
            btnReset.SetActive(true);
            btnEndTurn.SetActive(true);
        }
        else if (currentIndex == 4)
        {
            _board.ClearAllHighlights();
            _savedProceed?.Invoke();
            _savedProceed = null;
            currentIndex = 5;
            popup.SetActive(false);
        }
        else if (currentIndex == 5)
        {
            currentIndex++;
            UpdatePopup();
            popup.SetActive(true);
        }
        else if (currentIndex == testTexts.Length - 1)
        {
            OnClickClose();
        }
    }

    private System.Collections.IEnumerator ShowPopupNextFrame()
    {
        yield return null;
        storyUI.SetActive(true);
        UpdatePopup();
        popup.SetActive(true);
    }

    private void OnPuzzleComplete(PuzzleResult result)
    {
        if (currentIndex == 5)
        {
            UpdatePopup();
            storyUI.SetActive(true);
            popup.SetActive(true);
        }
    }

    public void OnPuzzleAreaClick()
    {
        puzzleTrigger.enabled = false;
        _board.OnBoardTapped -= OnBoardTapped;
        _board.SetBlockHighlights(preset.GetHighlightPositions(), Color.yellow);

        currentIndex = 2;
        UpdatePopup();
        popup.SetActive(true);
    }

    private void OnBoardTapped()
    {
        _board.OnBoardTapped -= OnBoardTapped;
        OnPuzzleAreaClick();
    }
    
    public void OnClickClose()
    {
        _board.SetInteractionFilter(null);
        _board.SetSwapFilter(null);
        _board.SetInputLocked(false);
        _board.SetChainInterceptor(null);
        _board.ClearAllHighlights();
        _savedProceed?.Invoke();

        storyUI.SetActive(false);
        inGameUI.SetActive(true);
        dimPanel.SetActive(false);
        PlayerPrefs.SetInt("TutorialDone", 1);
    }

    public void OnClickPrev()
    {
        if (currentIndex > 1)
        {
            currentIndex--;
            UpdatePopup();
            popup.SetActive(true);
        }
    }

    public void OnClickSkip()
    {
        _board.SetInteractionFilter(null);
        _board.SetSwapFilter(null);
        _board.SetInputLocked(false);
        _board.SetChainInterceptor(null);
        _board.ClearAllHighlights();
        _savedProceed?.Invoke();

        PlayerPrefs.SetInt("TutorialDone", 1);
        SceneManager.LoadScene("Stage");
    }

    // Todo: 데이터 연동 후 교체
    private string[] testTexts =
    {
        "첫 대사",
        "퍼즐 UI 안내",
        "기본 조작 안내",
        "행동력 안내",
        "콤보 안내",
        "전투 UI 안내",
        "튜토리얼 종료"
    };
}