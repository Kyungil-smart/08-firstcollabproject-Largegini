using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

// 작성자 : 홍정옥
// 기능 : 타이틀 메뉴 버튼 연결 및 설정창 제어 스크립트

public class TitleManager : MonoBehaviour
{
    [SerializeField] private GameObject settingCanvas;
    
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
        SceneLoader.Intance.ChangeScene(SceneLoader.Intance.Stage);
    }
    
    public void OnClickSettings()
    {
        settingCanvas.SetActive(true);
    }
    
    public void CloseSettings()
    {
        if (settingCanvas != null)
            settingCanvas.SetActive(false);
        
        Debug.Log("설정 닫기");
    }

    public void OnClickExit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
