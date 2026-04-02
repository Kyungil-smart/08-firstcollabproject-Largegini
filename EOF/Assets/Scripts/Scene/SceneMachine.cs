using UnityEngine;

public class SceneMachine
{
    /*
        내용 요약 : 씬 전환
        작성자 : 안정연
     */
    private IScene _currentScene = null;

    public void ChangeScene(IScene scene)
    {
        if(_currentScene == scene) return;
        
        _currentScene?.Exit();
        _currentScene = scene;
        _currentScene.Enter();
    }

    public void Update()
    {
        _currentScene?.Update();
    }
}
