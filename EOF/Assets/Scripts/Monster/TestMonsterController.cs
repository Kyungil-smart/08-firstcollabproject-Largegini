using UnityEngine;


// 작성자 : 한성우
// 잘 되는지 테스트용 단 한 마리의 몬스터 컨트롤러(추후 삭제해야함)
namespace Test
{
    public class TestMonsterController : MonoBehaviour
    {
        [Header("입력이 필요한 값")]
        [SerializeField] private int thisID = 101;

        [Header("자동으로 불러들이는 값 예시")]
        [SerializeField] private string thisLocalizeID;
        [SerializeField] private int thisMaxHP;
        [SerializeField] private int thisDamage;

        void Start()
        {
            InitializeMonster();
        }

        private void InitializeMonster()
        {

            MonsterTable table = DataManager._instance.GetMonsterTable();

            // 테이블이 없거나 id 없으면 예외 처리
            if (table == null || !table.MonsterDic.ContainsKey(thisID)) Debug.LogError($"ID {thisID} 몬스터 확인 불가!");

            else
            {
                MonsterData myData = table.MonsterDic[thisID];

                thisLocalizeID = myData.LocalizeID;
                thisMaxHP = myData.HP;
                thisDamage = myData.Damage_1;
            }
                
                
                
                
                
        }
    }
}