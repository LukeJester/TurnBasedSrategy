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

    public event EventHandler<OnShootEventArgs> OnShoot;

    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }

    private enum State // for the camera change when shooting like in Mutant: year zero but in Wastland 3 it dosent
    {
        Aiming,
        Shooting,
        cooloff
    }

    //timess spent in each of the shooting state
    [SerializeField] private float AimingStateTime = 1f;
    [SerializeField] private float shootingStateTime = 0.1f;
    [SerializeField] private float cooloffStateTime = 0.5f;

    private State state;
    private float stateTimer;
    private Unit targetUnit;
    private bool canShootBullet;

    //some Weapon Vriables that need to get moved
    private int maxShootDistance = 4;
    private int weaponDamage = 50;

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
        OnShoot?.Invoke(this, new OnShootEventArgs {targetUnit = targetUnit, shootingUnit = unit});

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
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

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

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        ActionStart(onActionComplete);

        targetUnit = LevelGrid.Instance.getUnitAtGridPosition(gridPosition);

        state = State.Aiming;
        stateTimer = AimingStateTime;

        canShootBullet = true;
    }
}
