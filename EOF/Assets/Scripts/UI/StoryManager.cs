using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

// 작성자 : 홍정옥
// 기능 : 스토리 씬 텍스트 및 버튼 제어

public class StoryManager : MonoBehaviour
{
    [SerializeField] private TMP_Text storyText;
    [SerializeField] private Image storyImage;

    // 테스트용 데이터
    private string[] testTexts = {
        "첫 번째 텍스트",
        "두 번째 텍스트",
        "세 번째 텍스트",
        "오예 마지막 텍스트"
    };

    private int currentIndex = 0;

    void Start()
    {
        UpdateStory();
    }
    
    public void OnClickNext()
    {
        if (currentIndex < testTexts.Length - 1)
        {
            currentIndex++;
            UpdateStory();
        }
        else
        {
            SceneManager.LoadScene("Stage");
        }
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
        SceneManager.LoadScene("Stage");
    }
    
    private void UpdateStory()
    {
        storyText.text = testTexts[currentIndex];
    }
}