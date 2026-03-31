using UnityEngine;


// 작성자 : 한성우
// 잘 되는지 테스트용 플레이어 컨트롤러


public class PlayerStatController : MonoBehaviour
{
    [Header("입력이 필요한 값")]
    [SerializeField] private int thisID;



    // 테이블 기반 클래스와 플레이어 상속 클래스 가져오기
    PlayerTable table;
    Player thisScript;

    private bool isInitialized = false;




    private void Start()
    {
        InitializePlayer(thisID);
        Debug.Log($"InitializePlayer {thisID} 찾아서 실행 완료");
    }



    public void InitializePlayer(int id)
    {
        Debug.Log("InitializePlayer 입장");


        if (DataManager._instance == null)
        {
            Debug.Log("DataManager._instance == null");
            return;
        }

        table = DataManager._instance.GetPlayerTable();
        thisScript = GetComponent<Player>();   // 플레이어나 플레이어 상속받는 스크립트 찾기
        Debug.Log("Player 찾기 실행");


        // 테이블이 없거나 id 없으면 예외 처리
        if (table == null || !table.PlayerDic.ContainsKey(thisID)) Debug.LogError($"ID {thisID} 플레이어 확인 불가!");
        else
        {
            Debug.Log("if 진행 후 else 실행");
            PlayerData myData = table.PlayerDic[thisID];

            // thisScript. = myData.LocalizeID;
            thisScript._maxHealth = myData.MaxHP;
            Debug.Log($"b : {thisScript._maxHealth}");
            // thisScript._minDamage = myData.Damage_1;

            // thisScript.InitStat();
            Debug.Log("InitStat 실행");
        }




        isInitialized = true;
    }
}
