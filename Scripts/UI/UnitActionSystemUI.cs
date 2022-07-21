using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Linq;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtionContainerTransfrom;
    [SerializeField] private TextMeshProUGUI actionPointsText;
    [SerializeField] private Transform groupActionButtionContainerTransfrom;
    [SerializeField] private Transform groupActionButtonPrefab;


    private List<ActionButtionUI> actionButtionUIList;
    private List<GroupActionButtionUI> groupActionButtionUIList;

    private void Awake()
    {
        actionButtionUIList = new List<ActionButtionUI>();
        groupActionButtionUIList = new List<GroupActionButtionUI>();
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

        CreatUnitGroupActionButtons();
        UpdateSelectedVisual();
        UpdateActionButtonVisualForEnemyTurn();

        

        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged;

        GroupActionButtionUI.OnAnyActionGroupSelected += GroupActionButtionUI_OnAnyActionGroupSelected;
    }

    private void CreatUnitActionButtons()
    {
        // foreach(Transform buttonTransform in actionButtionContainerTransfrom)
        // {
        //     Destroy(buttonTransform.gameObject);
        // }

        // actionButtionUIList.Clear();

        // Unit selectedUnit =  UnitActionSystem.Instance.GetSelectedUnit();

        // if (selectedUnit == null)
        //     return;
            
        // foreach(BaseAction baseAction in selectedUnit.GetBaseActionArray())
        // {
        //     Transform actionButtonTransform =  Instantiate(actionButtonPrefab, actionButtionContainerTransfrom);
        //     ActionButtionUI actionButtionUI = actionButtonTransform.GetComponent<ActionButtionUI>();
        //     actionButtionUI.SetBaseAction(baseAction);

        //     actionButtionUIList.Add(actionButtionUI);
        // }
    }

    private void CreatUnitGroupActionButtons()
    {
        foreach (Transform buttonTransform in groupActionButtionContainerTransfrom)
        {
            Destroy(buttonTransform.gameObject);
        }

        groupActionButtionUIList.Clear();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        if (selectedUnit == null)
            return;

        List<BaseAction> actionList = new List<BaseAction>();
        List<ActionGroup> actionGroupList = new List<ActionGroup>();

        foreach (BaseAction baseAction in selectedUnit.GetBaseActionArray())
        {
            actionList.Add(baseAction);
            actionGroupList.Add(baseAction.GetActionGroup());
        }

        actionGroupList = actionGroupList.Distinct().ToList();

        foreach (ActionGroup actionGroup in actionGroupList)
        {
            Transform groupActionButtonTransform = Instantiate(groupActionButtonPrefab, groupActionButtionContainerTransfrom);
            GroupActionButtionUI groupActionButtionUI = groupActionButtonTransform.GetComponent<GroupActionButtionUI>();
            groupActionButtionUI.SetBaseActionGroup(actionGroup);

            foreach (BaseAction baseAction in selectedUnit.GetBaseActionArray())
            {
                if(baseAction.GetActionGroup() == actionGroup)
                {
                    groupActionButtionUI.AddToBaseActionList(baseAction);
                }  
            }

            groupActionButtionUIList.Add(groupActionButtionUI);

        }
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        CreatUnitActionButtons();

        CreatUnitGroupActionButtons();
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
        foreach(GroupActionButtionUI groupActionButtionUI in groupActionButtionUIList)
        {
            groupActionButtionUI.UpdateSelectedVisual();
        }

        // foreach(ActionButtionUI actionButtionUI in actionButtionUIList)
        // {
        //     actionButtionUI.UpdateSelectedVisual();
        // }
    }

    public void UpdateSelectedGroupActionButton()
    {
        foreach (GroupActionButtionUI groupActionButtionUI in groupActionButtionUIList)
        {
            groupActionButtionUI.SetIsSelected();
        }
    }

    private void Show()
    {
        //groupActionButtionContainerTransfrom.gameObject.SetActive(true);
    }

    private void Hide()
    {
        //groupActionButtionContainerTransfrom.gameObject.SetActive(false);
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

    private void GroupActionButtionUI_OnAnyActionGroupSelected(object sender, EventArgs e)
    {
        UpdateSelectedGroupActionButton();
    }

    private void UpdateActionButtonVisualForEnemyTurn()
    {
        groupActionButtionContainerTransfrom.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
        actionPointsText.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
    }
}