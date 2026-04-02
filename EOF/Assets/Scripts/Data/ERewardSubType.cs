using UnityEngine;

// 작성자 : 한성우
// 보상 의 서브 타입을 정의하는 enum 스크립트


public enum ERewardSubType
{
    // 사용 안함
    None = 0,

    // Player_Stat 에 사용
    Attack_Value = 1,
    Defence_Value = 2,
    Recovery_Value = 3,
    Special_Value = 4,
    Gauge_Value = 5,
    Drain_Value = 6,


    // 아래는 아직 미구현

    // Player_Stat_2 에 사용
    Inverse_Value,  // 공격력 상승, 쉴드량 하락
    Counterpoise_Value, // 생명력 % 하락, 특수 게이지 상승력 증가
    Interchange_Value,


    // Player_Skill 에 사용
    Chain_Value01,
    Chain_Value02,
    Rejuvenate_Value,
    Bulwark_Value,
    Onslaught_Value01,
    Onslaught_Value02,
    Resurrection_Value,


    // 아직 미분류








}
