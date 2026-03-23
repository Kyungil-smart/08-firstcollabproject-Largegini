using System;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    private BattleTurn _battle;

    private void Start()
    {
        _battle = BattleTurn.pTurn;
    }
}

public enum BattleTurn
{
    pTurn,
    eTurn,
}
