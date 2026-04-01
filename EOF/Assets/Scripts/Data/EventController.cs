using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;


// 작성자 : 한성우
// 이벤트를 구분하여 관리하는 스크립트



public class EventController : MonoBehaviour
{
    // 셋팅해줘야하는 변수
    [Header("셋팅하는 값")]
    [field: SerializeField] public EventType CurrentEventType { get; private set; }

    // 불러올 변수 (테스트 용으로 SerializeField)
    [Header("불러오는 값")]
    // [field: SerializeField] public int EventID { get; private set; }
    // [field: SerializeField] public EventType LoadedEventType { get; private set; }
    public int randomNumber = 0;
    

    EventTable table;   // 불러올 이벤트 테이블
    private List<EventData> targetEvent = new List<EventData>();  // 조건에 맞는 이벤트를 잠시 저장할 테이블


    private void Start()
    {
        Init();

        PickRandomEvent();
    }

    public void Init()
    {
        

        if (DataManager._instance == null)
        {
            Debug.Log("DataManager._instance == null");
            return;
        }

        table = DataManager._instance.GetEventTable();

        // 이전에 저장된 리스트가 있다면 모두 클리어하기
        targetEvent.Clear();
    }



    // 노드 구분에 따라 불러올 랜덤 이벤트 선택
    public void PickRandomEvent()
    {
        

        // 현재 노드가 초반 / 중반 / 후반인지에 따라 그에 맞는 테이블 값 불러오기
        foreach (var element in table.EventDic)
        {
            if(element.Value.EventType == CurrentEventType)
            {
                targetEvent.Add(element.Value);
                Debug.Log($"{element.Value.EventID} 저장");
            }
        }
        // Debug.Log($"저장됨 : {targetEvent[0].EventID}");
        // Debug.Log($"저장됨 : {targetEvent[1].EventID}");
        // Debug.Log($"저장됨 : {targetEvent[2].EventID}");


        // 랜덤 이벤트 중 선택하기
        randomNumber = Random.Range(0, targetEvent.Count);
        Debug.Log($"선택된 숫자 : {randomNumber}, 선택된 ID : {targetEvent[randomNumber].EventID}");


        // 해당 이벤트를 로드하기
        LoadEventChoice(targetEvent[randomNumber]);

    }



    // 이벤트에 따른 선택지 불러오기
    public void LoadEventChoice(EventData target)
    {
        Debug.Log($"{target.EventID} 1번 선택지 대사 : {target.RewardAID}");
        Debug.Log($"{target.EventID} 2번 선택지 대사 : {target.RewardBID}");
    }



    // 유저가 고른 선택지 저장하기
    public void SaveSelectedEvent(int selectedRewardID)
    {
        // 유저가 고른 선택지 



        // 고른 선택지를 데이터에 저장하기



    }

}
