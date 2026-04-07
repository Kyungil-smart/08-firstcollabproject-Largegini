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
    
    private readonly List<List<GameObject>> _allPaths = new();
    
    private readonly List<Coroutine> _flowCoroutines = new();

    private Vector3 GetBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }

    public void DrawPath(Vector3 from, Vector3 to)
    {
        List<GameObject> pathDots = new();

        Vector3 mid = (from + to) / 2f + Vector3.up * curveHeight;

        for (int i = 0; i <= dotCount; i++)
        {
            float t = (float)i / dotCount;
            Vector3 pos = GetBezierPoint(t, from, mid, to);

            GameObject dot = Instantiate(dotPrefab, transform);
            RectTransform rect = dot.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.sizeDelta = new Vector2(dotSize, dotSize);
                rect.position = pos;
            }

            Image img = dot.GetComponent<Image>();
            if (img != null)
                img.color = new Color(dotColor.r, dotColor.g, dotColor.b, 0f);

            pathDots.Add(dot);
        }

        _allPaths.Add(pathDots);
    }
    
    public void DrawPathImmediate(Vector3 from, Vector3 to, float alpha = 0.35f)
    {
        List<GameObject> pathDots = new();

        Vector3 mid = (from + to) / 2f + Vector3.up * curveHeight;

        for (int i = 0; i <= dotCount; i++)
        {
            float t = (float)i / dotCount;
            Vector3 pos = GetBezierPoint(t, from, mid, to);

            GameObject dot = Instantiate(dotPrefab, transform);
            dot.transform.position = pos;

            RectTransform rect = dot.GetComponent<RectTransform>();
            if (rect != null)
                rect.sizeDelta = new Vector2(dotSize, dotSize);

            Image img = dot.GetComponent<Image>();
            if (img != null)
                img.color = new Color(dotColor.r, dotColor.g, dotColor.b, alpha);

            pathDots.Add(dot);
        }

        _allPaths.Add(pathDots);
    }

    public Sequence RevealDots()
    {
        if (_allPaths.Count == 0)
            return DOTween.Sequence();

        List<GameObject> latestPath = _allPaths[_allPaths.Count - 1];
        return RevealDots(latestPath);
    }

    public Sequence RevealDots(List<GameObject> dots)
    {
        Sequence seq = DOTween.Sequence();

        foreach (GameObject dot in dots)
        {
            if (dot == null) continue;

            Image img = dot.GetComponent<Image>();
            if (img == null) continue;

            seq.Append(img.DOFade(1f, flowSpeed));
        }

        seq.OnComplete(() =>
        {
            Coroutine flow = StartCoroutine(FlowAnimation(dots));
            _flowCoroutines.Add(flow);
        });

        return seq;
    }

    private IEnumerator FlowAnimation(List<GameObject> dots)
    {
        while (true)
        {
            for (int i = 0; i < dots.Count; i++)
            {
                if (dots[i] == null) continue;

                Image img = dots[i].GetComponent<Image>();
                if (img == null) continue;

                img.DOFade(maxAlpha, pulseDuration * 0.5f)
                   .OnComplete(() =>
                   {
                       if (img != null)
                           img.DOFade(minAlpha, pulseDuration * 0.5f);
                   });

                yield return new WaitForSeconds(flowSpeed);
            }

            yield return null;
        }
    }

    public void ClearDots()
    {
        foreach (Coroutine flow in _flowCoroutines)
        {
            if (flow != null)
                StopCoroutine(flow);
        }
        _flowCoroutines.Clear();

        foreach (List<GameObject> path in _allPaths)
        {
            foreach (GameObject dot in path)
            {
                if (dot == null) continue;

                Image img = dot.GetComponent<Image>();
                if (img != null)
                    DOTween.Kill(img);

                Destroy(dot);
            }
        }

        _allPaths.Clear();
    }
}