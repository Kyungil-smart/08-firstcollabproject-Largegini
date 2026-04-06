using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


// 작성자 : 한성우
// 이벤트 가 받을 속성을 정의하고, csv 파일에서 읽어온 데이터를 변환해주는 기능을 제공하는 스크립트


public class EventData
{
    public int EventID { get; set; }
    public string EventName { get; set; }
    public string ResourceID { get; set; }
    public EventType EventType { get; set; }
    public int RewardAID { get; set; }
    public int RewardBID { get; set; }


    public void SetEventData(Dictionary<string, object> row)
    {
        EventID = Convert.ToInt32(row["ID"]);
        EventName = Convert.ToString(row["Key"]);
        ResourceID = Convert.ToString(row["ResourceID"]);
        EventType = (EventType)Enum.Parse(typeof(EventType), Convert.ToString(row["EventType"]));
        RewardAID = Convert.ToInt32(row["RewardID_1"]);
        RewardBID = Convert.ToInt32(row["RewardID_2"]);


        // Debug.Log($"a : {HP}");

    }

}
