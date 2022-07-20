using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    public static FogOfWar Instance { get; private set; }

    private void Start()
    {
        LevelGrid.Instance.OnAnyUnitMoveGridPosition += LevelGrid_OnAnyUnitMoveGridPosition;
        Cover.AfterAnyDestroyed += Cover_AfterAnyDestroyed;
        UpdateAllFogOfWar();
    }

    private void LevelGrid_OnAnyUnitMoveGridPosition(object sender, System.EventArgs e)
    {
        UpdateAllFogOfWar();
    }

    private void Cover_AfterAnyDestroyed(object sender, EventArgs e)
    {
        UpdateAllFogOfWar();
    }

    private void UpdateAllFogOfWar()
    {
        FogOfWarVisual.Instance.HideAllGridPositions();

        List<GridPosition> revealedGridPositionList = new List<GridPosition>();

        foreach (Unit unit in UnitManager.Instance.GetFriendlyUnitList())
        {
            GridPosition unitGridPosition = unit.GetGridPosition();
            Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);

            revealedGridPositionList.Add(unitGridPosition);

            TestFogOfWarOnPosition(unitWorldPosition, ref revealedGridPositionList);

            // Test positions around Unit
            List<GridPosition> neighbourGridOffsetList = new List<GridPosition> {
                new GridPosition(+1, +0),
                new GridPosition(-1, +0),
                new GridPosition(+0, +1),
                new GridPosition(+0, -1),

                new GridPosition(+1, +1),
                new GridPosition(-1, +1),
                new GridPosition(+1, -1),
                new GridPosition(-1, -1),
            };

            foreach (GridPosition neighbourGridOffset in neighbourGridOffsetList)
            {
                GridPosition neighbourGridPosition = unitGridPosition + neighbourGridOffset;
                if (LevelGrid.Instance.IsValidGridPosition(neighbourGridPosition))
                {
                    // Valid
                    Vector3 neighbourWorldPosition = LevelGrid.Instance.GetWorldPosition(neighbourGridPosition);
                    TestFogOfWarOnPosition(neighbourWorldPosition, ref revealedGridPositionList);
                }
            }
        }

        FogOfWarVisual.Instance.ShowGridPositions(revealedGridPositionList);

        // Show/Hide all Enemies in Revealed Grid Positions
        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
        {
            if (revealedGridPositionList.Contains(enemyUnit.GetGridPosition()))
            {
                // Enemy visible
                enemyUnit.ShowVisual();
            }
            else
            {
                // Enemy invisible
                enemyUnit.HideVisual();
            }
        }
    }

    private void TestFogOfWarOnPosition(Vector3 unitWorldPosition, ref List<GridPosition> revealedGridPositionList)
    {
        Vector3 baseDir = new Vector3(1, 0, 0);
        float angleIncrease = 10;
        for (float angle = 0; angle < 360; angle += angleIncrease)
        {
            Vector3 dir = ApplyRotationToVectorXZ(baseDir, angle);
            //Debug.DrawLine(unitWorldPosition, unitWorldPosition + dir * 14f, Color.green, 5f);

            float viewDistanceMax = 14f;
            float viewDistanceIncrease = .4f;
            for (float viewDistance = 0f; viewDistance < viewDistanceMax; viewDistance += viewDistanceIncrease)
            {
                Vector3 targetPosition = unitWorldPosition + dir * viewDistance;
                GridPosition targetGridPosition = LevelGrid.Instance.GetGridPosition(targetPosition);
                if (LevelGrid.Instance.IsValidGridPosition(targetGridPosition))
                {
                    // Valid Grid Position
                    CoverType coverType = LevelGrid.Instance.GetCoverTypeAtPosition(targetPosition);
                    if (coverType == CoverType.Full) break; // If coverType is Full, Unit cannot see further through it

                    if (coverType == CoverType.Environment) break; // If coverType is Wall, Unit cannot see further through it

                    if (!revealedGridPositionList.Contains(targetGridPosition))
                    {
                        // Position not yet tested
                        revealedGridPositionList.Add(targetGridPosition);
                    }
                }
            }
        }
    }

    public static Vector3 ApplyRotationToVectorXZ(Vector3 vec, float angle)
    {
        return Quaternion.Euler(0, angle, 0) * vec;
    }

    private void OnDestroy()
    {
        Cover.AfterAnyDestroyed -= Cover_AfterAnyDestroyed;
    }
}