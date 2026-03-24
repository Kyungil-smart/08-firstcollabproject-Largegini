using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseController : MonoBehaviour
{
    public static bool GameIsPaused = false;
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private GameObject settingCanvas;

    public void Start()
    {
        Resume();
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
        if(pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(false);

        Time.timeScale = 1f;
        GameIsPaused = false;

        Debug.Log("게임 재개");
    }

    public void Pause()
    {
        if(pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(true);

        Time.timeScale = 0f;
        GameIsPaused = true;

        Debug.Log("게임 일시정지");
    }

    public void OpenSettings()
    {
        if(pauseMenuCanvas != null)
            settingCanvas.SetActive(true);
        
        Time.timeScale = 0f;
        GameIsPaused = true;
        
        Debug.Log("설정 열기");
    }

    public void CloseSettings()
    {
        if (pauseMenuCanvas != null)
            settingCanvas.SetActive(false);
        
        Debug.Log("설정 닫기");
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoMainMenu()
    {
        SceneManager.LoadScene("Title");
    }
}
