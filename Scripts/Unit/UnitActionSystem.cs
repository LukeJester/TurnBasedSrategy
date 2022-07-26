using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{

    public static UnitActionSystem Instance {get; private set; }

    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler<OnSelectedAction> OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;

    public class OnSelectedAction : EventArgs
    {
        public Unit unit;
        public BaseAction unitAction;
    }

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unityLayerMask;

    private BaseAction selectedAction;
    private bool isBusy;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("There is more then one UnitActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {

        SetSelectedUnit(selectedUnit); // I can change this the unit who has first turn
    }
    private void Update()
    {
        if(isBusy)
            return;

        if (!TurnSystem.Instance.IsPlayerTurn())
            return;

        if(EventSystem.current.IsPointerOverGameObject())
            return;

        if (TryHandelUnitSelection())
            return;
        
        HandleSelectedAction();
    }

    private void HandleSelectedAction()
    {
        if (InputManager.Instance.IsMouseButtonDownthisFrame())
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if(selectedAction.IsValidActionGridPosition(mouseGridPosition))  
            {
                if(selectedUnit.TrySpendActionPointsToTakeAction(selectedAction))
                {
                    SetBusy();
                    selectedAction.TakeAction(mouseGridPosition, ClearBusy);

                    OnActionStarted?.Invoke(this, EventArgs.Empty);
                }    
            }
        }
    }

    private void SetBusy()
    {
        isBusy = true;

        OnBusyChanged?.Invoke(this, isBusy);
    }

    private void ClearBusy()
    {
        isBusy = false;

        OnBusyChanged?.Invoke(this, isBusy);
    }

    private bool TryHandelUnitSelection()
    {
        if (InputManager.Instance.IsMouseButtonDownthisFrame())
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
            if(Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unityLayerMask))
            {
                if(raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                {
                    if(unit == selectedUnit)
                        return false; // already selected

                    if (unit.IsEnemy())
                        return false; // is an enemy unit

                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }

        return false;
    }

    public void SetSelectedUnit(Unit unit)
    {
        
        selectedUnit = unit;
        if(selectedUnit == null)
            return;
        SetSelectedAction(unit.GetAction<MoveAction>());

        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;

        OnSelectedActionChanged?.Invoke(this, new OnSelectedAction { unitAction = selectedAction, unit =  selectedUnit });
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }
}
