using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{

    public event EventHandler OnThrowGrenade;

    [SerializeField] private Transform grenadeProjectilePrefab;
    [SerializeField] private Transform grenadeProjectileRasiusPrefab;
    [SerializeField] private Transform handTransforms;
    [SerializeField] private string progectileName;

    private Transform grenadeRangeTransform;
    private GridPosition targetGridPosition;
    private bool actionIsSelected = false;

    GridVisualType gridVisualType;

    GrenadeProjectile grenadeProjectile;

    private int maxThrowDistance = 5;

    private void Start()
    {
        grenadeProjectile = grenadeProjectilePrefab.GetComponent<GrenadeProjectile>();

        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;

        AnimationEventHandler.OnAnyThrowActionAnimation += AnimationEventHandler_OnAnyThrowActionAnimation;
    }

    private void Update()
    {
        if (grenadeRangeTransform != null)
        {
            grenadeRangeTransform.position = LevelGrid.Instance.SnapWorldPosition(MouseWorld.instance.transform.position);
        }

        // if (!actionIsSelected)
        //     return;
        // ShowGridPositionsEffectedByProjectile();
    }

    private static void ShowGridPositionsEffectedByProjectile()
    {
        GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.instance.transform.position);
        GridSystemVisual.Instance.HideAllGridPositions();

        GridSystemVisual.Instance.ShowGridPositionRangeSquare(gridPosition, 1, GridVisualType.Red);
    }

    public override string GetActionName()
    {
        return "Throw " + progectileName;
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
        StartCoroutine(ThrowItemCoroutine(gridPosition)); 

        ActionStart(onActionComplete);
    }

    IEnumerator ThrowItemCoroutine(GridPosition targetGridPosition)
    {
        this.targetGridPosition = targetGridPosition;

        Vector3 aimDirection = (LevelGrid.Instance.GetWorldPosition(targetGridPosition) - unit.GetWorldPosition()).normalized;
        float rotateSpeed = 5f;
        unit.transform.forward = Vector3.Slerp(unit.transform.forward, aimDirection, rotateSpeed * Time.deltaTime);

        while (Vector3.Distance(unit.transform.forward, aimDirection) > .1)
        {
            unit.transform.forward = Vector3.Slerp(unit.transform.forward, aimDirection, rotateSpeed * Time.unscaledDeltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        OnThrowGrenade?.Invoke(this, EventArgs.Empty);
        yield return null;
    }

    private void SetUpThrownItem()
    {
        Transform greandeProjectileTransform = Instantiate(grenadeProjectilePrefab, unit.GetWorldPosition(), Quaternion.identity);
        GrenadeProjectile grenadeProjectile = greandeProjectileTransform.GetComponent<GrenadeProjectile>();
        grenadeProjectile.Setup(targetGridPosition, OnGrenadeBehaviorComplete);
    }

    public override ActionGroup GetActionGroup()
    {
        return ActionGroup.Item;
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, UnitActionSystem.OnSelectedAction e) 
    {
        
        
        if (grenadeRangeTransform != null)
        {
            Destroy(grenadeRangeTransform.gameObject);
        }

        if (e.unitAction == this)
        {
            actionIsSelected = true;

            grenadeRangeTransform = Instantiate(grenadeProjectileRasiusPrefab, LevelGrid.Instance.SnapWorldPosition(MouseWorld.instance.transform.position), Quaternion.identity);
            grenadeRangeTransform.localScale = Vector3.one * (GetGrenadeTileRange() * LevelGrid.Instance.GetCellSize() + 1.9f);
        }
        else
        {
            actionIsSelected = false;
        }
    }

    private void AnimationEventHandler_OnAnyThrowActionAnimation(object sender, AnimationEventHandler.OnAnyThrow e)
    {
        if (e.unit == unit)
            SetUpThrownItem();
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
