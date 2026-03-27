using UnityEngine;
using UnityEngine.InputSystem;

public class SceneLoader : MonoBehaviour
{
   /*
    내용 요약 : Scene관리자
    작성자 : 안정연
    */
   private static SceneLoader _instance;
   
   public static SceneLoader Intance { get; private set; }
   
   private SceneMachine _sceneMachine;

   public TitleScene Title;
   public StageScene Stage;
   public BattleScene Battle;
   public EventScene Event;
   
   // 스테이지 진행 정보
   public int StageIndex;
   
   private void Awake()
   {
       if (Intance != null && Intance != this)
       {
           Destroy(gameObject);
           return;
       }
       
       Intance = this;
       DontDestroyOnLoad(gameObject);
       Init();
   }

   private void Start()
   {
       _sceneMachine.ChangeScene(Title);
       // 테스트 코드
       //_sceneMachine.ChangeScene(Stage);
   }

   private void Update()
   {
       _sceneMachine.Update();

       // 디버그
       if(Keyboard.current.pKey.isPressed)
           ChangeScene(Stage);
       
       if(Keyboard.current.oKey.isPressed)
           ChangeScene(Title);
   }

   public void ChangeScene(IScene scene)
   {
       _sceneMachine.ChangeScene(scene);
   }
   
   private void Init()
   {
       _sceneMachine = new SceneMachine();
       
       Title = new TitleScene();
       Stage = new StageScene();
       Battle = new BattleScene();
       Event = new EventScene();

       StageIndex = 0;
   }
}
