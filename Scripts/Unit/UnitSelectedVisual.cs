using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour
{
    [SerializeField] private Unit unit;

    private MeshRenderer meshRenderer;

    private void Awake() 
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        TurnSystem.Instance.OnTurnChange += TurnSystem_OnTurnChange;

        UpdateVisual();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs Empty)
    {
        UpdateVisual();
    }

    private void TurnSystem_OnTurnChange(object sender, EventArgs Empty)
    {
        UpdateVisualOnTurnChange();
    }

    private void UpdateVisual()
    {
        if (UnitActionSystem.Instance.GetSelectedUnit() == unit)
        {
            meshRenderer.enabled = true;
        }
        else
        {
            meshRenderer.enabled = false;
        }
    }

    private void UpdateVisualOnTurnChange()
    {
        if(!TurnSystem.Instance.IsPlayerTurn()) // if enemy turn turn off the visual
        {
            meshRenderer.enabled = false;
        }
        else if (TurnSystem.Instance.IsPlayerTurn()) // if my turn, turn on selected unit visual
        {
            if (UnitActionSystem.Instance.GetSelectedUnit() == unit)
            {
                meshRenderer.enabled = true;
            }
            else
            {
                meshRenderer.enabled = false;
            }
        }
    }

    private void OnDestroy() // can change this to a subscription to the die even in the health system to be more genaric
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;
        TurnSystem.Instance.OnTurnChange -= TurnSystem_OnTurnChange;
    }
}
