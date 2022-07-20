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
        public bool hit;
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
    private Unit shootingUnit;
    private bool canShootBullet;

    //Weapon Variables
    private int maxShootDistance;
    private int fullAccuracyMaximunShootDistance;
    private int fullAccuracyMinimumShootDistance;
    private int weaponDamage;
    private int weaponAPCost;
    private int accuracy;
    private int currentAmmoInClip;
    private int critChance;
    private Transform bulletProjectilePrefab;
    private Transform shootPointTransform;

    private float maxAccuracy = 100;

    protected override void Awake() // this might need to be moved to start for concistancy
    {
        base.Awake();
        maxShootDistance = rangedWeapon.GetWeaponMaxRange();
        fullAccuracyMaximunShootDistance = rangedWeapon.GetWeaponFullAccuracyMaximun();
        fullAccuracyMinimumShootDistance = rangedWeapon.GetWeaponFullAccuracyMinimum();
        weaponDamage = rangedWeapon.GetWeaponDamage();
        weaponAPCost = rangedWeapon.GetAPCost();
        accuracy = rangedWeapon.GetAccuracy();
        bulletProjectilePrefab = rangedWeapon.GetBulletProjectilePrefab();
        shootPointTransform = rangedWeapon.GetShootPointTransform();
        critChance = rangedWeapon.GetCritChance();

        shootingUnit = GetComponentInParent<Unit>();
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
        // Calc hit or miss
        float randomRoll = UnityEngine.Random.Range(0, 1f);
        bool hit = randomRoll < GetHitPercent(targetUnit);

        rangedWeapon.SubtractAmmoFromClip(1);
        
        OnAnyShoot?.Invoke(this, new OnShootEventArgs { targetUnit = targetUnit, shootingUnit = unit, bulletProjectilePrefab = bulletProjectilePrefab, shootPointTransform = shootPointTransform });
        OnShoot?.Invoke(this, new OnShootEventArgs {targetUnit = targetUnit, shootingUnit = unit, bulletProjectilePrefab  = bulletProjectilePrefab , shootPointTransform = shootPointTransform, hit = hit });

        //Note for coppying code to make the Overcharged shot action
        //Some Actions can be placed on a weapon other that the base shoot
        //EX overload where targetUnit.Damage(weaponDamage * 2f); , cost extra AP, and damage weapon
        if(hit)
        {
            targetUnit.Damage(weaponDamage);
        }
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
        return "Shoot";
    }

    public override int GetActionPointsCost()
    {
        return weaponAPCost;
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

                if (!rangedWeapon.HasEnoughAmmoToShoot())
                    continue;  // not enough ammo to shoot

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

    public override ActionGroup GetActionGroup()
    {
        return ActionGroup.Attack;
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }

    private int GetShootDistance(GridPosition shootGridPosition)
    {
        GridPosition currentGridPosition = unit.GetGridPosition();
        GridPosition shootVector = shootGridPosition - currentGridPosition;
        int shootDistance = Mathf.Abs(shootVector.x) + Mathf.Abs(shootVector.z);
        return shootDistance;
    }

    public bool IsWithinShootingDistance(GridPosition shootGridPosition)
    {
        return GetShootDistance(shootGridPosition) <= maxShootDistance;
    }


    public float GetHitPercent(Unit targetUnit)
    {
        if (IsWithinShootingDistance(targetUnit.GetGridPosition()))
        {
            // Within shoot range
            float hitPercent = maxAccuracy;

            int shootDistance = GetShootDistance(targetUnit.GetGridPosition());
            int remainingShootDistance = Mathf.Max(0, shootDistance - fullAccuracyMaximunShootDistance);

            if (shootDistance <= fullAccuracyMinimumShootDistance) // if to close
            {
                remainingShootDistance = Mathf.Max(0, fullAccuracyMinimumShootDistance - shootDistance);
            }

            hitPercent -= (100 - accuracy) * remainingShootDistance;

            if (IsTargetUnitInCoverComparedtoThisUnitsPosition(targetUnit))
            {
                switch (targetUnit.GetCoverType())
                {
                    case CoverType.Full:
                        hitPercent -= 30f;
                        break;
                    case CoverType.Half:
                        hitPercent -= 10f;
                        break;
                }
            }

            // Take into account elivation, +10% chance to hit
            
            hitPercent = Mathf.Max(0, hitPercent);

            return hitPercent/100;

        }
        else
        {
            // Not within shoot range
            return 0f;
        }
    }

    private bool IsTargetUnitInCoverComparedtoThisUnitsPosition(Unit targetUnit)
    {
        List<CoverDirection> coverDirectionList = new List<CoverDirection>();
        coverDirectionList = targetUnit.GetCoverDirectionList();

        if (coverDirectionList.Contains(CoverDirection.North))
            if (shootingUnit.GetGridPosition().z > targetUnit.GetGridPosition().z)
                return true;
        if (coverDirectionList.Contains(CoverDirection.East))
            if (shootingUnit.GetGridPosition().x > targetUnit.GetGridPosition().x)
                return true;
        if (coverDirectionList.Contains(CoverDirection.South))
            if (shootingUnit.GetGridPosition().z < targetUnit.GetGridPosition().z)
                return true;
        if (coverDirectionList.Contains(CoverDirection.West))
            if (shootingUnit.GetGridPosition().x < targetUnit.GetGridPosition().x)
                return true;
        

        return false;
    }

    
}
