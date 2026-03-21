using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HPBar : MonoBehaviour
{
    [SerializeField] int currentHP;
    [SerializeField] int maxHP;
    
    public RectTransform hpBarfill;
    public TMP_Text hpText;
    
    float maxWidth;

    private void Start()
    {
        maxWidth = hpBarfill.parent.GetComponent<RectTransform>().rect.width;
        hpBarfill.sizeDelta = new Vector2(maxWidth, hpBarfill.sizeDelta.y);
        Debug.Log("maxWidth" + maxWidth);
    }
    
    private void Update()
    {
        
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
