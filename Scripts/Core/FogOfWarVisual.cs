using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarVisual : MonoBehaviour 
{
    public static FogOfWarVisual Instance { get; private set; }


    [SerializeField] private Transform pfFogOfWarGridVisual;


    private int width;
    private int height;
    private MeshRenderer[,] gridVisualArray;

    private bool startHappened = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (startHappened)
            return;
        
        startHappened = true;

        int width = LevelGrid.Instance.GetWidth();
        int height = LevelGrid.Instance.GetHeight();

        gridVisualArray = new MeshRenderer[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridPosition gridPosition = new GridPosition(x,y);
                Transform gridVisual = Instantiate(pfFogOfWarGridVisual, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
                gridVisual.parent = transform;
                gridVisualArray[x, y] = gridVisual.Find("Visual").GetComponent<MeshRenderer>();

                CoverType coverAtGridPosition = LevelGrid.Instance.GetCoverTypeAtGridPosition(gridPosition);
                if (coverAtGridPosition == CoverType.Environment || coverAtGridPosition == CoverType.Full)
                {
                    gridVisualArray[x, y].enabled = false;
                }
            }
        }
    }

    // Hide all Grid Positions, which means show the shadow visual
    public void HideAllGridPositions()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                MeshRenderer gridVisualMeshRenderer = gridVisualArray[x, y];

                GridPosition gridPosition= new GridPosition(x,y);
                CoverType coverAtGridPosition = LevelGrid.Instance.GetCoverTypeAtGridPosition(gridPosition);
                if (coverAtGridPosition == CoverType.Environment || coverAtGridPosition == CoverType.Full)
                {
                    gridVisualMeshRenderer.enabled = false;
                    return;
                }
    
                gridVisualMeshRenderer.enabled = true;
            }
        }
    }

    // Show all these Grid positions which means hide the shadow visual
    public void ShowGridPositions(List<GridPosition> gridPositionList)
    {
        foreach (GridPosition gridPosition in gridPositionList)
        {
            if (gridVisualArray ==null) // race error with the FogOfWar script
            {
                Start();
            }

            MeshRenderer gridVisualMeshRenderer = gridVisualArray[gridPosition.x, gridPosition.z];
            gridVisualMeshRenderer.enabled = false;
        }
    }
}
