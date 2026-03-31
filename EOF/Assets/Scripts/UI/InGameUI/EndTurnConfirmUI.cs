using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class EndTurnConfirmUI : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button btnConfirm;
    [SerializeField] private Button btnCancel;

    [Header("이벤트")]
    public UnityEvent onEndTurnConfirmed;

    private void Awake()
    {
        btnConfirm.onClick.AddListener(OnClickConfirm);
        btnCancel.onClick.AddListener(OnClickCancel);
    }

    public void Open() => gameObject.SetActive(true);
    
    private IEnumerator OpenNextFrame()
    {
        yield return null;
        gameObject.SetActive(true);
    }

    private void OnClickConfirm()
    {
        gameObject.SetActive(false);
        onEndTurnConfirmed?.Invoke();
    }

    private void OnClickCancel() => gameObject.SetActive(false);
}