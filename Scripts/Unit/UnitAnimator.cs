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
    [SerializeField] Transform rifleTransform;
    [SerializeField] Transform meleeWeaponTransform;

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

        MeleeAction meleeAction = GetComponentInChildren<MeleeAction>();
        if (meleeAction != null)
        {
            meleeAction.OnMeleeActionStarted += meleeAction_OnMeleeActionStarted;
            meleeAction.OnMeleeActionCompleted += meleeAction_OnMeleeActionCompleted;
        }
    }

    private void Start()
    {
        EquipRife();
    }

    private void meleeAction_OnMeleeActionStarted(object sender, EventArgs e)
    {
        EquipMeleeWeapon();
        animator.SetTrigger("melee");
    }

    private void meleeAction_OnMeleeActionCompleted(object sender, EventArgs e)
    {
        EquipRife();
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

   private void EquipMeleeWeapon()
   {
        meleeWeaponTransform.gameObject.SetActive(true);
        rifleTransform.gameObject.SetActive(false);
   }

    private void EquipRife()
    {
        rifleTransform.gameObject.SetActive(true);
        meleeWeaponTransform.gameObject.SetActive(false);
    }
}
