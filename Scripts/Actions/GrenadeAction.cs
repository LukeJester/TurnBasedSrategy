using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{

    public event EventHandler OnThrowGrenade;

    [SerializeField] private Transform grenadeProjectilePrefab;
    [SerializeField] private Transform grenadeProjectileRasiusPrefab;

    private Transform grenadeRangeTransform;

    GrenadeProjectile grenadeProjectile;

    private int maxThrowDistance = 5;

    private void Start()
    {
        grenadeProjectile = grenadeProjectilePrefab.GetComponent<GrenadeProjectile>();

        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
    }



    private void Update()
    {
        if (grenadeRangeTransform != null)
        {
            grenadeRangeTransform.position = LevelGrid.Instance.SnapWorldPosition(MouseWorld.instance.transform.position);
        }
    }

    public override string GetActionName()
    {
        return "Grenade";
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

        GridPosition UnitGridPosition = unit.GetGridPosition();
        //GridPosition grenadeGridPosition = MouseWorld.instance.GetMousesCurentGridPosition();

        for (int x = -maxThrowDistance; x <= maxThrowDistance; x++)
        {
            for (int z = -maxThrowDistance; z <= maxThrowDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = UnitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    continue;

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxThrowDistance)
                    continue; // make circul range

                if (UnitGridPosition == testGridPosition)
                    continue; //already standing there

                

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        //will first instaniate it in hand position with grenade consumable / all consumables will be spawned in right hand
        Transform greandeProjectileTransform =  Instantiate(grenadeProjectilePrefab, unit.GetWorldPosition(), Quaternion.identity);
        // will throw grenade after short delay so it goes in mid throw animation
        GrenadeProjectile grenadeProjectile = greandeProjectileTransform.GetComponent<GrenadeProjectile>();

        OnThrowGrenade?.Invoke(this, EventArgs.Empty); // not used, but will be when we add throwing greande animation

        grenadeProjectile.Setup(gridPosition, OnGrenadeBehaviorComplete);

        ActionStart(onActionComplete);
    }

    // spawns range prefab for greande
    // spans one for each unit on the fild, how to only spawn for this one?
    private void UnitActionSystem_OnSelectedActionChanged(object sender, UnitActionSystem.OnSelectedAction e) 
    {
        if (grenadeRangeTransform != null)
        {
            Destroy(grenadeRangeTransform.gameObject);
        }

        if (e.unitAction.GetActionName() == "Grenade")
        {
            grenadeRangeTransform = Instantiate(grenadeProjectileRasiusPrefab, LevelGrid.Instance.SnapWorldPosition(MouseWorld.instance.transform.position), Quaternion.identity);
            grenadeRangeTransform.localScale = Vector3.one * (GetGrenadeTileRange() * LevelGrid.Instance.GetCellSize() + 1.3f);
        }
    }

    private void OnGrenadeBehaviorComplete()
    {
        ActionComplete();
    }

    public GrenadeProjectile GetGrenadeProjectile()
    {
        return grenadeProjectile;
    } 

    public int GetGrenadeTileRange()
    {
        return grenadeProjectile.GetExplosionRadiusInTiles();
    }

    public int GetExplosionRadius()
    {
        return grenadeProjectile.GetExplosionRadius();
    }

}
