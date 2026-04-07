using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class EndingCredit : MonoBehaviour
{
    public RectTransform creditContent;
    public AudioClip bgm;         
    public float scrollSpeed = 100f; 
    
    private void Start() 
    {
        float endY = 4000f; 
        float duration = endY / scrollSpeed;

        if (bgm != null) SoundManager.Instance.PlayBGM(bgm); 
        
        creditContent.DOAnchorPosY(endY, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                Debug.Log("크레딧 종료! 타이틀로 이동합니다.");
                Time.timeScale = 1.0f;
                SoundManager.Instance.StopBGM();
                SceneLoader.Intance.ChangeScene(SceneLoader.Intance.Title);
            });
    }

    public void OnFast(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) Time.timeScale = 5.0f;
        else if (ctx.canceled) Time.timeScale = 1.0f;
    }
}