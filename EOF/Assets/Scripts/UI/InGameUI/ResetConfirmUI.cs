using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class ResetConfirmUI : MonoBehaviour
{
    [SerializeField] private Button btnResetConfirm;
    [SerializeField] private Button btnNo;
    
    [SerializeField] private ActionBarUI actionBarUI;
    
    public UnityEvent onResetConfirmed;

    private void Awake()
    {
        btnResetConfirm.onClick.AddListener(OnClickYes);
        btnNo.onClick.AddListener(OnClickNo);
    }

    public void Open() => gameObject.SetActive(true);

    private IEnumerator OpenNextFrame()
    {
        yield return null;
        gameObject.SetActive(true);
    }
    private void OnClickYes()
    {
        if (Player.Instance._behavior < 1)
        {
            OnClickNo();
            return;
        }

        Player.Instance._behavior--;
        actionBarUI.SetAP(Player.Instance._behavior, Player.Instance._maxbehavior);
        onResetConfirmed?.Invoke();
        gameObject.SetActive(false);
    }

    private void OnClickNo() => gameObject.SetActive(false);
}