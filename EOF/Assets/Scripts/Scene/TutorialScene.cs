using UnityEngine.SceneManagement;

public class TutorialScene : IScene
{
    /*
     * 요약 내용 : 튜토리얼 씬 로직
     * 작성자 : 안정연
     */
    public void Enter()
    {
        SceneLoader.Intance.HasTutorial = true;
        SceneManager.LoadScene((int)ESceneType.Tutorial);
    }

    public void Update()
    {
        
    }

    public void Exit()
    {
        
    }
}
