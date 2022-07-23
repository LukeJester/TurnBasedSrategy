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
    private float reloadTimer = 0;


    private void Start()
    {
        reloadAPCost = rangedWeapon.GetReloadAPCost();
    }

    private void Update()
    {
        if (!isActive)
            return;

        reloadTimer += Time.deltaTime;

        if (reloadTimer >= 1.5f )
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
        reloadTimer = 0;

        rangedWeapon.Reload();

        //FOR RELAOD ANIMATION
        OnReload?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
    }

    public override ActionGroup GetActionGroup()
    {
        return ActionGroup.Relaod;
    }

    private void GetIfHasAmmoToShoot()
    {
        hasEnoughAmmoToShoot = rangedWeapon.HasEnoughAmmoToShoot();
    }
}
