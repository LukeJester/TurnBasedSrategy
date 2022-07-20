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
    public static event EventHandler<OnMeleeDodgeEventArgs> OnAnyMeleeDodge;

    public class OnMeleeDodgeEventArgs : EventArgs
    {
        public Unit targetUnit;
    }

    private enum State
    {
        SwingingWeaponBeforeHit,
        SwingingWeaponAfterHit,
    }

    [SerializeField] MeleeWeapon meleeWeapon;

    //Weapon Variables
    private int maxMeleeDistance;
    private int meleeDamage;
    private int weaponAPCost;
    private int accuracy;

    private float maxAccuracy = 100;

    private bool hit;

    private State state;
    private float stateTimer;
    private Unit targetUnit;

    private void Start()
    {
        maxMeleeDistance = meleeWeapon.GetWeaponRange();
        meleeDamage = meleeWeapon.GetWeaponDamage();
        weaponAPCost = meleeWeapon.GetAPCost();
        accuracy = meleeWeapon.GetAccuracy();
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
                if (hit)
                {
                    targetUnit.Damage(meleeDamage);
                }
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
        return "Melee";
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

        // Calc hit or miss
        float randomRoll = UnityEngine.Random.Range(0, 1f);
        hit = randomRoll < GetHitPercent(targetUnit);

        if (!hit)
        {
            //calls the target unit animator on a miss to dodge, Might do this in all atack scripts for a miss
            OnAnyMeleeDodge?.Invoke(this, new OnMeleeDodgeEventArgs { targetUnit = targetUnit });
        }

        OnMeleeActionStarted?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }

    private int GetMeleeDistance(GridPosition shootGridPosition)
    {
        GridPosition currentGridPosition = unit.GetGridPosition();
        GridPosition shootVector = shootGridPosition - currentGridPosition;
        int shootDistance = Mathf.Abs(shootVector.x) + Mathf.Abs(shootVector.z);
        return shootDistance;
    }

    public bool IsWithinShootingDistance(GridPosition shootGridPosition)
    {
        return GetMeleeDistance(shootGridPosition) <= maxMeleeDistance;
    }


    public float GetHitPercent(Unit shootUnit)
    {
        if (IsWithinShootingDistance(shootUnit.GetGridPosition()))
        {
            float hitPercent = maxAccuracy;

            hitPercent -= (100 - accuracy) ;

            switch (shootUnit.GetCoverType())
            {
                case CoverType.Full:
                    hitPercent -= 30f;
                    break;
                case CoverType.Half:
                    hitPercent -= 10f;
                    break;
            }

            hitPercent = Mathf.Max(0, hitPercent);

            return hitPercent / 100;

        }
        else
        {
            // Not within shoot range
            return 0f;
        }
    }

    public int GetMaxMeleeDistance()
    {
        return maxMeleeDistance;
    }
}
