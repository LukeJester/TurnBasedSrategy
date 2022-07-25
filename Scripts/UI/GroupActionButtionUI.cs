using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class GroupActionButtionUI : MonoBehaviour
{

    public static event EventHandler OnAnyActionGroupSelected;
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private Button button;

    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtionContainerTransfrom;


    private ActionGroup actionGroup;
    private List<BaseAction> baseActionList;
    private List<ActionButtionUI> actionButtionUIList;
    private CanvasGroup canvasGroup;

    private bool isSelected;


    private void Awake()
    {
        actionButtionUIList = new List<ActionButtionUI>();
        baseActionList = new List<BaseAction>();
        canvasGroup = GetComponentInParent<CanvasGroup>();
    }

    public void SetBaseActionGroup(ActionGroup actionGroup)
    {
        this.actionGroup = actionGroup;
        textMeshPro.text = actionGroup.ToString();

        button.onClick.AddListener(() =>{ OnUIGroupButtonClick(); });
    }

    private void OnUIGroupButtonClick()
    {
        if (canvasGroup.alpha == 0 )
            return;

        if(isSelected)
        {
            SetIsSelected();
            return;
        }
            

        OnAnyActionGroupSelected?.Invoke(this, EventArgs.Empty);

        DisplayActionsInThisGroup();
        isSelected = true;
    }

    private void DisplayActionsInThisGroup()
    {
        foreach (Transform buttonTransform in actionButtionContainerTransfrom)
        {
            Destroy(buttonTransform.gameObject);
        }

        actionButtionUIList.Clear();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        if (selectedUnit == null)
            return;

        foreach (BaseAction baseAction in baseActionList)
        {
            Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtionContainerTransfrom);
            ActionButtionUI actionButtionUI = actionButtonTransform.GetComponent<ActionButtionUI>();
            actionButtionUI.SetBaseAction(baseAction);

            actionButtionUIList.Add(actionButtionUI);
        }
        
    }

    public void HideActionsInThisGroup()
    {
        foreach (Transform buttonTransform in actionButtionContainerTransfrom)
        {
            Destroy(buttonTransform.gameObject);
        }

        actionButtionUIList.Clear();

    }

    public void AddToBaseActionList(BaseAction baseAction)
    {
        baseActionList.Add(baseAction);
    }

    public void SetIsSelected()
    {
        this.isSelected = false;
        HideActionsInThisGroup();
    }

    public void UpdateSelectedVisual()
    {
        foreach(ActionButtionUI actionButtionUI in actionButtionUIList)
        {
            actionButtionUI.UpdateSelectedVisual();
        }
    }


}
