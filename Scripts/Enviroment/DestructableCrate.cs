using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DestructableCrate : MonoBehaviour, IDestructable
{

    public static event EventHandler OnAnyDestroyed;
    public static event EventHandler AfterAnyDestroyed;
    public static event EventHandler OnAnyPlacment;

    [SerializeField] Transform crateDestroyedPrefab;

    private GridPosition gridPosition;

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

        OnAnyPlacment?.Invoke(this, EventArgs.Empty);
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public void Destroy(Action OnInteractionComplete)
    {
        Transform crateDestroyedTransform = Instantiate(crateDestroyedPrefab, transform.position, transform.rotation);

        ApplyExploasionToChildren(crateDestroyedTransform, 150f, transform.position, 10);
        Destroy(gameObject);

        OnAnyDestroyed?.Invoke(this, EventArgs.Empty);
    }

    private void ApplyExploasionToChildren(Transform root, float exploasionForce, Vector3 exploaionposition, float exploaionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(exploasionForce, exploaionposition, exploaionRange);
            }

            ApplyExploasionToChildren(child, exploasionForce, exploaionposition, exploaionRange);
        }
    }

    private void OnDestroy()
    {
        AfterAnyDestroyed?.Invoke(this, EventArgs.Empty);
    }
}