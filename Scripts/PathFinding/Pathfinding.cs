using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{

    public static Pathfinding Instance { get; private set; }

    private const int   MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private LayerMask ObstaclesLayerMask;
    [SerializeField] private LayerMask wallLayerMask;

    private int width;
    private int height;
    private float cellSize;
    private GridSystem<PathNode> gridSystem;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more then one Pathfinding! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Setup(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridSystem = new GridSystem<PathNode>(width, height, cellSize,
            (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
        //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                float rayCastOffsetDistance = 5f;
                if(Physics.Raycast(worldPosition + Vector3.down * rayCastOffsetDistance,
                    Vector3.up,
                    rayCastOffsetDistance * 2,
                    ObstaclesLayerMask))
                    {
                        GetNode(x,z).SetIsWalkable(false);
                        // This can also be done for flying Enemys!!! Just make it so that they cant fly onto a not walkable area
                        // This will kinda creat a whole diffrent pathfinding grid for flying enimes, where they can choose what have a better cost
                        // flying over the rock or walking around it
                        // need a way to know if a grid is flyable EI, crates can be flown over but not wall with roofs.
                    }
                // Add the wall directions to the the pathnodes
                RaycastHit rayOut;
                Physics.Raycast(worldPosition + Vector3.down * rayCastOffsetDistance, Vector3.up, out rayOut);
                if (rayOut.collider != null && rayOut.collider.GetComponent<Floor>())
                {
                    Floor floor = rayOut.collider.GetComponent<Floor>();
                    GetNode(x, z).AddWallDirection(floor.GetAllWallDirections());
                    SetWallCover();
                }
            }
        }
    }

    private void SetWallCover()
    {
        //Cehck if wall have cover, set its GP with matching cover direction, then do that for hte opposite side
        //EX) east wall GP gets east cover, then give GP x+1 West Cover
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
    {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = gridSystem.GetGridObject(startGridPosition);
        PathNode endNode = gridSystem.GetGridObject(endGridPosition);
        openList.Add(startNode);

        for (int x = 0; x < gridSystem.GetWidth(); x++)
        {
            for (int z = 0; z < gridSystem.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z) ;
                PathNode pathNode = gridSystem.GetGridObject(gridPosition);

                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.CalculateFCost();
                pathNode.ResetCameFromPathNode();
            }
        }

        startNode.SetGCost(0);
        startNode.SetHCost(calculateDisitance(startGridPosition, endGridPosition));
        startNode.CalculateFCost();

        while(openList.Count > 0)
        {
            PathNode currentNode = GetLowsetFCostPathNode(openList);

            if(currentNode == endNode)
            {
                //reached the final node
                pathLength = endNode.GetFCost();
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach(PathNode neighborNode in GetNeighborList(currentNode))
            {
                if(closedList.Contains(neighborNode))
                    continue;

                if(!neighborNode.IsWalkable())
                {
                    closedList.Add(neighborNode);
                    continue;
                }

                //test if enemy unit is at the position, then you cant walk throgh them!
                // only stops you from walking through enimes, not the other way around. Hope their IA dosent make this noticable
                // this will become noticable when a melee only enamy priorotises the most engered and walks throw your tank to get to the ingered guy
                if(LevelGrid.Instance.getUnitAtGridPosition(neighborNode.GetGridPosition()) != null)
                {
                    Unit unit = LevelGrid.Instance.getUnitAtGridPosition(neighborNode.GetGridPosition());
                    if (unit.IsEnemy())
                    {
                        closedList.Add(neighborNode);
                        continue;
                    }
                }

                int tentativeGCost = currentNode.GetGCost() + calculateDisitance(currentNode.GetGridPosition(), neighborNode.GetGridPosition());
            
                if(tentativeGCost < neighborNode.GetGCost())
                {
                    neighborNode.SetCameFromPathNode(currentNode);
                    neighborNode.SetGCost(tentativeGCost);
                    neighborNode.SetHCost(calculateDisitance(neighborNode.GetGridPosition(), endGridPosition));
                    neighborNode.CalculateFCost();
                    
                    if (!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }
                }
            }
        }

        //No Path Found
        pathLength = 0;
        return null;
    }

    public int calculateDisitance(GridPosition gridPositionA, GridPosition gridPositionB)
    {
        GridPosition gridPositionDistance = gridPositionA - gridPositionB;
        int xDistance =Mathf.Abs(gridPositionDistance.x);
        int zDistance = Mathf.Abs(gridPositionDistance.z);
        int remaning = Mathf.Abs(xDistance - zDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaning;
    }

    private PathNode GetLowsetFCostPathNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostPathNode = pathNodeList[0];
        for (int i = 0; i < pathNodeList.Count; i++)
        {
            if(pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost())
            {
                lowestFCostPathNode = pathNodeList[i];
            }
        }
        return lowestFCostPathNode;
    }

    private PathNode GetNode(int x, int z)
    {
        return gridSystem.GetGridObject(new GridPosition(x,z));
    }

    private List<PathNode> GetNeighborList(PathNode currentNode)
    {
        List<PathNode> neighborList = new List<PathNode>();

        GridPosition gridPosition = currentNode.GetGridPosition();

        if(gridPosition.x - 1 >= 0)
        {
            //Left Node
            if (!(GetNode(gridPosition.x - 1, gridPosition.z + 0).GetWallDirections() != null && GetNode(gridPosition.x - 1, gridPosition.z + 0).GetWallDirections().Contains(Wall.WallDirection.East)) && !(GetNode(gridPosition.x + 0, gridPosition.z + 0).GetWallDirections() != null && GetNode(gridPosition.x + 0, gridPosition.z + 0).GetWallDirections().Contains(Wall.WallDirection.West)))
            {
                neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0));
            }
            
            if( gridPosition.z + 1 < gridSystem.GetHeight() && GetNode(gridPosition.x - 1, gridPosition.z + 1).GetWallDirections() == null && GetNode(gridPosition.x + 0, gridPosition.z + 0).GetWallDirections() == null)
            {
                //Left Up Node
                neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1));
            }

            if (gridPosition.z - 1 >= 0 && GetNode(gridPosition.x - 1, gridPosition.z - 1).GetWallDirections() == null && GetNode(gridPosition.x + 0, gridPosition.z + 0).GetWallDirections() == null)
            {
                //Left Down Node
                neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1));
            }
        }
        if (gridPosition.x + 1 < gridSystem.GetWidth())
        {
            //Right Node
            if (!(GetNode(gridPosition.x + 1, gridPosition.z + 0).GetWallDirections() != null && GetNode(gridPosition.x + 1, gridPosition.z + 0).GetWallDirections().Contains(Wall.WallDirection.West)) && !(GetNode(gridPosition.x + 0, gridPosition.z + 0).GetWallDirections() != null && GetNode(gridPosition.x + 0, gridPosition.z + 0).GetWallDirections().Contains(Wall.WallDirection.East)))
            {
                neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0));
            }
           
            if ( gridPosition.z + 1 < gridSystem.GetHeight() && GetNode(gridPosition.x + 1, gridPosition.z + 1).GetWallDirections() == null && GetNode(gridPosition.x + 0, gridPosition.z + 0).GetWallDirections() == null )
            {
                //Right Up Node
                neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1));
            }
            
            if (gridPosition.z - 1 >= 0 && GetNode(gridPosition.x + 1, gridPosition.z - 1).GetWallDirections() == null && GetNode(gridPosition.x + 0, gridPosition.z + 0).GetWallDirections() == null ) 
            {
                //Right Down Node
                neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1));
            }
        }

        if (gridPosition.z + 1 < gridSystem.GetHeight())
        {
            //Up Node
            if (!(GetNode(gridPosition.x + 0, gridPosition.z + 1).GetWallDirections() != null && GetNode(gridPosition.x + 0, gridPosition.z + 1).GetWallDirections().Contains(Wall.WallDirection.South)) && !(GetNode(gridPosition.x + 0, gridPosition.z + 0).GetWallDirections() != null && GetNode(gridPosition.x + 0, gridPosition.z + 0).GetWallDirections().Contains(Wall.WallDirection.North))) 
            {
                neighborList.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1));
            }
        }
        if (gridPosition.z - 1 >= 0)
        {
            //Down Node
            if (!(GetNode(gridPosition.x + 0, gridPosition.z - 1).GetWallDirections() != null && GetNode(gridPosition.x + 0, gridPosition.z - 1).GetWallDirections().Contains(Wall.WallDirection.North)) && !(GetNode(gridPosition.x + 0, gridPosition.z + 0).GetWallDirections() != null && GetNode(gridPosition.x + 0, gridPosition.z + 0).GetWallDirections().Contains(Wall.WallDirection.South)))
            {
                neighborList.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1));
            }
        }

        return neighborList;
    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new List<PathNode>();
        pathNodeList.Add(endNode);
        PathNode currentNode = endNode;
        while(currentNode.GetCameFromPathNode() != null)
        {
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }

        pathNodeList.Reverse();

        List<GridPosition> gridPostionList = new List<GridPosition>();
        foreach(PathNode pathNode in pathNodeList)
        {
            gridPostionList.Add(pathNode.GetGridPosition());
        }
        return gridPostionList;
    }

    public void SetIsWalkableGridPosition(GridPosition gridPosition, bool isWalkable)
    {
        gridSystem.GetGridObject(gridPosition).SetIsWalkable(isWalkable);
    }

    public bool IsWalkableGridPosition(GridPosition gridPosition)
    {
        return gridSystem.GetGridObject(gridPosition).IsWalkable();
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        return FindPath(startGridPosition, endGridPosition, out int pathLength) != null;
    }

    public int GetPathLenth(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }

    
}