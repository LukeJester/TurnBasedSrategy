using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelGrid : MonoBehaviour
{

    public static LevelGrid Instance { get; private set; }

    public event EventHandler OnAnyUnitMoveGridPosition;

    [SerializeField] private Transform gridDebugObjectPrefab;

    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private float cellSize = 2f;

    private GridSystem<GridObject> gridSystem;
    

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more then one LevelGrid! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        gridSystem = new GridSystem<GridObject>(width, height, cellSize, (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));
        //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

        UpdateCoverGridPositions(width, height, cellSize);
    }

    private void Start()
    {
        Pathfinding.Instance.Setup(width, height, cellSize);

        DestructableCrate.AfterAnyDestroyed += DestructableCrate_AfterAnyDestroyed;
        DestructableCrate.OnAnyPlacment += DestructableCrate_OnAnyPlacment;
    }

    public void UpdateCoverGridPositions(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                float rayCastOffsetDistance = 5f;

                RaycastHit rayOut;
                Physics.Raycast(worldPosition + Vector3.down * rayCastOffsetDistance, Vector3.up, out rayOut);
                if (rayOut.collider != null && rayOut.collider.GetComponent<Cover>())
                {
                    Cover coverObject = rayOut.collider.GetComponent<Cover>();
                    gridSystem.GetGridObject(gridPosition).SetCoverType(coverObject.GetCoverType());
                }
                else
                {
                    gridSystem.GetGridObject(gridPosition).SetCoverType(CoverType.None); // doesnt work due to the Destroy call on the crate running after this/thus still being in the world
                }
            }
        }
    }

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.AddUnit(unit);
    }

    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetUnitList();
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.RemoveUnit(unit);
    }

    public void UnitMoveGridPosititon(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
    {
        RemoveUnitAtGridPosition(fromGridPosition, unit);
        AddUnitAtGridPosition(toGridPosition, unit);

        OnAnyUnitMoveGridPosition?.Invoke(this, EventArgs.Empty);
    }

    public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);

    public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);
    
    public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);

    public int GetWidth() => gridSystem.GetWidth();

    public int GetHeight() => gridSystem.GetHeight();

    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.HasAnyUnit();
    }

    public Unit getUnitAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetUnit();
    }

    public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetInteractable();
    }

    public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.SetInteractable(interactable);
    }

    public CoverType GetCoverTypeAtPosition(Vector3 worldPosition)
    {
        GridPosition gridPosition = gridSystem.GetGridPosition(worldPosition);
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetCoverType();
    }

    public CoverType GetUnitCoverType(Vector3 worldPosition) // where I can add the is Wakable check
    {
        GridPosition gridPosition = gridSystem.GetGridPosition(worldPosition);
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);

        // Find closest cover to this position
        
        bool hasLeft = gridSystem.IsValidGridPosition(new GridPosition(gridPosition.x - 1, gridPosition.z + 0));
        bool hasRight = gridSystem.IsValidGridPosition(new GridPosition(gridPosition.x + 1, gridPosition.z + 0));
        bool hasFront = gridSystem.IsValidGridPosition(new GridPosition(gridPosition.x + 0, gridPosition.z + 1));
        bool hasBack = gridSystem.IsValidGridPosition(new GridPosition(gridPosition.x + 0, gridPosition.z - 1));

        CoverType leftCover, rightCover, frontCover, backCover;
        leftCover = rightCover = frontCover = backCover = CoverType.None;

        if (hasLeft) leftCover = gridSystem.GetGridObject(new GridPosition(gridPosition.x - 1, gridPosition.z + 0)).GetCoverType();
        if (hasRight) rightCover = gridSystem.GetGridObject(new GridPosition(gridPosition.x + 1, gridPosition.z + 0)).GetCoverType();
        if (hasFront) frontCover = gridSystem.GetGridObject(new GridPosition(gridPosition.x + 0, gridPosition.z + 1)).GetCoverType();
        if (hasBack) backCover = gridSystem.GetGridObject(new GridPosition(gridPosition.x + 0, gridPosition.z - 1)).GetCoverType();

        if (leftCover == CoverType.Full ||
            rightCover == CoverType.Full ||
            frontCover == CoverType.Full ||
            backCover == CoverType.Full)
        {
            // At least one Full Cover
            return CoverType.Full;
        }

        if (leftCover == CoverType.Half ||
            rightCover == CoverType.Half ||
            frontCover == CoverType.Half ||
            backCover == CoverType.Half)
        {
            // At least one Half Cover
            return CoverType.Half;
        }

        return CoverType.None;
    }

    private void DestructableCrate_AfterAnyDestroyed(object sender, EventArgs e)
    {
        UpdateCoverGridPositions(this.width, this.height, this.cellSize);
    }

    private void DestructableCrate_OnAnyPlacment(object sender, EventArgs e)
    {
        UpdateCoverGridPositions(this.width, this.height, this.cellSize);
    }
}
