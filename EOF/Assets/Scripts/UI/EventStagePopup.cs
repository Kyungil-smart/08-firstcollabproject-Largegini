using UnityEngine;
using UnityEngine.SceneManagement;

public class EventStagePopup : MonoBehaviour
{
    public void OpenPopup()
    {
        gameObject.SetActive(true);
    }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
    public void EnterEventStage()
    {
        SceneManager.LoadScene("EventStage");
    }
}
