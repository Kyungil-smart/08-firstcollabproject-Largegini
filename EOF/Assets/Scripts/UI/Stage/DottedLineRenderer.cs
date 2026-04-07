using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DottedLineRenderer : MonoBehaviour
{
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private int dotCount = 20;
    [SerializeField] private float curveHeight = 80f;

    [Header("흐르는 애니메이션")]
    [SerializeField] private float flowSpeed = 0.05f;
    [SerializeField] private float pulseDuration = 0.6f;
    [SerializeField] private float minAlpha = 0.2f;
    [SerializeField] private float maxAlpha = 1.0f;
    
    [SerializeField] private Color dotColor = new Color(1f, 0f, 0f, 1f);
    [SerializeField] private float dotSize = 15f;

    private readonly List<GameObject> _dots = new();
    private Coroutine _flowCoroutine;

    private Vector3 GetBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }

    public void DrawPath(Vector3 from, Vector3 to)
    {
        //ClearDots();
        Vector3 mid = (from + to) / 2f + Vector3.up * curveHeight;

        for (int i = 0; i <= dotCount; i++)
        {
            float t = (float)i / dotCount;
            Vector3 pos = GetBezierPoint(t, from, mid, to);

            var dot = Instantiate(dotPrefab, transform);
            dot.transform.position = pos;
            
            var rect = dot.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(dotSize, dotSize);
            
            var img = dot.GetComponent<Image>();
            img.color = new Color(dotColor.r, dotColor.g, dotColor.b, 0f);

            _dots.Add(dot);
        }
    }
    
    public Sequence RevealDots()
    {
        Sequence seq = DOTween.Sequence();
        foreach (var dot in _dots)
        {
            var img = dot.GetComponent<Image>();
            seq.Append(img.DOFade(1f, flowSpeed));
        }
        
        seq.OnComplete(() => _flowCoroutine = StartCoroutine(FlowAnimation()));

        return seq;
    }
    
    private IEnumerator FlowAnimation()
    {
        while (true)
        {
            for (int i = 0; i < _dots.Count; i++)
            {
                int index = i; // 캡처
                var img = _dots[index].GetComponent<Image>();
                
                img.DOFade(maxAlpha, pulseDuration * 0.5f)
                   .OnComplete(() => img.DOFade(minAlpha, pulseDuration * 0.5f));

                yield return new WaitForSeconds(flowSpeed);
            }
            yield return null;
        }
    }

    public void ClearDots()
    {
        if (_flowCoroutine != null)
        {
            StopCoroutine(_flowCoroutine);
            _flowCoroutine = null;
        }

        foreach (var dot in _dots)
        {
            DOTween.Kill(dot.GetComponent<Image>());
            Destroy(dot);
        }
        _dots.Clear();
    }
}