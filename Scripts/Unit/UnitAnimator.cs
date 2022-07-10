using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    //I think i might make this be a feild on the weapon script, that is called by this script thought the ation to the wepon script
    // [SerializeField] private Transform bulletProjectilePrefab;
    // [SerializeField] private Transform shootPointTransform;
    private Transform bulletProjectilePrefab;
    private Transform shootPointTransform;


    private void Awake()
    {
        //replace below code with this if you want later
        //child.TryGetComponent<MoveAction>(out MoveAction moveAction)

        MoveAction moveAction = GetComponentInChildren<MoveAction>();
        if (moveAction != null)
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += OnStopMoving;
        }

        ShootAction shootAction = GetComponentInChildren<ShootAction>();
        if (shootAction != null)
        {
            shootAction.OnShoot += ShootAction_OnShoot;
        }
    }

    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        animator.SetBool("isWalking", true);
    }

    private void OnStopMoving(object sender, EventArgs e)
    {
        animator.SetBool("isWalking", false);
    }

    private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        animator.SetTrigger("shoot");

        Transform bulletProjectileTransform =  Instantiate(e.bulletProjectilePrefab, e.shootPointTransform.position, Quaternion.identity);
        BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();
        
        Vector3 targetUnitShootAtPosition = e.targetUnit.GetWorldPosition();
        targetUnitShootAtPosition.y = e.shootPointTransform.position.y;
        bulletProjectile.SetUp(targetUnitShootAtPosition);
    }
}
