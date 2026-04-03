using UnityEngine;
using UnityEngine.InputSystem;

// 작성자 : 홍정옥
// 기능 : 타이틀 메뉴 버튼 연결 및 설정창 제어 스크립트

public class TitleManager : MonoBehaviour
{
    [SerializeField] private GameObject settingCanvas;
    [SerializeField] private GameObject exitConfirmUI;
    
    public void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (settingCanvas != null && settingCanvas.activeSelf)
            {
                CloseSettings();
            }
        }
    }
    public void OnClickStart()
    {
        if(SceneLoader.Intance.HasTutorial)
            SceneLoader.Intance.ChangeScene(SceneLoader.Intance.Stage);
        
        SceneLoader.Intance.ChangeScene(SceneLoader.Intance.Tutorial);
    }
    
    public void OnClickSettings()
    {
        settingCanvas.SetActive(true);
    }
    
    public void CloseSettings()
    {
        if (settingCanvas != null)
            settingCanvas.SetActive(false);
    }

    public void OnClickExit()
    {
        exitConfirmUI.SetActive(true);
    }

    public void OnClickExitConfirm()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnclickExitCancel()
    {
        exitConfirmUI.SetActive(false);
    }
}
