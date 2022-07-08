using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 200;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform bulletHitVFXPrefab;

    private Vector3 targetPosition; 

    void Update()
    {
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
    
        float distanceBeforeMoving = Vector3.Distance(transform.position, targetPosition);
        transform.position += moveDirection * bulletSpeed * Time.deltaTime;

        float distanceAfterMoving = Vector3.Distance(transform.position, targetPosition);

        if(distanceBeforeMoving < distanceAfterMoving)
        {
            transform.position = targetPosition;

            trailRenderer.transform.parent = null;

            Destroy(gameObject);

            Instantiate(bulletHitVFXPrefab, targetPosition, Quaternion.identity);
        }
    }

    public void SetUp(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }
}
