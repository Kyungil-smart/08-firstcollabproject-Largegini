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
    [SerializeField] private GameObject[] popups; 
    
    [SerializeField] private TMP_Text[] popupTexts;
    [SerializeField] private Image storyImage;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button puzzleAreaButton;
    
    private int currentIndex = 0;
    
    void Start()
    {
        puzzleAreaButton.interactable = false;
        UpdateStory();
        ShowPopUp(0);
    }

    public void ShowPopUp(int index)
    {
        foreach (var  popup in popups)
        {
            popup.SetActive(false);
        }

        if (index < popups.Length)
        {
            popups[index].SetActive(true);
        }
    }
    
    public void OnClickNext()
    {
        if (currentIndex == 0)
        {
            ShowPopUp(popups.Length);
            puzzleAreaButton.interactable = true;
            currentIndex++;
        }
        else if (currentIndex < testTexts.Length - 1)
        {
            currentIndex++;
            UpdateStory();
            ShowPopUp(currentIndex);
        }
        else
        {
            storyUI.SetActive(false);
            inGameUI.SetActive(true);
            dimPanel.SetActive(false);
            PlayerPrefs.SetInt("TutorialDone", 1);
        }
    }

    public void OnPuzzleAreaClicked()
    {
        puzzleAreaButton.interactable = false;
        currentIndex = 1;
        UpdateStory();
        ShowPopUp(currentIndex);
    }
    
    public void OnClickPrev()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateStory();
        }
    }
    
    public void OnClickSkip()
    {
        PlayerPrefs.SetInt("TutorialDone", 1);
        SceneManager.LoadScene("Stage");
    }
    
    private void UpdateStory()
    {
        popupTexts[currentIndex].text = testTexts[currentIndex];
        
        prevButton.gameObject.SetActive(currentIndex > 0);
        
        puzzleAreaButton.interactable = (currentIndex >= 1);
    }
    
    // 테스트용 추후 교체
    private string[] testTexts = {
        "첫 대사",
        "퍼즐 UI 안내",
        "기본 조작 안내",
        "행동력 안내",
        "콤보 안내",
        "전투 UI 안내"
    };
}
