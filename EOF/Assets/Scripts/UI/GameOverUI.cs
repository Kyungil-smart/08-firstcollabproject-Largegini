using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    public void OnClickRetry()
    {
        SceneLoader.Intance.ChangeScene(SceneLoader.Intance.Battle);
    }

    public void OnClickReturn()
    {
        SceneLoader.Intance.ChangeScene(SceneLoader.Intance.Stage);
    }
}
