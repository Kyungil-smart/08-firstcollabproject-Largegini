using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

// 작성자 : 홍정옥
// 기능 : 튜토리얼 씬 텍스트 및 버튼 제어

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject storyUI;
    [SerializeField] private GameObject dimPanel;
    [SerializeField] private GameObject popup;
    [SerializeField] private GameObject puzzleArea;

    [SerializeField] private TMP_Text storyText;
    [SerializeField] private Image storyImage;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button closeButton;

    [SerializeField] private Sprite[] popupImages;

    private int currentIndex = 0;
    
    private UnityEngine.EventSystems.EventTrigger puzzleTrigger;

    void Start()
    {
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
        storyText.text = testTexts[currentIndex];

        if (popupImages != null && currentIndex < popupImages.Length)
            storyImage.sprite = popupImages[currentIndex];
        
        prevButton.gameObject.SetActive(currentIndex > 0);

        bool isLast = (currentIndex == testTexts.Length - 1);
        nextButton.gameObject.SetActive(!isLast);
        closeButton.gameObject.SetActive(isLast);
    }

    private void OnClickNext()
    {
        if (currentIndex == 0)
        {
            popup.SetActive(false);
            puzzleArea.GetComponent<UnityEngine.EventSystems.EventTrigger>().enabled = true;
            currentIndex = 1;
        }
        else if (currentIndex == 3)
        {
            popup.SetActive(false);
            // Todo : 타일 하이라이트에 드래그? 스왑? 연결
        }
        else if (currentIndex == 4)
        {
            // Todo : 3매치 재개 호출 -> 5번 팝업 호출
            currentIndex++;
            UpdatePopup();
        }
        else if (currentIndex < testTexts.Length - 1)
        {
            currentIndex++;
            UpdatePopup();
        }
    }
        public void OnPuzzleAreaClick()
        {
            puzzleArea.GetComponent<UnityEngine.EventSystems.EventTrigger>().enabled = false;
            currentIndex = 1;
            UpdatePopup();
            popup.SetActive(true);
        }
        
        public void OnClickClose()
        {
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
            PlayerPrefs.SetInt("TutorialDone", 1);
            SceneManager.LoadScene("Stage");
        }
        
        // 테스트용 - 추후 교체
        private string[] testTexts = {
            "첫 대사",
            "퍼즐 UI 안내",
            "기본 조작 안내",
            "행동력 안내",
            "콤보 안내",
            "전투 UI 안내"
        };
}
