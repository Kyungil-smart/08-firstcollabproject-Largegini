using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeIO : MonoBehaviour
{
    /*
     * 작성 내용 : 페이드 인, 아웃 기능
     * 작성자 : 안정연
     */
    
    private Canvas _canvas;
    private CanvasScaler _canvasScaler; 
    private CanvasGroup _canvasGroup;
    private float _fadeDuration;
    
    // 사신 등장 텍스트
    private TextMeshProUGUI _tmp;
    // private string _txt;

    // 로컬라이즈 테이블 불러오기(한성우)
    private string localeTableName = "LocalTable";

    private void Awake()
    {
        _fadeDuration = 1f;
        
        _canvas = gameObject.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 10;
       
        _canvasScaler = gameObject.AddComponent<CanvasScaler>();
        _canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        _canvasScaler.referenceResolution = new Vector2(1920, 1080);
        _canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        _canvasScaler.matchWidthOrHeight = 1;
        _canvasScaler.referencePixelsPerUnit = 64;

       Addressables.InstantiateAsync("FadeImg", gameObject.transform).Completed += SetCanvasGroup;
    }

    private void SetCanvasGroup(AsyncOperationHandle<GameObject> op)
    {
        if (op.Status == AsyncOperationStatus.Succeeded)
        {
            _canvasGroup = op.Result.GetComponent<CanvasGroup>();
            _tmp = op.Result.GetComponentInChildren<TextMeshProUGUI>();
            _tmp.text = LocalizationSettings.StringDatabase.GetLocalizedString(localeTableName, $"Unknown_Death_04");
        }
    else

    {
            Debug.Log("페이드 이미지 레이어 그룹 추가실패");
        }
            
    }

    public void FadeIn()
    {
        _canvasGroup.DOFade(1, _fadeDuration)
            .OnStart(() =>
            {
                _canvasGroup.blocksRaycasts = true;
            })
            .OnComplete(() =>
            {
                FadeOut();
            });
    }

    public void FadeOut()
    {
        _canvasGroup.DOFade(0, _fadeDuration+1f)
            .OnStart(() =>
            {
                
            })
            .OnComplete(() =>
            {
                _canvasGroup.blocksRaycasts = false;
                _tmp.enabled = false;
            });
    }
    
    
    public void ChangeScene(ESceneType sceneType)
    { /// 외부에서 전환할 씬 이름 받기 ///
        _canvasGroup.DOFade(1, _fadeDuration)
            .OnStart(()=>{
                _canvasGroup.blocksRaycasts = true;
            })
            .OnComplete(()=>{
                SceneManager.LoadScene((int)sceneType);
                FadeOut();
            });
    }

    public void EvonyFade()
    {
        _canvasGroup.DOFade(1, _fadeDuration)
            .OnStart(() =>
            {
                _canvasGroup.blocksRaycasts = true;
                _tmp.enabled = true;

            }).OnComplete(() =>
            {
                SceneManager.LoadScene((int)ESceneType.Battle);
                FadeOut();
                
            });
    }
}
