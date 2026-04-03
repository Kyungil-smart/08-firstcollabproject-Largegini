using Unity.VisualScripting;
using UnityEngine;

public class RewardSkillController : MonoBehaviour
{
    Player player;
    RewardTable table;   // 불러올 이벤트 보상 테이블 (추후 기능 변경 시 필요)


    private float currentHPpercent;   // 현재 체력 퍼센트


    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        player = GetComponent<Player>();
        table = new RewardTable();
    }



    // 스킬 발동 기능 모음
    //  생명력 % 이햐 스킬 스텟 발동 기능
    public void ActivateStatSkillProcess()
    {
        // 현재 체력 퍼센트 계산
        CheckCurrentHealthPer();


        // 조건에 따른 스킬 발동 여부 계산
        if (player.Rejuvenate == true && currentHPpercent <= 0.5f)
        {
            UseRejuvenate(true);
            Debug.Log($"Rejuvenate 발동");
        }
        else UseRejuvenate(false);


        if (player.Bulwark == true && currentHPpercent >= 0.5f)
        {
            UseBulwark(true);
            Debug.Log($"Bulwark 발동");
        }
        else UseBulwark(false);


        if (player.Onslaught01 == true && currentHPpercent <= 0.3f)
        {
            UseOnslaught01(true);
            Debug.Log($"Onslaught01 발동");
        }
        else UseOnslaught01(false);


        if (player.Onslaught02 == true && currentHPpercent <= 0.4f)
        {
            UseOnslaught02(true);
            Debug.Log($"Onslaught02 발동");
        }
        else UseOnslaught02(false);

    }

    //  특정 % 추가 데미지 주는 기능
    public float GiveExtraDamage(float dmg)
    {
        int randA = 0;
        int randB = 0;

        float dmgBonus = 0f;

        if (player.SkillChain01 == true)
        {
            randA = Random.Range(1, 101);

            if (randA <= 30)    // 30% 확률로 추가 데미지
            {
                dmgBonus += 0.5f;
                Debug.Log($"SkillChain01 발동 : {dmgBonus * 100} % 만큼의 추가 데미지");
            }
        }
        if (player.SkillChain02 == true)
        {
            randB = Random.Range(1, 101);

            if(randB <= 100)    // 100% 확률로 추가 데미지 (확률 변동 가능성 있어서 일단 넣어놓음)
            {
                dmgBonus += 0.1f;
                Debug.Log($"SkillChain02 발동 : {dmgBonus * 100} % 만큼의 추가 데미지");
            }
        }

        dmg = dmg * (1 + dmgBonus);

        return dmg;
    }

    //  부활 기능
    // BattleSystem 에 기능이 있어서 부활은 해당 스크립트에서 수정
    /*
    public void ActivateResurrection()
    {
        // 전투 당 첫 번째 죽음이라면 부활 발동
        if(player._isFirstDeath == true)
        {
            // 캐릭터 애니메이션을 추가해야할 수도 있음

            player._health = player._maxHealth * 0.5f;
        }

        
    }
    */



    // 현재 체력을 비율로 전환하는 기능
    public void CheckCurrentHealthPer()
    {
        currentHPpercent = player._health / player._maxHealth;
        Debug.Log($"현재 체력 퍼센트 : {currentHPpercent * 100} %");
    }



    public void UseSkillChain01()
    {
        // 기능 추가 필요
    }


    public void UseSkillChain02()
    {
        // 기능 추가 필요
    }


    public void UseRejuvenate(bool onOff)
    {
        if (onOff) player.AddHeal = 2;
        else player.AddHeal = 0;
    }


    public void UseBulwark(bool onOff)
    {
        if (onOff) player.AddDefensive = 2;
        else player.AddDefensive = 0;
    }


    public void UseOnslaught01(bool onOff)
    {
        if (onOff) player.AddAttack = 2;
        else player.AddAttack = 0;
    }


    public void UseOnslaught02(bool onOff)
    {
        if (onOff) player.AddGaugeIncreaseRate = 1;
        else player.AddGaugeIncreaseRate = 0;
    }

    /*
    public void UseResurrection()
    {
        // 기능 추가 필요
    }
    */

}
