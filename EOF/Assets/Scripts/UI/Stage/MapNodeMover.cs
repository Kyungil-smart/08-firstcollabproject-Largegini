using UnityEngine;
using DG.Tweening;

public class MapNodeMover : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DottedLineRenderer dottedLine;

    [Header("Settings")]
    [SerializeField] private float moveDelay = 0.5f;
    [SerializeField] private float moveDuration = 0.4f;

    public void DrawVisitedPath(RectTransform fromNode, RectTransform toNode)
    {
        dottedLine.DrawPathImmediate(fromNode.position, toNode.position);
    }

    public void PlayMoveToNextNode(RectTransform fromNode, RectTransform toNode, System.Action onComplete)
    {
        Debug.Log($"from.position={fromNode.position}, to.position={toNode.position}");
        dottedLine.DrawPath(fromNode.position, toNode.position);
    
        Sequence seq = DOTween.Sequence();
    
        seq.Append(dottedLine.RevealDots());
    
        seq.AppendInterval(moveDelay);
    
        seq.Append(fromNode.DOScale(0.8f, moveDuration).SetEase(Ease.InBack));
    
        seq.Append(toNode.DOScale(1.2f, moveDuration).SetEase(Ease.OutBack));
        seq.Append(toNode.DOScale(1.0f, 0.2f));
    
        seq.AppendCallback(() => onComplete?.Invoke());
    }
}