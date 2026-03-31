using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
// 작성자 : 홍정옥
// 기능 : 공격시 HP바 및 HP 텍스트 감소
public class PlayerHPBar : MonoBehaviour
{
    [SerializeField] float currentHP;
    [SerializeField] float maxHP;
    
    public RectTransform hpBarfill;
    public TMP_Text hpText;
    
    float maxWidth;

    private void Start()
    {
        StartCoroutine(WaitForPlayer());
    }
    

    IEnumerator WaitForPlayer()
    {
        yield return new WaitUntil((() => Player.Instance != null && Player.Instance._maxHealth > 0));
        
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
            canvas.worldCamera = Camera.main;
        
        maxHP = Player.Instance._maxHealth;
        currentHP = maxHP;
        maxWidth = hpBarfill.parent.GetComponent<RectTransform>().rect.width;
        hpBarfill.sizeDelta = new Vector2(maxWidth, hpBarfill.sizeDelta.y);
        hpText.text = $"{currentHP}/{maxHP}";
        Debug.Log("maxWidth" + maxWidth);
    }
    private void Update()
    {
        if (maxWidth > 0)
        {
            currentHP = Player.Instance._health;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);
            hpText.text = $"{currentHP}/{maxHP}";

            float ratio = (float)currentHP / maxHP;
            hpBarfill.DOSizeDelta(new Vector2(maxWidth * ratio, hpBarfill.sizeDelta.y), 0.4f);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        hpText.text = $"{currentHP}/{maxHP}";
        
        float ratio = (float)currentHP / maxHP;
        hpBarfill.DOSizeDelta(new Vector2(maxWidth * ratio, hpBarfill.sizeDelta.y), 0.4f);
    }
}
