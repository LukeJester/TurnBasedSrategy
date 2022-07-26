using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitManager : MonoBehaviour
{

    public static UnitManager Instance { get; private set; }

    public static event EventHandler OnAnyUnitAddedToUnitManager;
    public static event EventHandler OnAnyUnitRemovedFromUnitManager;

    private List<Unit> unitList;
    private List<Unit> frendlyUnitList;
    private List<Unit> enemyUnitList;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more then one UnitManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        unitList = new List<Unit>();
        frendlyUnitList = new List<Unit>();
        enemyUnitList = new List<Unit>();
    }

    private void Start()
    {
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
    }

    private void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;

        unitList.Add(unit);

        if(unit.IsEnemy())
        {
            enemyUnitList.Add(unit);
        }
        else
        {
            frendlyUnitList.Add(unit);
            OnAnyUnitAddedToUnitManager?.Invoke(this, EventArgs.Empty);
        }

    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;

        unitList.Remove(unit);

        if (unit.IsEnemy())
        {
            enemyUnitList.Remove(unit);
        }
        else
        {
            frendlyUnitList.Remove(unit);
            OnAnyUnitRemovedFromUnitManager?.Invoke(this, EventArgs.Empty);
        }

    }

    public List<Unit> GetUnitList()
    {
        return unitList;
    }

    public List<Unit> GetFriendlyUnitList()
    {
        return frendlyUnitList;
    }

    public List<Unit> GetEnemyUnitList()
    {
        return enemyUnitList;
    }
}
