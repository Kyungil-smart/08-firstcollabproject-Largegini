using UnityEngine;

public class InGameHUDController : MonoBehaviour
{
    [SerializeField] private ResetConfirmUI resetConfirmUI;
    [SerializeField] private EndTurnConfirmUI endTurnConfirmUI;

    public void OnClickReset()
    {
        resetConfirmUI.Open();
    }

    public void OnClickEndTurn()
    {
        endTurnConfirmUI.Open();
    }
}
