using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StageUI : MonoBehaviour
{
    [SerializeField] private GameObject settingCanvas;
    [SerializeField] private GameObject eventPopup;

    [SerializeField] public GameObject[] NodeBtns;
    [SerializeField] private GameObject[] eventPopups;
    [SerializeField] private TMP_Text[] eventPopupTexts;
    
    [SerializeField] private Button[] closeButtons;
    [SerializeField] private Button[] nextButtons;
    
   
    

    private ColorBlock _btnActiveColor;
    private int _eventIndex = 0;

    private string[] _eventText=
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
        foreach (GameObject btn in NodeBtns)
        {
            LockBtn(btn);
        }
        
        if(SceneLoader.Intance.StageIndex < NodeBtns.Length)
            UnLockBtn(NodeBtns[SceneLoader.Intance.StageIndex]);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (settingCanvas != null && settingCanvas.activeSelf)
            {
                CloseSettings();
            }
        }
    }

    public void OnClickBattleNode()
    {
        SceneLoader.Intance.ChangeScene(SceneLoader.Intance.Battle);
    }
    
    public void OnClickEventNode()
    {
        _eventIndex = 0;
        
        foreach (var btn in closeButtons)
            btn.gameObject.SetActive(false);
    
        //eventPopup.SetActive(true);
        UpdateEventPopup();
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
        eventPopups[_eventIndex].SetActive(false);
        //SceneLoader.Intance.ChangeScene(SceneLoader.Intance.Event);
    }
    private void UpdateEventPopup()
    {
        foreach (var popup in eventPopups)
            popup.SetActive(false);

        eventPopups[_eventIndex].SetActive(true);
        eventPopupTexts[_eventIndex].text = _eventText[_eventIndex];

        bool isLast = _eventIndex == _eventText.Length - 1;

        for (int i = 0; i < closeButtons.Length; i++)
        {
            closeButtons[i].gameObject.SetActive(isLast && i == _eventIndex);
            nextButtons[i].gameObject.SetActive(!isLast && i == _eventIndex);
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
        
        Debug.Log("설정 닫기");
    }

    private void LockBtn(GameObject btn)
    {
        btn.GetComponent<Button>().interactable = false;
        ColorBlock colorBlock = btn.GetComponent<Button>().colors;
        colorBlock.normalColor = Color.gray2;
        btn.GetComponent<Button>().colors = colorBlock;
    }
    
    private void UnLockBtn(GameObject btn)
    {
        btn.GetComponent<Button>().interactable = true;
        btn.GetComponent<Button>().colors = _btnActiveColor;
    }
}
