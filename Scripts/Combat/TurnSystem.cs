using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem Instance { get; private set; }

    public event EventHandler OnTurnChange;

    private int turnNumber = 1;
    private bool isPlayerTurn = true; // this give you the frst turn, make sense for X-Com like game

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more then one TurnSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void NextTurn()
    {
        if (!TurnSystem.Instance.IsPlayerTurn()) // only add to the turn count on My Turn, assumes I always go first
        {
            turnNumber++;
        }
        isPlayerTurn = !isPlayerTurn;

        OnTurnChange?.Invoke(this, EventArgs.Empty);
    }

    public int GetTurnNumber()
    {
        return turnNumber;
    }

    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }
}
