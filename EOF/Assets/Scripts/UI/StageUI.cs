using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class StageUI : MonoBehaviour
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

    public void OnClickBattleNode()
    {
        SceneLoader.Intance.ChangeScene(SceneLoader.Intance.Battle);
    }
    
    public void OnClickEventNode()
    {
        SceneLoader.Intance.ChangeScene(SceneLoader.Intance.Event);
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
