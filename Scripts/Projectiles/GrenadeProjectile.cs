using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GrenadeProjectile : MonoBehaviour
{

    public static event EventHandler OnAnyGrenadeExploaded;

    [SerializeField] float explosionRadius = 4;
    [SerializeField] int explosionDamage = 30;
    [SerializeField] Transform grenageExploadVFXPrefab;
    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] AnimationCurve arcYAnimationCurve;
    
    private Vector3 targetPosition; 
    private Action OnGrenadeBehaviorComplete;
    private float totalDistance;
    private Vector3 positionXZ;

    private void Update()
    {
        Vector3 moveDirection = (targetPosition - positionXZ).normalized;

        float moveSpeed = 15f;
        positionXZ += moveDirection * moveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(positionXZ, targetPosition);
        float distanceNormolized = 1 - distance/totalDistance;

        float maxHeight = totalDistance/4f;
        float positionY =  arcYAnimationCurve.Evaluate(distanceNormolized) * maxHeight;
        transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);

        float reachedTargetDistance = 0.2f;
        if(Vector3.Distance(positionXZ, targetPosition) < reachedTargetDistance)
        {
            Collider[] colliderArray = Physics.OverlapSphere(targetPosition, explosionRadius);
            
            foreach(Collider collider in colliderArray)
            {
                if(collider.TryGetComponent<Unit>(out Unit targetUnit))
                {
                    targetUnit.Damage(explosionDamage);
                }

                if (collider.TryGetComponent<IDestructable>(out IDestructable destructable))
                {
                    destructable.Destroy(OnGrenadeBehaviorComplete);
                }
            }

            OnAnyGrenadeExploaded?.Invoke(this, EventArgs.Empty);

            trailRenderer.transform.parent = null;

            Instantiate(grenageExploadVFXPrefab, targetPosition + Vector3.up * 1f, Quaternion.identity);
            
            Destroy(gameObject);

            OnGrenadeBehaviorComplete();
        }
    }

    public void Setup(GridPosition targetGridPosition, Action OnGrenadeBehaviorComplete)
    {
        this.OnGrenadeBehaviorComplete = OnGrenadeBehaviorComplete;
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);

        positionXZ = transform.position;
        positionXZ.y = 0;

        totalDistance = Vector3.Distance(positionXZ, targetPosition);
    }
}
