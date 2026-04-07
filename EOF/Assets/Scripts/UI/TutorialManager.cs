using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 작성자 : 홍정옥
// 기능 : 튜토리얼 씬 텍스트 및 버튼 제어

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject storyUI;
    [SerializeField] private GameObject dimPanel;
    [SerializeField] private GameObject popup;  // 다이얼로그가 있는 팝업 창
    [SerializeField] private GameObject puzzleArea;
    [SerializeField] private GameObject btnReset;   // 인게임 리셋 버튼
    [SerializeField] private GameObject btnEndTurn; // 인게임 턴 종료 버튼

    [SerializeField] private TMP_Text storyText;
    [SerializeField] private Image storyImage;
    //[SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;

    [SerializeField] private Sprite[] popupImages;
    [SerializeField] private Sprite iconClose;
    [SerializeField] private Image iconNext;
    [SerializeField] private Sprite iconContinue;

    [SerializeField] private BoardManager boardManager;
    [SerializeField] private TutorialBoardPreset preset;
    
    private Button _btnReset;
    private Button _btnEndTurn;

    private ITutorialBoardControl _board;
    private Action _savedProceed;

    [SerializeField] private DialogueTyper dialogueTyper;


    [field: SerializeField] public int currentIndex { get; set; } = 0;

    private UnityEngine.EventSystems.EventTrigger puzzleTrigger;

    // 로컬라이즈 테이블 불러오기(한성우)
    private string localeTableName = "LocalTable";

    // 화자 스프라이트를 불러올 게임 오브젝트 (한성우)
    [SerializeField] private GameObject speakerKing;
    [SerializeField] private GameObject speakerQueen;


    private void Awake()
    {
        _btnReset = btnReset.GetComponent<Button>();
        _btnEndTurn = btnEndTurn.GetComponent<Button>();
        // 로컬라이즈 텍스트 초기화 (한성우)
        InitText();
    }


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

        speakerKing.SetActive(true);
        speakerQueen.SetActive(false);

        UpdatePopup();
        popup.SetActive(true);
    }

    private void UpdatePopup()
    {
        btnReset.SetActive(false);
        btnEndTurn.SetActive(false);
        // Todo: 데이터 연동 후 교체
        //storyText.text = testTexts[currentIndex];
        dialogueTyper.ShowText(testTexts[currentIndex]);

        if (popupImages != null && currentIndex < popupImages.Length)
            storyImage.sprite = popupImages[currentIndex];
        //이전 버튼 주석 처리
        //prevButton.gameObject.SetActive(currentIndex > 0); 

        // Debug.Log($"UpdatePopup S {currentIndex}");
        bool isLast = (currentIndex == testTexts.Length - 1);
        iconNext.sprite = isLast ? iconClose : iconContinue;
        // Debug.Log($"UpdatePopup E {currentIndex}");
    }

    public void OnClickNext()
    {
        // 로컬라이즈 번호 메모 (한성우)
        //   인덱스 0~11 첫 대사
        //   12~14 두번째 대사
        //   15~16 첫 퍼즐 대사
        //   17~18 행동력 소모 안내 대사
        //   19 콤보 대사
        //   20~24 전투 UI 안내 대사

        // 첫 대사
        if (currentIndex < 3)
        {
            currentIndex++;
            UpdatePopup();  // 0, 1, 2, 3
            popup.SetActive(true);
        }
        // 인게임 UI 보이기
        else if (currentIndex == 3)
        {
            Debug.Log($"튜토리얼 2 S, 인덱스 : {currentIndex}");
            _board.SetInputLocked(false);
            _board.SetInteractionFilter(_ => false);
            _board.OnBoardTapped += OnBoardTapped;

            popup.SetActive(false); // 다이얼로그가 있는 팝업 창 닫기
            dimPanel.SetActive(true);
            //btnReset.SetActive(true);   // 인게임 리셋 버튼 활성화
            //btnEndTurn.SetActive(true); // 인게임 턴 종료 버튼 활성화
            puzzleTrigger.enabled = true;
            currentIndex++;

            speakerKing.SetActive(false);
            speakerQueen.SetActive(true);

            Debug.Log($"튜토리얼 2 E, 인덱스 : {currentIndex}");
        }
        // 대사
        else if (currentIndex >= 4 && currentIndex <= 7)
        {
            Debug.Log($"튜토리얼 3 S, 인덱스 : {currentIndex}");
            currentIndex++; // 4, 5, 6, 7
            UpdatePopup();
            popup.SetActive(true);
            Debug.Log($"튜토리얼 3 E, 인덱스 : {currentIndex}");
        }
        else if (currentIndex == 8)
        {
            Debug.Log($"튜토리얼 4 S, 인덱스 : {currentIndex}");
            _board.SetInteractionFilter(pos => pos.Equals(preset.DragSource));
            _board.SetSwapFilter((from, to) =>
                from.Equals(preset.DragSource) && to.Equals(preset.DragTarget));
            _board.SetChainInterceptor((matches, proceed) =>
            {
                _savedProceed = proceed;
                _board.SetChainInterceptor(null);
                currentIndex++;
                StartCoroutine(ShowPopupNextFrame());
            });

            popup.SetActive(false);
            btnReset.SetActive(true);
            btnEndTurn.SetActive(true);
            _btnReset.interactable = false;
            _btnEndTurn.interactable = false;
            Debug.Log($"튜토리얼 4 E, 인덱스 : {currentIndex}");
        }
        else if (currentIndex == 9)
        {
            Debug.Log($"튜토리얼 5 S, 인덱스 : {currentIndex}");
            _board.ClearAllHighlights();
            _savedProceed?.Invoke();
            _savedProceed = null;
            currentIndex++;
            popup.SetActive(false);
            Debug.Log($"튜토리얼 5 E, 인덱스 : {currentIndex}");
        }
        else if (currentIndex >= 10 && currentIndex <= 15)
        {
            Debug.Log($"튜토리얼 6 S, 인덱스 : {currentIndex}");
            currentIndex++;
            UpdatePopup();
            popup.SetActive(true);
            Debug.Log($"튜토리얼 6 E, 인덱스 : {currentIndex}");
        }
        else if (currentIndex == testTexts.Length - 1)
        {
            Debug.Log($"튜토리얼 7 S, 인덱스 : {currentIndex}");
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
        if (currentIndex == 10)
        {
            _btnReset.interactable = true;
            _btnEndTurn.interactable = true;
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

        // currentIndex = 2;
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
        btnReset.GetComponent<Button>().interactable = true;
        btnEndTurn.GetComponent<Button>().interactable = true;
        PlayerPrefs.SetInt("TutorialDone", 1);
    }

    /*public void OnClickPrev()
    {
        if (currentIndex > 1)
        {
            currentIndex--;
            UpdatePopup();
            popup.SetActive(true);
        }
    }*/

    public void OnClickSkip()
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
        btnReset.SetActive(true);
        btnEndTurn.SetActive(true);
        _btnReset.interactable = true;
        _btnEndTurn.interactable = true;
        PlayerPrefs.SetInt("TutorialDone", 1);
    }

    // Todo: 데이터 연동 후 교체
    // 로컬라이즈 테이블 기반 튜토리얼 텍스트 교체 (한성우)
    private string[] testTexts = new string[17];
    private void InitText()
    {
        // Debug.Log("InitText");
        // 로컬라이즈 테이블에서 텍스트 불러오기
        for (int i = 0; i < 4; i++)
        {
            testTexts[i] = LocalizationSettings.StringDatabase.GetLocalizedString(localeTableName, $"Grave_King_{i + 1}");
        }
        for (int j = 4; j < 17; j++)
        {
            testTexts[j] = LocalizationSettings.StringDatabase.GetLocalizedString(localeTableName, $"Grave_Princess_{j - 3}");
        }
    }

    /*
    private string[] testTexts =
    {
        "첫 대사",
        "퍼즐 UI 안내",
        "기본 조작 안내",
        "행동력 안내",
        "콤보 안내",
        "전투 UI 안내",
        "튜토리얼 종료",
        
    };
    */



}