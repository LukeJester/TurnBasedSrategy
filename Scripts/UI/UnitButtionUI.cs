using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UnitButtionUI : MonoBehaviour
{

    [SerializeField] private Button button;
    [SerializeField] private Image UnitFace;
    [SerializeField] private GameObject selectedGameObject;
    [SerializeField] private TextMeshProUGUI unitAP;
    [SerializeField] private TextMeshProUGUI unitAmmo;
    [SerializeField] private Image healthBarImage;

    private Unit unit;
    private HealthSystem healthSystem;
    private RangedWeapon rangedWeapon;
    private ShootAction shootAction;
    private OverwatchAction overwatchAction;
    private ReloadAction reloadAction;

    private bool isSelected;

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
    }

    public void SetBaseUnit(Unit newUnit)
    {
        unit = newUnit;
        UnitFace.sprite = unit.GetUnitsFace();
        unitAP.text = "AP: " + unit.GetActionPoints() + "/" + unit.GetMaxActionPoints();
        rangedWeapon = unit.GetComponentInChildren<RangedWeapon>();
        unitAmmo.text = "Ammo: " + rangedWeapon.GetCurrentAmmo() + "/" + rangedWeapon.GetMaxAmmo();

        button.onClick.AddListener(() => { OnUIUnitClick(); });

        healthSystem = unit.GetComponent<HealthSystem>();
        healthSystem.OnDamaged += healthSystem_OnDamaged;

        shootAction = unit.GetComponentInChildren<ShootAction>();
        shootAction.OnShoot += shootAction_OnShoot;

        overwatchAction = unit.GetComponentInChildren<OverwatchAction>();
        overwatchAction.OnShoot += overwatchAction_OnShoot;

        reloadAction = unit.GetComponentInChildren<ReloadAction>();
        reloadAction.OnReload += reloadAction_OnReload;

        UpdateSelectedVisual();
    }

    

    private void OnUIUnitClick()
    {
        if (isSelected)
            return;
        
        UnitActionSystem.Instance.SetSelectedUnit(unit);
        UpdateSelectedVisual();


        HandelUnitOutOfCameraView();
    }

    private void HandelUnitOutOfCameraView()
    {
        // Vector3 viewPos = Camera.main.WorldToViewportPoint(unit.transform.position);
        // if (Mathf.Abs(viewPos.x)  > 1F || Mathf.Abs(viewPos.y) > 1F)
        //     Debug.Log("Move Camera");
    }

    private void SetIsSelected()
    {
        this.isSelected = false;
    }

    private void UpdateSelectedVisual()
    {
        if (UnitActionSystem.Instance.GetSelectedUnit() == unit)
        {
            selectedGameObject.SetActive(true);
            isSelected = true;
        }
        else
        {
            if (selectedGameObject != null)
                selectedGameObject.SetActive(false);
            isSelected = false;
        }
    }

    private void UpdateAP(Unit updatingUnit)
    {
        if (updatingUnit == unit)
            unitAP.text = "AP: " + updatingUnit.GetActionPoints() + "/" + updatingUnit.GetMaxActionPoints();
    }

    private void UpdateHealthBar()
    {
        healthBarImage.fillAmount = healthSystem.GetHealthNormilized();
    }

    private void UpdateAmmo()
    {
        unitAmmo.text = "Ammo: " + rangedWeapon.GetCurrentAmmo() + "/" + rangedWeapon.GetMaxAmmo();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        UpdateSelectedVisual();
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        Unit updatingUnit = (Unit)sender;
        UpdateAP(updatingUnit);
    }

    private void healthSystem_OnDamaged(object sender, EventArgs e)
    {
        HealthSystem healthSystem = (HealthSystem)sender;
        if (healthSystem == this.healthSystem)
            UpdateHealthBar();
    }

    private void shootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        Unit shootingUnit = e.shootingUnit;
        if (shootingUnit == unit)
            UpdateAmmo();
    }

    private void overwatchAction_OnShoot(object sender, OverwatchAction.OnShootEventArgs e)
    {
        Unit shootingUnit = e.shootingUnit;
        if (shootingUnit == unit)
            UpdateAmmo();
    }

    private void reloadAction_OnReload(object sender, EventArgs e)
    {
        ReloadAction reloadAction = (ReloadAction)sender;
        if (reloadAction == this.reloadAction)
            UpdateAmmo();
    }

    private void OnDestroy()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;
        Unit.OnAnyActionPointsChanged -= Unit_OnAnyActionPointsChanged;

        if (healthSystem != null)
            healthSystem.OnDamaged -= healthSystem_OnDamaged;
    }
}
