using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unit : MonoBehaviour 
{
    private const int Action_Point_Max = 20;

    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;
    public static event EventHandler OnAnyCoverStateChanged;
    public event EventHandler OnCoverStateChanged;

    [SerializeField] private bool isEnemy;
    [SerializeField] private Transform UnitVisual;
    [SerializeField] private Transform UnitWorldUI;
    [SerializeField] private Sprite unitsFace;

    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    private BaseAction[] baseActionArray;
    private int actionPoints = Action_Point_Max;
    private CoverType currentCoverType;
    private CoverType newCoverType;
    private List<CoverDirection> coverDirectionList;
    private StatusEffects statusEffects;
    

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        baseActionArray = GetComponentsInChildren<BaseAction>();
        statusEffects = GetComponent<StatusEffects>();
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
        LevelGrid.Instance.OnAnyUnitMoveGridPosition += LevelGrid_OnAnyUnitMoveGridPosition;

        TurnSystem.Instance.OnTurnChange += TurnSystem_OnTurnChange;

        healthSystem.OnDead += healthSystem_OnDead;

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);

        Cover.AfterAnyDestroyed += Cover_AfterAnyDestroyed;
        Cover.OnAnyPlacment += Cover_OnAnyPlacment;

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
        }
    }

    private void UpdateCoverType() // wont let you go to new cover if the same type one grid away
    {
        newCoverType = LevelGrid.Instance.GetUnitCoverType(transform.position);
        coverDirectionList = LevelGrid.Instance.GetUnitCoverDirection(transform.position);

        // foreach (CoverDirection coverDir in coverDirectionList)
        // {
        //     Debug.Log(coverDir);
        // }

        currentCoverType = newCoverType;

        OnCoverStateChanged?.Invoke(this, EventArgs.Empty);
        OnAnyCoverStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public CoverType GetCoverType()
    {
        return currentCoverType;
    }

    public List<CoverDirection> GetCoverDirectionList()
    {
        return coverDirectionList;
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

    public void Heal(int healAmount)
    {
        healthSystem.Heal(healAmount);
    }

    public void ApplyStatusEffect(StatusEffect statusEffect, int duration)
    {
        statusEffects.ApplyStatusEffect(statusEffect, duration);
    }

    public bool IsVisible()
    {
        //test if laer mask is invisable?
        return UnitVisual.gameObject.layer == 9;
        //return UnitVisual.gameObject.activeSelf;
    }

    public void HideVisual()
    {
        // Invisible - 9
        // unit - 7
        // default - 0

        if (UnitVisual == null) // the OnDestroy function on destructable create is calling Fog Of War script which calls this when play ends
            return;
        ChangeLayerOfChildren(UnitVisual, 9);
        UnitWorldUI.gameObject.layer = 9;

        MoveAction moveAction = GetComponentInChildren<MoveAction>();
        moveAction.SetMoveSpeed(100);
    }

    public void ShowVisual()
    {
        if (UnitVisual == null)
            return;
        ChangeLayerOfChildren(UnitVisual, 7);
        UnitWorldUI.gameObject.layer = 7;

        MoveAction moveAction = GetComponentInChildren<MoveAction>();
        moveAction.reSetMoveSpeed();

    }

    private void ChangeLayerOfChildren(Transform root, int layerMask)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Transform>(out Transform childTransform))
            {
                childTransform.gameObject.layer = layerMask;
            }
            
            ChangeLayerOfChildren(child, layerMask);
        }
    }


    private void healthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        
        Destroy(gameObject); // can change to play a deaht animation and or an explosion for the mechs

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);

        Cover.AfterAnyDestroyed -= Cover_AfterAnyDestroyed;
        Cover.OnAnyPlacment -= Cover_OnAnyPlacment;
    }

    private void Cover_AfterAnyDestroyed(object sender, EventArgs e)
    {
        if (this == null)
            return;
            
        UpdateCoverType();
    }

    private void Cover_OnAnyPlacment(object sender, EventArgs e)
    {
        UpdateCoverType();
    }

    private void LevelGrid_OnAnyUnitMoveGridPosition(object sender, LevelGrid.OnAnyUnitMoveGridPositionEventArgs e)
    {
        if (e.movedUnit == this)
            UpdateCoverType();
    }

    public float GetHealthNormilized()
    {
        return healthSystem.GetHealthNormilized();
    }

    public Transform GetUnitVisual()
    {
        return UnitVisual;
    }

    public Sprite GetUnitsFace()
    {
        return unitsFace;
    }
}
