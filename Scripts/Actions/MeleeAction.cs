using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAction : BaseAction
{
    public static event EventHandler OnAnyMelleHit;

    //for Melle Animations
    public event EventHandler OnMeleeActionStarted;
    public event EventHandler OnMeleeActionCompleted;

    private enum State
    {
        SwingingWeaponBeforeHit,
        SwingingWeaponAfterHit,
    }

    [SerializeField] MeleeWeapon meleeWeapon;

    private int maxMeleeDistance;
    private int meleeDamage;
    private int weaponAPCost;
    private State state;
    private float stateTimer;
    private Unit targetUnit;

    private void Start()
    {
        maxMeleeDistance = meleeWeapon.GetWeaponRange();
        meleeDamage = meleeWeapon.GetWeaponDamage();
        weaponAPCost = meleeWeapon.GetAPCost();
    }

    private void Update()
    {
        if(!isActive)
            return;

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.SwingingWeaponBeforeHit:
                Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                
                float rotateSpeed = 10f;
                unit.transform.forward = Vector3.Slerp(unit.transform.forward, aimDirection, rotateSpeed * Time.deltaTime);
                break;
            case State.SwingingWeaponAfterHit:
                break;
        }

        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    private void NextState()
    {
        switch (state)
        {
            case State.SwingingWeaponBeforeHit: 
                state = State.SwingingWeaponAfterHit;
                float afterHitStateTime = 0.1f;
                stateTimer = afterHitStateTime;
                targetUnit.Damage(meleeDamage);
                OnAnyMelleHit?.Invoke(this, EventArgs.Empty);
                break;
            case State.SwingingWeaponAfterHit:
                OnMeleeActionCompleted?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;
        }
    }

    public override string GetActionName()
    {
        return "Swoard";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction // set to hit closest thing in range
        {
            gridPosition = gridPosition,
            actionValue = 200, // settting high means we will always use this is possible
        };
    }

    public override int GetActionPointsCost()
    {
        return weaponAPCost;
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition UnitGridPosition = unit.GetGridPosition();

        for (int x = -maxMeleeDistance; x <= maxMeleeDistance; x++)
        {
            for (int z = -maxMeleeDistance; z <= maxMeleeDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = UnitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    continue;

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) //this can be remove for a explosive attack like for a grenade 
                    continue; // No enemy in grid position

                Unit targetUnit = LevelGrid.Instance.getUnitAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() == unit.IsEnemy())
                    continue; //cant attack units on the same side 

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.getUnitAtGridPosition(gridPosition);

        state = State.SwingingWeaponBeforeHit;
        float beforeHitStateTime = 0.7f;
        stateTimer = beforeHitStateTime;

        OnMeleeActionStarted?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }

    public int GetMaxMeleeDistance()
    {
        return maxMeleeDistance;
    }
}
