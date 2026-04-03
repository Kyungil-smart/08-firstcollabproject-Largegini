using Unity.VisualScripting;
using UnityEngine;

public class RewardSkillController : MonoBehaviour
{
    Player player;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        player = GetComponent<Player>();
    }



    // 스킬 발동 기능 모음
    //  생명력 % 이햐 스킬 스텟 발동 기능
    public void ActivateStatSkillProcess(float currentHPpercent, bool skill01, bool skill02, bool skill03, bool skill04)    // 기능 받는거 player 로 변경 필요
    {
        CheckCurrentHealth();
    }

    //  특정 % 추가 데미지 주는 기능
    public void GiveExtraDamage(float dmg, bool skill01, bool skill02)    // 기능 받는거 player 로 변경 필요
    {
        
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



    // 현재 체력을 받아서 스킬 조건을 체크하는 스크립트
    public void CheckCurrentHealth()
    {

    }



    public void UseSkillChain01()
    {
        // 기능 추가 필요
    }


    public void UseSkillChain02()
    {
        // 기능 추가 필요
    }


    public void UseRejuvenate()
    {
        // 기능 추가 필요
    }


    public void UseBulwark()
    {
        // 기능 추가 필요
    }


    public void UseOnslaught01()
    {
        // 기능 추가 필요
    }


    public void UseOnslaught02()
    {
        // 기능 추가 필요
    }


    public void UseResurrection()
    {
        // 기능 추가 필요
    }


}
