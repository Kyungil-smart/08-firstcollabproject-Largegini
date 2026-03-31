using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ActionBarUI : MonoBehaviour
{
    [SerializeField] private Image layerBlue;
    [SerializeField] private Image layerRed;
    
    [SerializeField] private TMP_Text apText;
    
    [SerializeField] private float tweenDuration = 0.3f;
    
    [SerializeField] private InGameHUDController hudController;

    private void Start()
    {
        layerBlue.fillAmount = 0f;
        layerRed.fillAmount = 0f;
        SetAP(3, 3);
    }

    public void SetAP(int current, int max)
    {
        apText.text = $"행동력 {current}/{max}";
        
        float blueFill = Mathf.Clamp01((float)current / max);
        layerBlue.DOFillAmount(blueFill, tweenDuration).SetEase(Ease.OutCubic);
        
        int overflow = Mathf.Max(0, current - max);
        float redFill = Mathf.Clamp01((float)overflow / max);
        layerRed.DOFillAmount(redFill, tweenDuration).SetEase(Ease.OutCubic);
        
        hudController.SetEndTurnBlink(current <= 0);
    }
}