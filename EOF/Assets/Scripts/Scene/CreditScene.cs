using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditScene : IScene
{
    public void Enter()
    {
        SceneManager.LoadScene((int)ESceneType.EndCredit);
    }

    public void Update()
    {
        
    }

    public void Exit()
    {
        
    }
}
