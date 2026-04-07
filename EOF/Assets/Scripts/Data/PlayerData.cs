using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


// 작성자 : 한성우
// 플레이어 가 받을 속성을 정의하고, csv 파일에서 읽어온 데이터를 변환해주는 기능을 제공하는 스크립트


public class PlayerData
{
    // 식별자
    public int PlayerID { get; set; }

    // Table_Player 스텟
    public float MaxHP { get; set; }
    public float CurrentHP { get; set; }    // 테이블에는 없지만 초기화에선 필요해서 추가
    public float Damage_Normal { get; set; }
    public float Damage_Special { get; set; }
    public float Shield { get; set; }
    public float Heal { get; set; }
    public int Action { get; set; }
    public int MaxGauge { get; set; }
    public float ComboRate { get; set; }
    public float GaugeIncreaseRate { get; set; }
    public float HPAbsorbRate { get; set; }

    // 이벤트용 스킬(테이블에는 없음)
    public bool SkillChain01 { get; set; }
    public bool SkillChain02 { get; set; }
    public bool Rejuvenate { get; set; }
    public bool Bulwark { get; set; }
    public bool Onslaught01 { get; set; }
    public bool Onslaught02 { get; set; }
    public bool Resurrection { get; set; }

    // 스킬 ID 저장용 리스트
    public List<int> GetSkillIDs { get; set; } = new List<int>();


    public void SetPlayerData(Dictionary<string, object> row)
    {
        PlayerID = Convert.ToInt32(row["ID"]);

        MaxHP = Convert.ToSingle(row["HP"]);

        Damage_Normal = Convert.ToSingle(row["Damage_Normal"]);
        Damage_Special = Convert.ToSingle(row["Damage_Special"]);
        Shield = Convert.ToSingle(row["Shield"]);
        Heal = Convert.ToSingle(row["Heal"]);

        Action = Convert.ToInt32(row["Action"]);
        MaxGauge = Convert.ToInt32(row["MaxGauge"]);
        ComboRate = Convert.ToSingle(row["ComboRate"]);

        GaugeIncreaseRate = Convert.ToInt32(row["GaugeIncreaseRate"]); ;
        HPAbsorbRate = Convert.ToInt32(row["HPAbsorbRate"]); ;


        // 스킬
        SkillChain01 = false;
        SkillChain02 = false;
        Rejuvenate = false;
        Bulwark = false;
        Onslaught01 = false;
        Onslaught02 = false;
        Resurrection = false;

        GetSkillIDs.Clear();    // 얻은 스킬 초기화

        // Debug.Log($"a : {HP}");

    }
}
