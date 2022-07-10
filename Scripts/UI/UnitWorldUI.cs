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
    [SerializeField] HealthSystem healthSystem;

    private void Start()
    {
        //this will be called every time any units AP changes due to OnAnyActionPointsChanged in Unit being static
        Unit.OnAnyActionPointsChanged += unit_OnAnyActionPointsChanged;
        healthSystem.OnDamaged += healthSystem_OnDamaged;

        UpdateActionPointsText();
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthBarImage.fillAmount = healthSystem.GetHealthNormilized();
    }

    private void UpdateActionPointsText()
    {
        actionPointsText.text = unit.GetActionPoints().ToString();
    }

    private void unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }

    private void healthSystem_OnDamaged(object sender, EventArgs e)
    {
        UpdateHealthBar();
    }
}

