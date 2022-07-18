using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MouseWorld : MonoBehaviour
{
    public static MouseWorld instance;

    public static event  EventHandler OnMousesCurentGridPositionChange;

    GridPosition gridPosition;
    GridPosition oldGridPosition;

    [SerializeField] private LayerMask mousePlaneLayerMask;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        transform.position = MouseWorld.GetPosition();

        GetMousesCurentGridPosition();

        if (gridPosition != oldGridPosition)
        {
            oldGridPosition = gridPosition;
            OnMousesCurentGridPositionChange?.Invoke(this, EventArgs.Empty);
        }

        //DrawWireSphere(Vector3 center, float radius);
    }
    
    public static Vector3 GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.mousePlaneLayerMask);
        return raycastHit.point;
    }

    public GridPosition GetMousesCurentGridPosition()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        return gridPosition;
    }

    
}
