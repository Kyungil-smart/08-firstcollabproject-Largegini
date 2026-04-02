using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScene : IScene
{
    public void Enter()
    {
        SceneManager.LoadScene((int)ESceneType.GameOver);
    }

    public void Update()
    {
        
    }

    public void Exit()
    {
        
    }
}
