using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] WallDirection wallDirection;

    public enum WallDirection
    {
        None,
        North,
        East,
        South,
        West
    }

    public WallDirection GetWallDirection()
    {
        return wallDirection;
    }
}
