using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadAction : BaseAction
{

    public event EventHandler OnReload;


    [SerializeField] RangedWeapon rangedWeapon;

    private int reloadAPCost;
    private bool hasEnoughAmmoToShoot;

    private void Start()
    {
        reloadAPCost = rangedWeapon.GetReloadAPCost();
    }

    private void Update()
    {
        if (!isActive)
            return;

        onActionComplete();
    }

    public override string GetActionName()
    {
        return "Reload";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        GetIfHasAmmoToShoot();
        int acionCurrentValue;
        if (!hasEnoughAmmoToShoot)
            acionCurrentValue = 1000;
        else
            acionCurrentValue = 0;
            
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = acionCurrentValue,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        if (rangedWeapon.HasFullClip())
            return new List<GridPosition>();  // cant reload a full clip

        GridPosition unitGridPosition = unit.GetGridPosition();

        return new List<GridPosition>
        {
            unitGridPosition
        };
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        rangedWeapon.Reload();

        //FOR RELAOD ANIMATION
        OnReload?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
    }

    private void GetIfHasAmmoToShoot()
    {
        hasEnoughAmmoToShoot = rangedWeapon.HasEnoughAmmoToShoot();
    }
}
