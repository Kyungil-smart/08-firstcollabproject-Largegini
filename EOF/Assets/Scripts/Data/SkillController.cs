using UnityEngine;


// 작성자 : 한성우
// 각종 스킬을 테이블로 밭아서 사용할 수 있도록 만들어주는 스크립트

public class SkillController : MonoBehaviour
{




    [Header("Runtime Data")]
    private SkillDataSO _skillData;



    // 스킬 확률 기능
    


    // 스킬 발동 기능
    public void UseMonsterMainSkill(Monster monster, int skillID)
    {
        // SO 에셋 불러오기
        _skillData = Resources.Load<SkillDataSO>($"DataSO/Skill_{skillID}");
        Debug.Log($"Skills/Skill_{skillID} 불러옴");

        Debug.Log($"{_skillData.Description}");
        // monster._animator.SetTrigger($"{_skillData.Animation}");
        Player.Instance.ReceiveDamage(Random.Range(_skillData.Damage, _skillData.DamageMax));
    }


}
