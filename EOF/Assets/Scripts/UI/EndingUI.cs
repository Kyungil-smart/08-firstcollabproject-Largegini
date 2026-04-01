using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EndingUI : MonoBehaviour
{
    [SerializeField] private GameObject stageClear;
    [SerializeField] private GameObject stageFail;
    
    [SerializeField] private float fadeInDuration = 1.5f;
    [SerializeField] private float holdDuration   = 2.5f;

    void Start()
    {
        bool isWin = PlayerPrefs.GetInt("BattleResult", 1) == 1;

        SetAlpha(stageClear, 0f);
        SetAlpha(stageFail,  0f);

        stageClear.SetActive(isWin);
        stageFail.SetActive(!isWin);

        GameObject target = isWin ? stageClear : stageFail;
        StartCoroutine(PlaySequence(target));
    }

    IEnumerator PlaySequence(GameObject target)
    {
        target.GetComponent<Image>().DOFade(1f, fadeInDuration).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(fadeInDuration + holdDuration);
    }

    private void SetAlpha(GameObject obj, float alpha)
    {
        if (obj == null) return;
        Image img = obj.GetComponent<Image>();
        if (img == null) return;
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }
}