using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{

    private List<Wall.WallDirection> wallDirectionList;
    [SerializeField] List<Transform> listOfWallTransfroms;

    private void Awake()
    {
        wallDirectionList = new List<Wall.WallDirection>();

        foreach(Transform transform in listOfWallTransfroms)
        {
            Wall wall = transform.GetComponent<Wall>();
            AddWallDirections(wall);
        }

        if(wallDirectionList == null)
        {
            wallDirectionList.Add(Wall.WallDirection.None);
        }
    }

    private void AddWallDirections(Wall wall)
    {
        wallDirectionList.Add(wall.GetWallDirection());
    }
    
    public List<Wall.WallDirection> GetAllWallDirections()
    {
        return wallDirectionList;
    }
}
