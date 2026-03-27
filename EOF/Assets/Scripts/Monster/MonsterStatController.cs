using UnityEditor;  // 유니티 에디터에서 사용하기 위해 필요
using UnityEngine;


// 작성자 : 한성우
// 잘 되는지 테스트용 단 한 마리의 몬스터 컨트롤러(추후 삭제해야함)

// [ExecuteInEditMode]
public class MonsterStatController : MonoBehaviour
{
    [Header("입력이 필요한 값")]
    [SerializeField] private int thisID;

    /*
    [Header("자동으로 불러들이는 값 예시")]
    [SerializeField] private string thisLocalizeID;
    [SerializeField] private int thisMaxHP;
    [SerializeField] private int thisDamage;
    */

    // 테이블 기반 클래스와 몬스터 상속 클래스 가져오기
    MonsterTable table;
    Monster thisScript;

    private bool isInitialized = false;




    private void Start()
    {
        // if (DataManager._instance.GetMonsterTable() != null) table = DataManager._instance.GetMonsterTable();
        InitializeMonster(thisID);
    }


    private void Update()
    {
        /*
        if (!isInitialized)
        {
            InitializeMonster(thisID);
        }
        */
    }

    public void InitializeMonster(int id)
    {

        if (DataManager._instance == null)
        {
            Debug.Log("DataManager._instance == null");
            return;
        }

        table = DataManager._instance.GetMonsterTable();
        thisScript = GetComponent<Monster>();   // 몬스터나 몬스터 상속받는 스크립트 찾기



        // 테이블이 없거나 id 없으면 예외 처리
        if (table == null || !table.MonsterDic.ContainsKey(thisID)) Debug.LogError($"ID {thisID} 몬스터 확인 불가!");

        // 


        else
        {
            MonsterData myData = table.MonsterDic[thisID];

            // thisScript. = myData.LocalizeID;
            thisScript._tableMaxHP = myData.HP;
            // Debug.Log($"b : {thisScript._tableMaxHP}");
            // thisScript._minDamage = myData.Damage_1;

            thisScript.InitStat();
            // Debug.Log("InitStat 실행");
        }




        isInitialized = true;
    }
}
