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
    [SerializeField] private int maxMoveDistance = 7;

    private List<Vector3> positionList;
    private int curretnPositionIndex;

    private void Update()
    {
        if (!isActive)
            return;

        Vector3 targetPosition = positionList[curretnPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        unit.transform.forward = Vector3.Slerp(unit.transform.forward, moveDirection, rotateSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {

            unit.transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            curretnPositionIndex ++;
            if (curretnPositionIndex >= positionList.Count)
            {
                // for animation
                OnStopMoving?.Invoke(this, EventArgs.Empty);

                ActionComplete();
            }
        }
    }

    public void PlaceHolder()
    {

    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);
        
        curretnPositionIndex = 0;
        positionList = new List<Vector3>();

        foreach(GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }
        
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

                // make each tile movment 1 AP (move one rang) and in the move action show full move range with currnet AP
                // pass current AP for unit into LevelGrid.Instance.IsValidGridPosition(testGridPosition, Unit.GetCurrentAP)
                //and have standered float AP = 1 in the IsValidGridPosition, so it wont mess with other actions
                //maybe in IsValidGridPosition I miltiple gride position by remaning AP?
                if(!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    continue;

                int testDistance = (int)Mathf.Round(Mathf.Sqrt((x * x) + (z * z)));
                //int testDistance = Mathf.Abs(x) + Mathf.Abs(z); // old square patern
                if (testDistance > maxMoveDistance)
                    continue; // make X-COM walk patern

                if (unitGridPosition == testGridPosition)
                    continue; //already standing there

                if(LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                    continue; // area ocupide by unit

                if(!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                    continue; // not walkable tile

                if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition))
                    continue; // has no path to the location

                int pathfindingDistanceMultiplier = 10;
                if (Pathfinding.Instance.GetPathLenth(unitGridPosition, testGridPosition) > maxMoveDistance * pathfindingDistanceMultiplier)
                    continue; // Path length is too long

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
        }; //add logic to seek enemy if out of range
    }
}
