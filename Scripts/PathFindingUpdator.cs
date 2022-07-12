using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathFindingUpdator : MonoBehaviour
{
    void Start()
    {
        DestructableCrate.OnAnyDestroyed += DestructableCrate_OnAnyDestroyed;
        DestructableCrate.OnAnyPlacment += DestructableCrate_OnAnyPlacment;
    }

    private void DestructableCrate_OnAnyDestroyed(object sender, EventArgs e)
    {
        DestructableCrate destructableCrate = sender as DestructableCrate;
        Pathfinding.Instance.SetIsWalkableGridPosition(destructableCrate.GetGridPosition(), true);
    }

    private void DestructableCrate_OnAnyPlacment(object sender, EventArgs e)
    {
        DestructableCrate destructableCrate = sender as DestructableCrate;
        Pathfinding.Instance.SetIsWalkableGridPosition(destructableCrate.GetGridPosition(), false);
    }
}


