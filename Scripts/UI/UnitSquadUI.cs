using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSquadUI : MonoBehaviour
{

    [SerializeField] private Transform unitButtonPrefab;
    [SerializeField] private Transform unitSquadButtionContainerTransfrom;

    private List<UnitButtionUI> unitButtionUIList;


    void Awake()
    {
        unitButtionUIList = new List<UnitButtionUI>();
    }

    private void Start()
    {
        
        UnitManager.OnAnyUnitAddedToUnitManager += UnitManager_OnAnyUnitAddedToUnitManager;
        UnitManager.OnAnyUnitRemovedFromUnitManager += UnitManager_OnAnyUnitRemovedFromUnitManager;
        UpdateUnitSquadButtons();
    }

    private void UpdateUnitSquadButtons()
    {
        foreach (Transform buttonTransform in unitSquadButtionContainerTransfrom)
        {
            Destroy(buttonTransform.gameObject);
        }

        unitButtionUIList.Clear();

        List<Unit> friendlyUnitList = UnitManager.Instance.GetFriendlyUnitList();

        foreach (Unit unit in friendlyUnitList)
        {
            Transform unitButtonTransform = Instantiate(unitButtonPrefab, unitSquadButtionContainerTransfrom);
            UnitButtionUI unitButtionUI = unitButtonTransform.GetComponent<UnitButtionUI>();
            unitButtionUI.SetBaseUnit(unit);

            unitButtionUIList.Add(unitButtionUI);
        }
    }

    private void UnitManager_OnAnyUnitAddedToUnitManager(object sender, EventArgs e)
    {
        UpdateUnitSquadButtons();
    }

    private void UnitManager_OnAnyUnitRemovedFromUnitManager(object sender, EventArgs e)
    {
        UpdateUnitSquadButtons();
    }
}
