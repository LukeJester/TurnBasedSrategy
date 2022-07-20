using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BaseAction : MonoBehaviour
{

    public static event EventHandler OnAnyActionStart;
    public static event EventHandler OnAnyActionCompleted;
    public static event EventHandler OnUpdateGridVisual;

    protected Unit unit;
    protected bool isActive;
    protected bool isActionAvailable;
    protected Action onActionComplete;

    protected virtual void Awake() 
    {
        unit = GetComponentInParent<Unit>();
    }

    public abstract string GetActionName();

    public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);

    public virtual bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }

    public abstract List<GridPosition> GetValidActionGridPositionList();

    public virtual int GetActionPointsCost()
    {
        return 1;
    }

    protected void ActionStart(Action onActionComplete)
    {
        this.onActionComplete = onActionComplete;
        isActive = true;

        OnAnyActionStart?.Invoke(this, EventArgs.Empty);
    }

    protected void ActionComplete()
    {
        isActive = false;
        onActionComplete();

        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetUnit()
    {
        return unit;
    }

    public EnemyAIAction GetBestEnemyAIAction()
    {
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();

        List<GridPosition> validActionGridPositionList = GetValidActionGridPositionList();

        foreach(GridPosition gridPosition in validActionGridPositionList)
        {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition);
            enemyAIActionList.Add(enemyAIAction);
        }

        if(enemyAIActionList.Count > 0)
        {
            enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue);
            return enemyAIActionList[0];
        }
        else
        {
            // no possible Enemy AI Action
            return null;
        }
        
    }

    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);

    public abstract ActionGroup GetActionGroup();
}

public enum ActionGroup
{
    Movment,
    Attack,
    Relaod,
    Abilities,
    Item,
    FinalAction
}
//Movment
//walking/ regular movment
//running gives extra movement per action but give a level of tired which lowers accuracy / lowers your dodge (easier to hit) untill your next turn
//jet pack could have the unit move to higher ground like on a roof

// Final Action options are 
// Ambush/Overwatch: Set up an attack that will trigger on the enemy's next turn. Uses all remaining AP, gaining +5% Hit Chance per extra AP spent.
// Defend/HunkerDown: Usable in combat.Hunker down and protect yourself from an attack. Use all remaining AP, gaining +5% Evasion for each spent.
// Prepare/Rest: Usable in combat.End your turn, and carry over up to 2 unused AP to your next turn.
