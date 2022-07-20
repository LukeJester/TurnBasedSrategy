using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{

    public static GridSystemVisual Instance { get; private set; }

    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }

    public enum GridVisualType
    {
        White,
        Blue,
        Red,
        Yellow,
        RedSoft,
        Green,
        GreenSoft
    }
    
    [SerializeField] Transform GridSystemVisualSinglePrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;

    private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

    private bool visualsAreShowing = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more then one GridSystemVisual! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        MakeGridVisuals();

        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        LevelGrid.Instance.OnAnyUnitMoveGridPosition += LevelGrid_OnAnyUnitMoveGridPosition;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;
        TurnSystem.Instance.OnTurnChange += TurnSystem_OnTurnChange;
        //MouseWorld.OnMousesCurentGridPositionChange += MouseWorld_OnMousesCurentGridPositionChange;

        UpdateGridVisual();
    }

    public void HideAllGridPositions()
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                gridSystemVisualSingleArray[x, z].Hide();
            }
        }
    }

    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();

        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x,z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    continue;

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > range)
                    continue; // make circul shoot patern for range

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    private void ShowGridPositionRangeSquare(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();

        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    continue;

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
    {
        foreach(GridPosition gridPosition in gridPositionList)
        {
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }

    private void UpdateGridVisual()
    {
        HideAllGridPositions();

        if (!TurnSystem.Instance.IsPlayerTurn())
            return;

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

        GridVisualType gridVisualType;

        switch (selectedAction)
        {
            default:
            case MoveAction moveAction:
                gridVisualType = GridVisualType.White;
                //could add walk range visual like in XCOM
                // OR make each tile movment 1 AP and in the move action show full move range with currnet AP
                break;

            case SpinAction spinAction:
                gridVisualType = GridVisualType.Blue;
                break;

            case ShootAction shootAction:
                gridVisualType = GridVisualType.Red;

                ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.GetMaxShootDistance(), GridVisualType.RedSoft);
                break;

            case GrenadeAction grenadeAction: // would i add the explaoion grid effected area here?
                gridVisualType = GridVisualType.RedSoft;

                //Only shows the grid positions when the action is selected and only grid position are highlighted when not already redsoft, could fix with // code above
                //ShowGridPositionRangeSquare(MouseWorld.instance.GetMousesCurentGridPosition(), grenadeAction.GetGrenadeProjectile().GetExplosionRadiusInTiles(), GridVisualType.Red);
                break;

            case createCrateAction createCrateAction:
                gridVisualType = GridVisualType.Yellow;
                break;

            case MeleeAction meleeAction:
                gridVisualType = GridVisualType.Red;

                ShowGridPositionRangeSquare(selectedUnit.GetGridPosition(), meleeAction.GetMaxMeleeDistance(), GridVisualType.RedSoft);
                break;

            case InteractAction interactAction:
                gridVisualType = GridVisualType.Blue;
                break;

            case HealAction healAction:
                gridVisualType = GridVisualType.Green;
                break;
        }

        ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType);
    }

    private void MakeGridVisuals()
    {
        gridSystemVisualSingleArray = new GridSystemVisualSingle[LevelGrid.Instance.GetWidth(),LevelGrid.Instance.GetHeight()];
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform gridSystemVisualSingleTransform = Instantiate(GridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity) ;

                gridSystemVisualSingleArray[x,z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }

        visualsAreShowing = true;
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void LevelGrid_OnAnyUnitMoveGridPosition(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void MouseWorld_OnMousesCurentGridPositionChange(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void TurnSystem_OnTurnChange(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            HideAllGridPositions();
        }
    }


    private Material GetGridVisualTypeMaterial(GridVisualType  gridVisualType)
    {
        foreach(GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType)
            {
                return gridVisualTypeMaterial.material;
            }
        }

        Debug.LogError("Could not fund GridVisualTypeMaterial for GridVisualTyp " + gridVisualType);
        return null;
    }

    // private void DeactivateGridVisuals() 
    // {
    //     var temp =GameObject.FindGameObjectsWithTag("GridVisual");
    //     foreach(GameObject gridVisual  in temp)
    //     {
    //         Destroy(gridVisual);
    //     }
    //     visualsAreShowing = false;
    // }

}
