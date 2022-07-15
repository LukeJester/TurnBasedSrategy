using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{

    public event EventHandler OnThrowGrenade;

    [SerializeField] private Transform grenadeProjectilePrefab;

    private int maxThrowDistance = 5;

    public override string GetActionName()
    {
        return "Grenade";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition UnitGridPosition = unit.GetGridPosition();

        for (int x = -maxThrowDistance; x <= maxThrowDistance; x++)
        {
            for (int z = -maxThrowDistance; z <= maxThrowDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = UnitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    continue;

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxThrowDistance)
                    continue; // make circul range

                if (UnitGridPosition == testGridPosition)
                    continue; //already standing there

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        //will first instaniate it in hand position with grenade consumable / all consumables will be spawned in right hand
        Transform greandeProjectileTransform =  Instantiate(grenadeProjectilePrefab, unit.GetWorldPosition(), Quaternion.identity);
        // will throw grenade after short delay so it goes in mid throw animation
        GrenadeProjectile grenadeProjectile = greandeProjectileTransform.GetComponent<GrenadeProjectile>();

        OnThrowGrenade?.Invoke(this, EventArgs.Empty); // not used, but will be when we add throwing greande animation

        grenadeProjectile.Setup(gridPosition, OnGrenadeBehaviorComplete);

        ActionStart(onActionComplete);
    }

    private void OnGrenadeBehaviorComplete()
    {
        ActionComplete();
    }
}
