using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.AddressableAssets; // 어드레서블 사용 시 추가 필요


public class StageUI : MonoBehaviour
{
    [SerializeField] private GameObject settingCanvas;

    [SerializeField] public GameObject[] NodeBtns;

    [SerializeField] private GameObject eventPopup;
    [SerializeField] private TMP_Text eventPopupText;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    [Header("선택지")]
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private TMP_Text[] choiceTexts;

    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private GameObject imageBG;
    [SerializeField] private GameObject imageEvent;
    [SerializeField] private Button skipButton;

    [Header("노드이동")]
    [SerializeField] private MapNodeMover nodeMover;
    [SerializeField] private RectTransform tutorialNode;
    
    private int currentIndex;
    private StoryPage[] pages;

    // TODO: ScriptableObject 연결 시 아래 주석 해제 후 _eventText 배열 제거
    // [SerializeField] private EventDataSO eventData;

    private ColorBlock _btnActiveColor;
    private int _eventIndex = 0;

    // 이벤트 팝업용으로 추가 (한성우)
    private EventController eventController;
    [field: SerializeField] public int SelectedIndex { get; set; }  // 인덱스 선택용

    private string[] _eventText =
    {
        "이벤트 설명 1",
        "이벤트 설명 2",
        "마지막 설명"
    };



    private void ShowPage(int index)
    {
        var page = pages[index];

        descriptionText.text = page.text;
        // image.sprite = page.image;
        

        bool isChoice = page.hasChoice;

        descriptionText.gameObject.SetActive(!isChoice);
        
        choicePanel.SetActive(isChoice);

        if (isChoice)
        {
            for (int i = 0; i < choiceButtons.Length; i++)
            {
                int capturedIndex = i;
                choiceTexts[i].text = page.choiceTexts[i];
                choiceButtons[i].interactable = true;

                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() =>
                    OnClickChoice(page, capturedIndex));
            }
        }
        skipButton.gameObject.SetActive(!isChoice);

        bool isLast = index == pages.Length - 1;
        bool isFirst = index == 0;
        skipButton.gameObject.SetActive(!isChoice && !isLast);
        prevButton.gameObject.SetActive(!isFirst && !isChoice && !isLast);
        closeButton.gameObject.SetActive(isLast && !isChoice);
        nextButton.gameObject.SetActive(!isLast && !isChoice);
    }
    public void OnClickSkip()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i].hasChoice)
            {
                currentIndex = i;
                ShowPage(currentIndex);
                return;
            }
        }
    }
    private void OnClickChoice(StoryPage page, int choiceIndex)
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            choiceButtons[i].interactable = (i == choiceIndex);
        }

        int nextPage = page.choiceNextIndex[choiceIndex];

        StartCoroutine(MoveToPageAfterDelay(nextPage));
    }
    private IEnumerator MoveToPageAfterDelay(int nextIndex)
    {
        yield return new WaitForSeconds(0.5f);
        currentIndex = nextIndex;
        ShowPage(currentIndex);
    }

    private void Awake()
    {
        _btnActiveColor = NodeBtns[0].GetComponent<Button>().colors;

        // 이벤트 팝업용 이벤트 컨트롤러 초기화 (한성우)
        if (eventController == null) eventController = new EventController();
        SelectedIndex = 0;

        // 이벤트 팝업을 위해 추가 (한성우)
        eventController.ActivateEventPopUp();

        // 주소로 아이콘 게임 오브젝트 불러오기 (한성우) 
        // https://wolstar.tistory.com/14 기반
        Addressables.LoadAssetAsync<GameObject>(eventController.EventImageAddress).Completed += (op) =>
        {
            // 로드가 완료 시 실행
            imageEvent = op.Result;

            if (imageEvent != null)
            {
                GameObject instanceImg = Instantiate(imageEvent, imageBG.transform);
            }

            // 예외 처리
            else
            {
                Debug.LogError($"에셋 로드 실패: {eventController.EventImageAddress}");
            }
        };



        /*
        imageEvent = Resources.Load<GameObject>($"EventImagePrefabs/{eventController.EventImageAddress}");
        // Debug.Log($"이벤트 프리펩 주소 : EventImagePrefabs/{eventController.EventImageAddress}");
        
        if (imageEvent != null)
        {
            GameObject instanceImg = Instantiate(imageEvent, imageBG.transform);
            
            RectTransform rect = instanceImg.GetComponent<RectTransform>();
            // 크기 및 위치 초기화
            
            if (rect != null)
            {
                rect.localPosition = Vector3.zero;
                rect.localScale = Vector3.one;
            }
            
        }
        */

        // Todo: 데이터연결 예정, 임시 테스트 데이터
        pages = new StoryPage[]
        {
            new StoryPage { text = eventController.EventText0101, hasChoice = false },
            // new StoryPage { text = "이벤트 설명 2", hasChoice = false },
            new StoryPage
            {
                text = "",
                hasChoice = true,
                choiceTexts = new string[] { eventController.EventText0201, eventController.EventText0202 },
                choiceNextIndex = new int[] { 2, 2 }
            },
            // new StoryPage { text = eventController.EventText0301, hasChoice = false },
            new StoryPage { text = "", hasChoice = false },
        };
        // 선택지 버튼 호버 애니메이션
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            var btn = choiceButtons[i];

            var trigger = btn.GetComponent<EventTrigger>()
                          ?? btn.gameObject.AddComponent<EventTrigger>();

            var enterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enterEntry.callback.AddListener(_ => btn.transform.DOScale(1.05f, 0.1f));

            var exitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            exitEntry.callback.AddListener(_ => btn.transform.DOScale(1f, 0.1f));

            trigger.triggers.Add(enterEntry);
            trigger.triggers.Add(exitEntry);
        }
    }

    private void Start()
    {
        SceneLoader.Intance.MaxStage = NodeBtns.Length - 1;
        foreach (GameObject btn in NodeBtns)
        {
            LockBtn(btn);
        }

        if (SceneLoader.Intance.HasTutorial && SceneLoader.Intance.StageIndex == 0)
        {
            UnLockBtn(NodeBtns[0]);
            nodeMover.PlayMoveToNextNode(tutorialNode, NodeBtns[0].GetComponent<RectTransform>(), null);
        }
        else
        {
            if (SceneLoader.Intance.StageIndex < NodeBtns.Length)
                UnLockBtn(NodeBtns[SceneLoader.Intance.StageIndex]);
        }
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (settingCanvas != null && settingCanvas.activeSelf)
                CloseSettings();
        }
    }

    public void OnClickEventRewardSelect(int index)
    {
        eventController.SaveSelectedEvent(index);
        
        pages[2].text = eventController.EventText0301;
        pages[2].hasChoice = false;

    }


    public void OnClickBattleNode()
    {
        SceneLoader.Intance.ChangeScene(SceneLoader.Intance.Battle);
    }

    public void OnClickEventNode()
    {
        eventPopup.SetActive(true);
        _eventIndex = 0;
        ShowPage(currentIndex);

        // 이벤트 팝업을 위해 추가 (한성우)
        // eventController.ActivateEventPopUp();
    }
    public void OnClickEventPrev()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowPage(currentIndex);
        }
    }

    public void OnClickEventNext()
    {
        if (currentIndex < pages.Length - 1)
        {
            currentIndex++;
            ShowPage(currentIndex);
        }
    }

    public void OnClickEventClose()
    {
        eventPopup.SetActive(false);
        SceneLoader.Intance.StageIndex += 1;

        int unlockIndex = SceneLoader.Intance.StageIndex - 1;

        if (unlockIndex - 1 >= 0)
            LockBtn(NodeBtns[unlockIndex - 1]);

        UnLockBtn(NodeBtns[unlockIndex]);

        RectTransform fromNode = unlockIndex == 0
            ? tutorialNode
            : NodeBtns[unlockIndex - 1].GetComponent<RectTransform>();

        RectTransform toNode = NodeBtns[unlockIndex].GetComponent<RectTransform>();
        nodeMover.PlayMoveToNextNode(fromNode, toNode, null);
    }

    private void UpdateEventPopup()
    {
        Debug.Log("UpdateEventPopup");

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


        // 이벤트 팝업을 위해 추가 (한성우)
        eventController.ActivateEventPopUp();

        /*
        if (eventPopup.GetComponent<EventController>() == null)
        {
            eventPopup.AddComponent<EventController>();
            
            if (SceneLoader.Intance.StageIndex == 0)
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
            

        }*/


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
[System.Serializable]
public class StoryPage
{
    public Sprite image;
    public string text;

    public bool hasChoice;
    public string[] choiceTexts;
    public int[] choiceNextIndex;
}