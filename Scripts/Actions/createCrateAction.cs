using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createCrateAction : BaseAction
{
    [SerializeField] private int maxPlacmentDistance = 3;
    [SerializeField] Transform CratePrefab;

    private void Update()
    {
        if (!isActive)
            return;

        ActionComplete();
    }

    public override string GetActionName()
    {
        return "Create Cover";
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

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxPlacmentDistance; x <= maxPlacmentDistance; x++)
        {
            for (int z = -maxPlacmentDistance; z <= maxPlacmentDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    continue;

                if (unitGridPosition == testGridPosition)
                    continue; //already standing there

                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                    continue; // area ocupide by unit

                if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                    continue; // not walkable tile


                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        Vector3 cratePlacmentPosition = LevelGrid.Instance.GetWorldPosition(gridPosition); ;
        
        Transform greandeProjectileTransform = Instantiate(CratePrefab, cratePlacmentPosition, Quaternion.identity);

        ActionStart(onActionComplete);
    }

    private void OnCreatPlacment()
    {
        ActionComplete();
    }
}
