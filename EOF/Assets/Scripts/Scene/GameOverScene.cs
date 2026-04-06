using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScene : IScene
{
    /*
     * 요약 내용 : 게임오버 씬 로직
     * 작성자 : 안정연
     */
    public void Enter()
    {
        SceneLoader.Intance.Fade.ChangeScene(ESceneType.GameOver);
    }

    public void Update()
    {
        
    }

    public void Exit()
    {
        
    }
}
