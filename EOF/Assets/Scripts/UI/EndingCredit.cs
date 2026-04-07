using DG.Tweening;
using UnityEngine;

public class EndingCredit : MonoBehaviour
{
    public RectTransform creditContent;
    public AudioSource bgm;         
    public float scrollSpeed = 100f; 

    private void Start()
    {
        creditContent.anchoredPosition = new Vector2(0, -Screen.height);
        
        float endY = creditContent.sizeDelta.y + Screen.height;
        float duration = endY / scrollSpeed;
        
        if(bgm != null) bgm.Play();
        
        creditContent.DOAnchorPosY(endY, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                SceneLoader.Intance.ChangeScene(SceneLoader.Intance.Title);
            });
    }

    private void Update()
    {
        Time.timeScale = Input.GetKey(KeyCode.Space) ? 5.0f : 1.0f;
    }
}
