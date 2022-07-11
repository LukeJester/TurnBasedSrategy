using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    //I can use a Switch statment when it comes to the weapon script. If weapon script (for human weapons)
    // or if weaponMechPart sctipt (for Mechs) for finding range, damage, name , ETC.
    //this was shown as an option in section 4 of the course I think
    // Or I could use a check if GetComponentInParent<Weapon Script> not nulln then use that section of code

    public static event EventHandler<OnShootEventArgs> OnAnyShoot;
    public event EventHandler<OnShootEventArgs> OnShoot;

    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
        public Transform bulletProjectilePrefab;
        public Transform shootPointTransform;
    }

    private enum State // for the camera change when shooting like in Mutant: year zero but in Wastland 3 it dosent
    {
        Aiming,
        Shooting,
        cooloff
    }

    [SerializeField] private LayerMask obstacledLayerMask;

    //timess spent in each of the shooting state
    [SerializeField] private float AimingStateTime = 1f;
    [SerializeField] private float shootingStateTime = 0.1f;
    [SerializeField] private float cooloffStateTime = 0.5f;

    [SerializeField] RangedWeapon rangedWeapon;

    private State state;
    private float stateTimer;
    private Unit targetUnit;
    private bool canShootBullet;

    //Weapon Variables
    private int maxShootDistance;
    private int weaponDamage;
    private Transform bulletProjectilePrefab;
    private Transform shootPointTransform;

    protected override void Awake()
    {
        base.Awake();
        maxShootDistance = rangedWeapon.GetWeaponRange();
        weaponDamage = rangedWeapon.GetWeaponDamage();
        bulletProjectilePrefab = rangedWeapon.GetBulletProjectilePrefab();
        shootPointTransform = rangedWeapon.GetShootPointTransform();
    }

    private void Update()
    {
        if (!isActive)
            return;

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.Aiming:
                Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                float rotateSpeed = 10f;
                unit.transform.forward = Vector3.Slerp(unit.transform.forward, aimDirection, rotateSpeed * Time.deltaTime);
                break;
            case State.Shooting:
                if (canShootBullet)
                {
                    Shoot();
                    canShootBullet = false;
                }
                break;
            case State.cooloff:
                ActionComplete();
                break;
        }

        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    private void Shoot()
    {
        OnAnyShoot?.Invoke(this, new OnShootEventArgs { targetUnit = targetUnit, shootingUnit = unit, bulletProjectilePrefab = bulletProjectilePrefab, shootPointTransform = shootPointTransform });
        OnShoot?.Invoke(this, new OnShootEventArgs {targetUnit = targetUnit, shootingUnit = unit, bulletProjectilePrefab  = bulletProjectilePrefab , shootPointTransform = shootPointTransform });

        //Note for coppying code to make the Overcharged shot action
        //Some Actions can be placed on a weapon other that the base shoot
        //EX overload where targetUnit.Damage(weaponDamage * 2f); , cost extra AP, and damage weapon
        targetUnit.Damage(weaponDamage);
    }

    private void NextState()
    {
        switch (state)
        {
            case State.Aiming:
                if (stateTimer <= 0f)
                {
                    state = State.Shooting;
                    stateTimer = shootingStateTime;
                }
                break;
            case State.Shooting:
                if (stateTimer <= 0f)
                {
                    state = State.cooloff;
                    stateTimer = cooloffStateTime;
                }
                break;
            case State.cooloff:
                if (stateTimer <= 0f)
                {
                    isActive = false;
                    onActionComplete();
                }
                break;
        }
    }

    public override string GetActionName()
    {
        //return the name of the wepon that this action is attached to/ might be in weapon script(mech weapon part)
        string weaponName = this.gameObject.name;
        //string weaponName =  this.gameObject.GetComponentInParent<MechWeaponPart>().GetWeaponName();
        return "Fire " + weaponName;
    }

    public override int GetActionPointsCost()
    {
        //return the AP of the wepon that this action is attached to/ might be in weapon script(mech weapon part)
        //int  weaponAPCost =  this.gameObject.GetComponentInParent<MechWeaponPart>().GetWeaponActionPointCost();
        return 2;
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return GetValidActionGridPositionList(unitGridPosition);
    }

    public List<GridPosition> GetValidActionGridPositionList(GridPosition UnitGridPosition) 
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = UnitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    continue;

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if(testDistance > maxShootDistance)
                    continue; // make circul shoot patern for range

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) //this can be remove for a explosive attack like for a grenade 
                    continue; // No enemy in grid position

                Unit targetUnit = LevelGrid.Instance.getUnitAtGridPosition(testGridPosition);
                
                if(targetUnit.IsEnemy() == unit.IsEnemy())
                    continue; //cant attack units on the same side 

                Vector3 unitWorlPositon = LevelGrid.Instance.GetWorldPosition(UnitGridPosition);
                Vector3 shoorDirection = (targetUnit.GetWorldPosition() - unitWorlPositon).normalized;
                float unitSholderHeight = 1.7f; //needs to chane dependent on the height of the enemy
                if (Physics.Raycast(unitWorlPositon + Vector3.up *unitSholderHeight, shoorDirection, Vector3.Distance(unitWorlPositon, targetUnit.GetWorldPosition()), obstacledLayerMask))
                    continue; //No Line of Sight

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.getUnitAtGridPosition(gridPosition);

        state = State.Aiming;
        stateTimer = AimingStateTime;

        canShootBullet = true;

        ActionStart(onActionComplete);
    }

    public Unit GetTargetUnit()
    {
        return targetUnit;
    }

    public int GetMaxShootDistance()
    {
        return maxShootDistance;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit targetUnit = LevelGrid.Instance.getUnitAtGridPosition(gridPosition);
        
        
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormilized()) * 100f),
        };
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }
}
