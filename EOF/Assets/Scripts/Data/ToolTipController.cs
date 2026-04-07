
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;


public class ToolTipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [field: SerializeField] public int SkillID { get; set; }
    [field: SerializeField] public string SkillDescKey { get; set; }
    [field: SerializeField] public GameObject TextObj { get; set; }
    // [field: SerializeField] public GameObject Player { get; set; }

    // 로컬라이즈 테이블 불러오기(한성우)
    private string localeTableName = "LocalTable";


    public void Start()
    {
        Init();
    }



    public void Init()
    {



        // 게임 오브젝트의 텍스트 초기화
        TextObj.GetComponent<TextMeshProUGUI>().text = LocalizationSettings.StringDatabase.GetLocalizedString(localeTableName, SkillDescKey);

        TextObj.SetActive(false);

    }


    // 툴팁 텍스트 On / Off
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (TextObj != null && SkillID != 0)
        {
            TextObj.gameObject.SetActive(true);
        }
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        if (TextObj != null && SkillID != 0)
        {
            TextObj.gameObject.SetActive(false);
        }
    }

    /*
    public void OnPointerClick(PointerEventData eventData)
    {
        if (TextObj != null && SkillID != 0)
        {
            // 켜고 끄기 여부 확인
            bool isCurrentlyActive = TextObj.activeSelf;

            // 꺼져있으면 키고, 켜져있으면 끄기
            TextObj.SetActive(!isCurrentlyActive);
        }
    }

    public void SetAvtive(bool boolSet)
    {
        if (TextObj != null && SkillID != 0)
        {
            TextObj.SetActive(boolSet);
        }
    }
    */
}
