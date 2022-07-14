using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unit : MonoBehaviour
{
    private const int Action_Point_Max = 2;

    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;
    public static event EventHandler OnAnyCoverStateChanged;
    public event EventHandler OnCoverStateChanged;

    [SerializeField] private bool isEnemy;

    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    private BaseAction[] baseActionArray;
    private int actionPoints = Action_Point_Max;
    private CoverType currentCoverType;
    private CoverType newCoverType;
    

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        baseActionArray = GetComponentsInChildren<BaseAction>();
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

        TurnSystem.Instance.OnTurnChange += TurnSystem_OnTurnChange;

        healthSystem.OnDead += healthSystem_OnDead;

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);

        DestructableCrate.AfterAnyDestroyed += DestructableCrate_AfterAnyDestroyed;
        DestructableCrate.OnAnyPlacment += DestructableCrate_OnAnyPlacment;

        UpdateCoverType();
    }

    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if(newGridPosition != gridPosition)
        {
            //Unit Change Grid Position
            GridPosition oldGrisPosition = gridPosition;
            gridPosition = newGridPosition;

            LevelGrid.Instance.UnitMoveGridPosititon(this, oldGrisPosition, newGridPosition);

            UpdateCoverType(); // could move this to the completion of the move action
        }
    }

    private void UpdateCoverType() 
    {
        newCoverType = LevelGrid.Instance.GetUnitCoverType(transform.position);

        if (newCoverType == currentCoverType)
            return;

        currentCoverType = newCoverType;

        //Debug.Log(currentCoverType);

        OnCoverStateChanged?.Invoke(this, EventArgs.Empty);
        OnAnyCoverStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public CoverType GetCoverType()
    {
        return currentCoverType;
    }

    public T GetAction<T>() where T : BaseAction
    {
        foreach(BaseAction baseAction in baseActionArray)
        {
            if(baseAction is T)
            {
                return (T)baseAction;
            }
        }
        return null;
    }

    public GridPosition GetGridPosition()
    {
        return  gridPosition;
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    public BaseAction[] GetBaseActionArray()
    {
        return baseActionArray;
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if(CanSpenActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanSpenActionPointsToTakeAction(BaseAction baseAction)
    {
        if(actionPoints >= baseAction.GetActionPointsCost())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SpendActionPoints(int amoumt)
    {
        actionPoints -= amoumt;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoints()
    {
        return actionPoints;
    }

    public int GetMaxActionPoints()
    {
        return Action_Point_Max;
    }

    private void TurnSystem_OnTurnChange(object sender, EventArgs e)
    {
        if (IsEnemy() && !TurnSystem.Instance.IsPlayerTurn() || !IsEnemy() && TurnSystem.Instance.IsPlayerTurn())
        {
            actionPoints = Action_Point_Max;

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public void Damage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }

    private void healthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        
        Destroy(gameObject); // can change to play a deaht animation and or an explosion for the mechs

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);

        DestructableCrate.AfterAnyDestroyed -= DestructableCrate_AfterAnyDestroyed;
        DestructableCrate.OnAnyPlacment -= DestructableCrate_OnAnyPlacment;
    }

    private void DestructableCrate_AfterAnyDestroyed(object sender, EventArgs e)
    {
        if (this == null)
            return;
            
        UpdateCoverType();
    }

    private void DestructableCrate_OnAnyPlacment(object sender, EventArgs e)
    {
        UpdateCoverType();
    }

    public float GetHealthNormilized()
    {
        return healthSystem.GetHealthNormilized();
    }
}
