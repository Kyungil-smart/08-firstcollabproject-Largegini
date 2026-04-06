using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using DG.Tweening;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

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
        if(op.Status == AsyncOperationStatus.Succeeded)
            _canvasGroup = op.Result.GetComponent<CanvasGroup>();
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
                //StartCoroutine("LoadScene", (int)sceneType); /// 씬 로드 코루틴 실행 ///
            });
    }

    //public GameObject Loading;
    //public Text Loading_text; //퍼센트 표시할 텍스트

    IEnumerator LoadScene(int sceneType)
    {
        //Loading.SetActive(true); //로딩 화면을 띄움

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneType);
        async.allowSceneActivation = false; //퍼센트 딜레이용

        float past_time = 0;
        float percentage = 0;

        while(!(async.isDone)){
            yield return null;

            past_time += Time.deltaTime;

            if(percentage >= 90){
                percentage = Mathf.Lerp(percentage, 100, past_time);

                if(percentage == 100){
                    async.allowSceneActivation = true; //씬 전환 준비 완료
                }
            }
            
            else
            {
                percentage = Mathf.Lerp(percentage, async.progress * 100f, past_time);
                if(percentage >= 90) past_time = 0;
            }
            //Loading_text.text = percentage.ToString("0") + "%"; //로딩 퍼센트 표기
        }
    }
}
