using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InGameHUDController : MonoBehaviour
{
    [SerializeField] private ResetConfirmUI resetConfirmUI;
    [SerializeField] private EndTurnConfirmUI endTurnConfirmUI;
    
    [SerializeField] private Button btnEndTurn;

    private Tween _blinkTween;

    public void OnClickReset()
    {
        resetConfirmUI.Open();
    }

    public void OnClickEndTurn()
    {
        endTurnConfirmUI.Open();
    }

    public void SetEndTurnBlink(bool isAPEmpty)
    {
        _blinkTween?.Kill();

        CanvasGroup cg = btnEndTurn.GetComponent<CanvasGroup>();
        if (cg == null) cg = btnEndTurn.gameObject.AddComponent<CanvasGroup>();

        if (isAPEmpty)
        {
            _blinkTween = cg.DOFade(0f, 0.5f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);
        }
        else
        {
            cg.DOFade(1f, 0.1f);
        }
    }
}