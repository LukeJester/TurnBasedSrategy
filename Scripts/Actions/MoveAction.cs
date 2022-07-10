using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveAction : BaseAction
{
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;

    //movment variables
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float rotateSpeed = 10f;
    [SerializeField] float stoppingDistance = 0.1f;
    [SerializeField] private int maxMoveDistance = 4;

    private Vector3 targetPosition;

    protected override void Awake()
    {
        base.Awake();
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (isActive)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;

            if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
            {

                unit.transform.position += moveDirection * moveSpeed * Time.deltaTime;
            }
            else
            {

                // for animation
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                
                ActionComplete();
            }

            unit.transform.forward = Vector3.Slerp(unit.transform.forward, moveDirection, rotateSpeed * Time.deltaTime);
        }
    }

    public void PlaceHolder()
    {

    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        this.targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        // for animation
        OnStartMoving?.Invoke(this , EventArgs.Empty);

        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for(int x = -maxMoveDistance; x<= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x,z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                
                if(!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    continue;

                if (unitGridPosition == testGridPosition)
                    continue; //already standing there

                if(LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                    continue; // area ocupide by unit

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Move";
    }

    public override int GetActionPointsCost()
    {
        return 1;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
         int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);
        
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10,
        };
    }
}
