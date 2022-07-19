using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverwatchAction : BaseAction
{

    //Ambush: Set up an attack that will trigger on the enemy's next turn. Uses all remaining AP, gaining +5% Hit Chance per extra AP spent.

    public event EventHandler<OnShootEventArgs> OnShoot;

    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
        public Transform bulletProjectilePrefab;
        public Transform shootPointTransform;
    }

    //pass in target and if hit
    //public event EventHandler<OnShootEventArgs> OnShoot; 
    // public class OnShootEventArgs : EventArgs
    // {
    //     public Unit shotUnit;
    //     public bool hit;
    // }

    [SerializeField] RangedWeapon rangedWeapon;

    [SerializeField] private LayerMask obstacledLayerMask;

    //Weapon Variables
    private int maxShootDistance;
    private int fullAccuracyMaximunShootDistance;
    private int fullAccuracyMinimumShootDistance;
    private int weaponDamage;
    private Transform bulletProjectilePrefab;
    private Transform shootPointTransform;

    private bool overwatchActive;
    private Unit movedUnit;

    public float slowDownFactor = 0.3f;

    private int APRemaning;
    private int APSpentOnThisAction;

    protected override void Awake()
    {
        base.Awake();
        maxShootDistance = rangedWeapon.GetWeaponMaxRange();
        fullAccuracyMaximunShootDistance = rangedWeapon.GetWeaponFullAccuracyMaximun();
        fullAccuracyMinimumShootDistance = rangedWeapon.GetWeaponFullAccuracyMinimum();
        weaponDamage = rangedWeapon.GetWeaponDamage();
        bulletProjectilePrefab = rangedWeapon.GetBulletProjectilePrefab();
        shootPointTransform = rangedWeapon.GetShootPointTransform();
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChange += TurnSystem_OnTurnChange;
        LevelGrid.Instance.OnAnyUnitMoveGridPosition += LevelGrid_OnAnyUnitMoveGridPosition;
    }

    private void Update()
    {
        if (!isActive) return;
        
        //could use animation event on the shoot animation
        ActionComplete();
    }

    private void ChechIfOverwatchActivates(Unit movedUnit)
    {
        if (overwatchActive)
        {    
            if (movedUnit.IsEnemy() && InLineOfSight(movedUnit))
            {
                if (Vector3.Distance(movedUnit.GetWorldPosition(), unit.GetWorldPosition())> maxShootDistance)
                    return;

                //if (hit chance < 70%?)
                    //return;
                    
                // It's an alive enemy and it's visible!
                overwatchActive = false;

                this.movedUnit = movedUnit;

                // Calc hit or miss
                bool hit = true;// UnityEngine.Random.Range(0, 1f) < GetHitPercent(targetUnit);

                //OnShoot?.Invoke(this, new OnShootEventArgs { shotUnit = movedUnit, hit = hit });
                Time.timeScale = slowDownFactor;
                //Time.fixedDeltaTime = 0.02f;
                StartCoroutine(ShootTargetCoroutine());
                //OnShoot?.Invoke(this, new OnShootEventArgs { targetUnit = movedUnit, shootingUnit = this.GetComponentInParent<Unit>(), bulletProjectilePrefab = bulletProjectilePrefab, shootPointTransform = shootPointTransform });

                // if (hit)
                // {
                //     // Hit the unit!
                //     int damageAmount = UnityEngine.Random.Range(30, 60);
                //     movedUnit.GetHealthSystem().Damage(damageAmount);
                // }

                //movedUnit.Damage(weaponDamage);
            }
        }
    }

    private bool InLineOfSight(Unit targetUnit)
    {
        Vector3 unitWorlPositon = transform.position;
        //Vector3 unitWorlPositon = LevelGrid.Instance.GetWorldPosition(UnitGridPosition);
        Vector3 shoorDirection = (targetUnit.GetWorldPosition() - unitWorlPositon).normalized;
        float unitSholderHeight = 1.7f; //needs to chane dependent on the height of the enemy
        if (Physics.Raycast(unitWorlPositon + Vector3.up * unitSholderHeight, shoorDirection, Vector3.Distance(unitWorlPositon, targetUnit.GetWorldPosition()), obstacledLayerMask))
            return false; //No Line of Sight
        else return true;
    }

    IEnumerator ShootTargetCoroutine()
    {
        Vector3 aimDirection = (movedUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
        float rotateSpeed = 2f;
        
        while (Vector3.Distance(unit.transform.forward, aimDirection) > .1)
        {
            unit.transform.forward = Vector3.Slerp(unit.transform.forward, aimDirection, rotateSpeed * Time.unscaledDeltaTime);
            yield return null;
        }

        OnShoot?.Invoke(this, new OnShootEventArgs { targetUnit = movedUnit, shootingUnit = this.GetComponentInParent<Unit>(), bulletProjectilePrefab = bulletProjectilePrefab, shootPointTransform = shootPointTransform });

        Time.timeScale = 1f;

        movedUnit.Damage(weaponDamage);
    }

    public bool IsOverwatchActive()
    {
        return overwatchActive;
    }

    public override string GetActionName()
    {
        return "Overwatch";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue =  0,
        }; // if in safe spot(cover) with only 2 ap left then use, or what ever the ap cost is
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        if (!rangedWeapon.HasEnoughAmmoToShoot())
            return new List<GridPosition>();  // not enough ammo to shoot

        GridPosition unitGridPosition = unit.GetGridPosition();

        return new List<GridPosition>
        {
            unitGridPosition
        };
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        // APSpentOnThisAction = APRemaning;
        // Debug.Log(APSpentOnThisAction);
         overwatchActive = true;

        ActionStart(onActionComplete);
    }

    public override int GetActionPointsCost()  // how to save the ap spent on this action to increase acruasy wit heach ap? 
    {
        Unit unit = this.GetComponentInParent<Unit>();
        int APRemaning= unit.GetActionPoints();
        
        return Mathf.Max(APRemaning,1) ;
    }

    private void TurnSystem_OnTurnChange(object sender, EventArgs e)
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            // Back into the Player's turn, stop Overwatching
            overwatchActive = false;
        }
    }

    private void LevelGrid_OnAnyUnitMoveGridPosition(object sender, LevelGrid.OnAnyUnitMoveGridPositionEventArgs e)
    {
        Unit movedUnit = e.movedUnit;
        if (this == null)
        {
            //overwatchActive = false;
            return;
        }

        ChechIfOverwatchActivates(movedUnit);
    }

    // dont complete imidiatly, wail unil shoot is over, look how grenade does it?
    //could use animation event on the shoot animation
    private void OnOverwatchComplete()
    {
        ActionComplete();
    }

}
