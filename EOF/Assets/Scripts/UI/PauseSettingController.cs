using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
// 작성자 : 홍정옥
// 기능 : 일시정지 및 설정 버튼 제어
public class PauseSettingController : MonoBehaviour
{
    public static bool GameIsPaused = false;
    
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject settingCanvas;
    [SerializeField] private GameObject goMainConfirmPopup;

    public void Start()
    {
        Resume();
        goMainConfirmPopup.SetActive(false);
    }
    public void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (settingCanvas != null && settingCanvas.activeSelf)
            {
                CloseSettings();
            }
            else if(GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        if(pauseCanvas != null)
            pauseCanvas.SetActive(false);
        if (settingCanvas != null)
            settingCanvas.SetActive(false);

        Time.timeScale = 1f;
        GameIsPaused = false;

        Debug.Log("게임 재개");
    }

    public void Pause()
    {
        if(pauseCanvas != null)
            pauseCanvas.SetActive(true);

        Time.timeScale = 0f;
        GameIsPaused = true;

        Debug.Log("게임 일시정지");
    }

    public void OpenSettings()
    {
        if(pauseCanvas != null)
            settingCanvas.SetActive(true);
        
        Time.timeScale = 0f;
        GameIsPaused = true;
        
        Debug.Log("설정 열기");
    }

    public void CloseSettings()
    {
        if (pauseCanvas != null)
            settingCanvas.SetActive(false);
        
        Debug.Log("설정 닫기");
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoMainMenu()
    {
        Debug.Log("GoMainMenu 호출됨");
        Debug.Log("goMainConfirmPopup: " + goMainConfirmPopup);
        goMainConfirmPopup.SetActive(true);
    }
    
    public void OnclickConfirmYes()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneLoader.Intance.ChangeScene(SceneLoader.Intance.Title);
        SceneLoader.Intance.StageIndex = 0;
        DataManager._instance.firstInitSave();
    }

    public void OnClickConfirmNo()
    {
        goMainConfirmPopup.SetActive(false);
    }
}
