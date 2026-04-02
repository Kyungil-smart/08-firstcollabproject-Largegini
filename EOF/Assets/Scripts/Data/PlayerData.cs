using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


// 작성자 : 한성우
// 플레이어 가 받을 속성을 정의하고, csv 파일에서 읽어온 데이터를 변환해주는 기능을 제공하는 스크립트


public class PlayerData
{
    public int PlayerID { get; set; }
    public float MaxHP { get; set; }
    public float CurrentHP { get; set; }

    public float Damage_Normal { get; set; }
    public float Damage_Special { get; set; }
    public float Shield { get; set; }
    public float Heal { get; set; }
    public int Action { get; set; }
 
    public int MaxGauge { get; set; }
    public float ComboRate { get; set; }

    public float GaugeIncreaseRate { get; set; }
    public float HPAbsorbRate { get; set; }


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

        // 아래는 테이블에는 없음
        GaugeIncreaseRate = 1;
        HPAbsorbRate = 0;

        // Debug.Log($"a : {HP}");

    }
}
