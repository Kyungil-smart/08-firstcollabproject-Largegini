using UnityEngine;

// 작성자 : 한성우
// 보상 의 메인 타입을 정의하는 enum 스크립트


public enum ERewardType
{
    None = 0,
    Player_Stat = 1,    // 벨류 2번만 사용하는 스텟 (같은 종류 보상 중복 획득 가능)
    Player_Stat_2 = 2,  // 밸류 1, 2번 모두 사용하는 스텟 (같은 종류 보상 중복 획득 가능)
    Player_Skill =3,    // 스킬 추가하는 스텟 (스킬 On/ Off 개념이라 같은 종류 보상 중복 획득 불가)
    Player_HPRecovery = 4,  // 일회성 회복
}
