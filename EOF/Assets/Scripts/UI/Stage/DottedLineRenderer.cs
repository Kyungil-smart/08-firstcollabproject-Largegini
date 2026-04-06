using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class DottedLineRenderer : MonoBehaviour
{
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private int dotCount = 10;
    [SerializeField] private float dotSpacing = 0.1f;

    private readonly List<GameObject> _dots = new();

    public void DrawPath(Vector3 from, Vector3 to)
    {
        ClearDots();

        for (int i = 0; i <= dotCount; i++)
        {
            float t = (float)i / dotCount;
            Vector3 pos = Vector3.Lerp(from, to, t);

            var dot = Instantiate(dotPrefab, transform);
            dot.transform.position = pos;
            dot.SetActive(false);
            _dots.Add(dot);
        }
    }
    
    public Sequence RevealDots()
    {
        Sequence seq = DOTween.Sequence();
        foreach (var dot in _dots)
        {
            dot.SetActive(true);
            var img = dot.GetComponent<Image>();
            img.color = new Color(1, 1, 1, 0);
            seq.Append(img.DOFade(1f, 0.03f));
        }
        return seq;
    }

    public void ClearDots()
    {
        foreach (var dot in _dots)
            Destroy(dot);
        _dots.Clear();
    }
}