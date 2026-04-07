using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class ActionBarUI : MonoBehaviour
{
    [SerializeField] private Image layerBlue;
    [SerializeField] private Image layerRed;
    
    [SerializeField] private TMP_Text apText;
    [SerializeField] private string apString;   // 행동력 로컬라이즈를 위해 추가 (한성우)                                               
    private string localeTableName = "LocalTable";  // 로컬라이즈 테이블 불러오기(한성우)

    [SerializeField] private float tweenDuration = 0.3f;
    
    [SerializeField] private InGameHUDController hudController;
    
    private int _lastBehavior = -1;

    private void Start()
    {
        apString = LocalizationSettings.StringDatabase.GetLocalizedString(localeTableName, "Action_Point");

        layerBlue.fillAmount = 0f;
        layerRed.fillAmount = 0f;
        StartCoroutine(InitAfterFrame());
    }
    
    private void Update()
    {
        if (Player.Instance == null) return;
    
        int current = Player.Instance._behavior;
        if (current != _lastBehavior)
        {
            _lastBehavior = current;
            SetAP(current, Player.Instance._maxbehavior);
        }
    }
    private IEnumerator InitAfterFrame()
    {
        yield return null;
        
        while (Player.Instance._maxbehavior <= 0)
        {
            yield return null;
        }
    
        SetAP(Player.Instance._behavior, Player.Instance._maxbehavior);
    }

    public void SetAP(int current, int max)
    {
        int displayCurrent = Mathf.Max(0, current);
        
        apText.text = $"{apString} : {displayCurrent}/{max}";
        
        float blueFill = Mathf.Clamp01((float)displayCurrent / max);
        layerBlue.DOFillAmount(blueFill, tweenDuration).SetEase(Ease.OutCubic);
        
        int overflow = Mathf.Max(0, current - max);
        float redFill = Mathf.Clamp01((float)overflow / max);
        layerRed.DOFillAmount(redFill, tweenDuration).SetEase(Ease.OutCubic);
        
        hudController.SetEndTurnBlink(current <= 0);
    }
}