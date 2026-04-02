using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StageUI : MonoBehaviour
{
    [SerializeField] private GameObject settingCanvas;

    [SerializeField] public GameObject[] NodeBtns;

    [SerializeField] private GameObject eventPopup;
    [SerializeField] private TMP_Text eventPopupText;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    
    // TODO: ScriptableObject 연결 시 아래 주석 해제 후 _eventText 배열 제거
    // [SerializeField] private EventDataSO eventData;

    private ColorBlock _btnActiveColor;
    private int _eventIndex = 0;

    private string[] _eventText =
    {
        "이벤트 설명 1",
        "이벤트 설명 2",
        "마지막 설명"
    };

    private void Awake()
    {
        _btnActiveColor = NodeBtns[0].GetComponent<Button>().colors;
    }

    private void Start()
    {
        SceneLoader.Intance.MaxStage = NodeBtns.Length - 1;
        foreach (GameObject btn in NodeBtns)
        {
            LockBtn(btn);
        }

        if (SceneLoader.Intance.StageIndex < NodeBtns.Length)
            UnLockBtn(NodeBtns[SceneLoader.Intance.StageIndex]);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (settingCanvas != null && settingCanvas.activeSelf)
                CloseSettings();
        }
    }

    public void OnClickBattleNode()
    {
        SceneLoader.Intance.ChangeScene(SceneLoader.Intance.Battle);
    }

    public void OnClickEventNode()
    {
        _eventIndex = 0;
        UpdateEventPopup();
    }
    public void OnClickEventPrev()
    {
        if (_eventIndex > 0)
        {
            _eventIndex--;
            UpdateEventPopup();
        }
    }
    
    public void OnClickEventNext()
    {
        if (_eventIndex < _eventText.Length - 1)
        {
            _eventIndex++;
            UpdateEventPopup();
        }
    }

    public void OnClickEventClose()
    {
        eventPopup.SetActive(false);
        SceneLoader.Intance.StageIndex += 1;
        LockBtn(NodeBtns[SceneLoader.Intance.StageIndex - 1]);
        UnLockBtn(NodeBtns[SceneLoader.Intance.StageIndex]);
    }

    private void UpdateEventPopup()
    {
        eventPopup.SetActive(true);
        
        // TODO: SO 연결시 아래줄로 교체[ㅎㅏ기
        // eventPopupText.text = eventData.texts[_eventIndex];
        eventPopupText.text = _eventText[_eventIndex];

        bool isLast = _eventIndex == _eventText.Length - 1;
        // TODO: SO 연결 시 _eventText.Length -> eventData.texts.Length로교체
        
        bool isFirst = _eventIndex == 0;
        prevButton.gameObject.SetActive(!isFirst);
        closeButton.gameObject.SetActive(isLast);
        nextButton.gameObject.SetActive(!isLast);


        // 기능 확인을 위해 임시로 추가 (한성우)
        eventPopup.AddComponent<EventController>(); 

        if(SceneLoader.Intance.StageIndex == 0)
        {
            eventPopup.GetComponent<EventController>().CurrentEventType = EventType.EventFirst;
        }
        else if (SceneLoader.Intance.StageIndex == 2)
        {
            eventPopup.GetComponent<EventController>().CurrentEventType = EventType.EventSecond;
        }
        else if (SceneLoader.Intance.StageIndex == 4)
        {
            eventPopup.GetComponent<EventController>().CurrentEventType = EventType.EventThired;
        }
    }

    public void OnClickSettings()
    {
        settingCanvas.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingCanvas != null)
            settingCanvas.SetActive(false);
    }

    private void LockBtn(GameObject btn)
    {
        btn.GetComponent<Button>().interactable = false;
        ColorBlock colorBlock = btn.GetComponent<Button>().colors;
        colorBlock.normalColor = Color.gray;
        btn.GetComponent<Button>().colors = colorBlock;
    }

    private void UnLockBtn(GameObject btn)
    {
        btn.GetComponent<Button>().interactable = true;
        btn.GetComponent<Button>().colors = _btnActiveColor;
    }
}