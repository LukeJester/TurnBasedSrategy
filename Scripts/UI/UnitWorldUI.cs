using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionPointsText;
    [SerializeField] private Unit unit;
    [SerializeField] Image healthBarImage;
    [SerializeField] TextMeshProUGUI hitPercentText;
    [SerializeField] HealthSystem healthSystem;

    private void Start()
    {
        //this will be called every time any units AP changes due to OnAnyActionPointsChanged in Unit being static
        Unit.OnAnyActionPointsChanged += unit_OnAnyActionPointsChanged;
        healthSystem.OnDamaged += healthSystem_OnDamaged;
        healthSystem.OnDead += healthSystem_OnDead;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;

        UpdateActionPointsText();
        UpdateHealthBar();

        HideHitPercent();
    }

    private void UpdateHealthBar()
    {
        healthBarImage.fillAmount = healthSystem.GetHealthNormilized();
    }

    private void UpdateActionPointsText()
    {
        actionPointsText.text = unit.GetActionPoints().ToString();
    }

    private void ShowHitPercent(float hitChance)
    {
        hitPercentText.gameObject.SetActive(true);
        hitPercentText.text = "HIT: " + Mathf.Round(hitChance * 100f) + "%";
    }

    private void HideHitPercent()
    {
        hitPercentText.gameObject.SetActive(false);
    }

    private void unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }

    private void healthSystem_OnDamaged(object sender, EventArgs e)
    {
        UpdateHealthBar();
    }

    private void healthSystem_OnDead(object sender, EventArgs e)
    {
        UnitActionSystem.Instance.OnSelectedActionChanged -= UnitActionSystem_OnSelectedActionChanged;
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, UnitActionSystem.OnSelectedAction e)
    {
        // See if action needs to show Hit Percent
        HideHitPercent();

        string actionName = e.unitAction.GetActionName();

        if (unit.IsEnemy() != e.unit.IsEnemy())
        {
            if (actionName == "Shoot")
            {
                ShootAction shootAction = e.unit.GetAction<ShootAction>();
                if (shootAction.GetValidActionGridPositionList().Contains(unit.GetGridPosition()))
                {
                    // This unit us on a valid shoot position
                    ShowHitPercent(shootAction.GetHitPercent(unit));
                }
            }

            if (actionName == "Melee")
            {
                MeleeAction meleeAction = e.unit.GetAction<MeleeAction>();
                if (meleeAction.GetValidActionGridPositionList().Contains(unit.GetGridPosition()))
                {
                    // This unit us on a valid shoot position
                    ShowHitPercent(meleeAction.GetHitPercent(unit));
                }
            }
        }      
    }

}

