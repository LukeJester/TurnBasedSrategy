using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtionContainerTransfrom;
    [SerializeField] private TextMeshProUGUI actionPointsText;

    private List<ActionButtionUI> actionButtionUIList;

    private void Awake()
    {
        actionButtionUIList = new List<ActionButtionUI>();
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnSelectedUnitChanged;
        TurnSystem.Instance.OnTurnChange += TurnSystem_OnTurnChange;
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;

        UpdateActionPoints();
        CreatUnitActionButtons();
        UpdateSelectedVisual();
        UpdateActionButtonVisualForEnemyTurn();

        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged;
    }

    private void CreatUnitActionButtons()
    {
        foreach(Transform buttonTransform in actionButtionContainerTransfrom)
        {
            Destroy(buttonTransform.gameObject);
        }

        actionButtionUIList.Clear();

        Unit selectedUnit =  UnitActionSystem.Instance.GetSelectedUnit();

        if (selectedUnit == null)
            return;
            
        foreach(BaseAction baseAction in selectedUnit.GetBaseActionArray())
        {
            Transform actionButtonTransform =  Instantiate(actionButtonPrefab, actionButtionContainerTransfrom);
            ActionButtionUI actionButtionUI = actionButtonTransform.GetComponent<ActionButtionUI>();
            actionButtionUI.SetBaseAction(baseAction);

            actionButtionUIList.Add(actionButtionUI);
        }
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        CreatUnitActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateSelectedVisual();
    }

    private void UnitActionSystem_OnActionStarted(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void UpdateSelectedVisual()
    {
        foreach(ActionButtionUI actionButtionUI in actionButtionUIList)
        {
            actionButtionUI.UpdateSelectedVisual();
        }
    }

    private void Show()
    {
        actionButtionContainerTransfrom.gameObject.SetActive(true);
    }

    private void Hide()
    {
        actionButtionContainerTransfrom.gameObject.SetActive(false);
    }

    private void UnitActionSystem_OnBusyChanged(object sender, bool isBusy)
    {
        if (isBusy)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    private void UpdateActionPoints()
    {
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        actionPointsText.text = "Action Points: " + selectedUnit.GetActionPoints() + "/" + selectedUnit.GetMaxActionPoints();
    }

    private void TurnSystem_OnTurnChange(object sender, EventArgs e)
    {
        UpdateActionPoints();
        UpdateActionButtonVisualForEnemyTurn();
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void UpdateActionButtonVisualForEnemyTurn()
    {
        actionButtionContainerTransfrom.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
        actionPointsText.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
    }
}
