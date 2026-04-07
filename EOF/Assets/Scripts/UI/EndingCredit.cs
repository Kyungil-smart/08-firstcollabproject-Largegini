using DG.Tweening;
using UnityEngine;

public class EndingCredit : MonoBehaviour
{
    public RectTransform creditRect;
    public float scrollSpeed = 50f;
    public float targetY = 2000f;
    public void StartCredit()
    {
        creditRect.DOAnchorPos(new Vector2(0, targetY), 50f)
            .SetEase(Ease.Linear) 
            .OnComplete(() => {
                SceneLoader.Intance.Fade.FadeIn();
                SceneLoader.Intance.ChangeScene(SceneLoader.Intance.Title);
            });
    }
}
