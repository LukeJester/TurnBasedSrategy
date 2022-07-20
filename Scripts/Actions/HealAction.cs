using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAction : BaseAction
{

    //[SerializeField] HealingItem healingItem;
    [SerializeField] Transform healingVFX;
    //[SerializeField] AudioClip healSound;

    //Healing Variables
    private int healingAmount = 50;
    private int numberOfTurnsHealingLasts;
    private int healingAPCost;

    private Unit targetUnit;

    private void Start()
    {
        //healingAmount = healingItem.GetHealingAmount();
        //numberOfTurnsHealingLasts = healingItem.GetTurnsOfHealing();
        //healingAPCost = healingItem.GetAPCost();
    }

    private void Update()
    {
        if(!isActive)
            return;

        ActionComplete();
    }

    public override string GetActionName()
    {
        return "Heal";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int currentActionValue = 0;
        if (gameObject.GetComponentInParent<Unit>().GetHealthNormilized() < 0.25f) // if health id below 25% then heal
        {
            currentActionValue = 1000;
        }
        return new EnemyAIAction // set to hit closest thing in range
        {
            gridPosition = gridPosition,
            actionValue = currentActionValue, // settting high means we will always use this is possible
        };
    }

    public override int GetActionPointsCost()
    {
        return 1; // return healingAPCost;
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();

        if (unit.GetHealthNormilized() >= 1)
            return new List<GridPosition>();
       
        return new List<GridPosition>
        {
            unitGridPosition
        };
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.getUnitAtGridPosition(gridPosition);

        Heal(targetUnit);

        ActionStart(onActionComplete);
    }

    public override ActionGroup GetActionGroup()
    {
        return ActionGroup.Item;
    }

    private void Heal(Unit targetUnit)
    {
        Transform healthVFX = Instantiate(healingVFX, targetUnit.transform.position, Quaternion.identity);
        targetUnit.Heal(healingAmount);
        Destroy(healthVFX.gameObject, 1.5f);
    }
}
