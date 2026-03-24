using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SettingManager : MonoBehaviour
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
}
