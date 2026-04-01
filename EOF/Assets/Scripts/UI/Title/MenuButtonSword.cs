using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 작성자 : 홍정옥
// 기능 : 버튼 호버시 칼 표시
public class MenuButtonSword : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject swordIcon;

    void Start()
    {
        if (swordIcon != null)
            swordIcon.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (swordIcon != null)
            swordIcon.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (swordIcon != null)
            swordIcon.SetActive(false);
    }
}