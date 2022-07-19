using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] Transform rifleTransform;
    [SerializeField] Transform meleeWeaponTransform;

    private Unit unit;
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

        MeleeAction meleeAction = GetComponentInChildren<MeleeAction>();
        if (meleeAction != null)
        {
            meleeAction.OnMeleeActionStarted += meleeAction_OnMeleeActionStarted;
            meleeAction.OnMeleeActionCompleted += meleeAction_OnMeleeActionCompleted;
        }

        GrenadeAction grenadeAction = GetComponentInChildren<GrenadeAction>();
        if (grenadeAction != null)
        {
            grenadeAction.OnThrowGrenade += grenadeAction_OnThrowGrenade;
        }

        OverwatchAction overwatchAction = GetComponentInChildren<OverwatchAction>();
        if (overwatchAction != null)
        {
            overwatchAction.OnShoot += overwatchAction_OnShoot;
        }

        ReloadAction reloadAction = GetComponentInChildren<ReloadAction>();
        if (reloadAction != null)
        {
            reloadAction.OnReload += reloadAction_OnReload;
        }

        unit = GetComponent<Unit>();
        unit.OnCoverStateChanged += unit_OnCoverStateChanged;
    }

    private void Start()
    {
        EquipRife();
    }

    private void meleeAction_OnMeleeActionStarted(object sender, EventArgs e) // can pass in hit/miss and if miss, make target play dodge, if hit make target play hit animaiton
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
        animator.SetBool("inCover", false);
        animator.SetBool("isWalking", true);
    }

    private void OnStopMoving(object sender, EventArgs e)
    {
        animator.SetBool("isWalking", false);
    }

    private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        animator.SetTrigger("shoot");

        Vector3 targetUnitShootAtPosition = e.targetUnit.GetWorldPosition();
        targetUnitShootAtPosition.y = e.shootPointTransform.position.y;

        if (!e.hit)
        {
            // MISS!
            Vector3 missShootPosition = targetUnitShootAtPosition;
            Vector3 dirToMissShootPosition = (missShootPosition - e.shootPointTransform.transform.position).normalized;
            Vector3 missDir = Vector3.Cross(dirToMissShootPosition, Vector3.down);

            missShootPosition += missDir * .25f;
            dirToMissShootPosition = (missShootPosition - e.shootPointTransform.transform.position).normalized;

            targetUnitShootAtPosition = missShootPosition + dirToMissShootPosition * 40f;
        }
        
        // Vector3 targetUnitShootAtPosition = e.targetUnit.GetWorldPosition();
        // targetUnitShootAtPosition.y = e.shootPointTransform.position.y;
 
        Transform bulletProjectileTransform = Instantiate(e.bulletProjectilePrefab, e.shootPointTransform.position, Quaternion.identity);
        BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();

        
        bulletProjectile.SetUp(targetUnitShootAtPosition);
    }

    private void unit_OnCoverStateChanged(object sender, EventArgs e)
    {
        if (unit.GetCoverType() == CoverType.None)
        {
            animator.SetBool("inCover", false);
        }
        else
        {
            animator.SetBool("inCover", true);
        }
        
    }

    private void grenadeAction_OnThrowGrenade(object sender, EventArgs e)
    {
        //animator.SetTrigger("grenade");
        //will first instaniate it in hand position
    }

    private void overwatchAction_OnShoot(object sender, OverwatchAction.OnShootEventArgs e)
    {
        animator.SetTrigger("shoot");

        Transform bulletProjectileTransform = Instantiate(e.bulletProjectilePrefab, e.shootPointTransform.position, Quaternion.identity);
        BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();

        Vector3 targetUnitShootAtPosition = e.targetUnit.GetWorldPosition();
        targetUnitShootAtPosition.y = e.shootPointTransform.position.y;
        bulletProjectile.SetUp(targetUnitShootAtPosition);

        // Vector3 unitShootPosition = e.shotUnit.GetPosition();
        // unitShootPosition.y = shootPoint.position.y;

        // if (!e.hit)
        // {
        //     // MISS!
        //     Vector3 missShootPosition = unitShootPosition;
        //     Vector3 dirToMissShootPosition = (missShootPosition - shootPoint.position).normalized;
        //     Vector3 missDir = Vector3.Cross(dirToMissShootPosition, Vector3.down);

        //     missShootPosition += missDir * .25f;
        //     dirToMissShootPosition = (missShootPosition - shootPoint.position).normalized;

        //     unitShootPosition = missShootPosition + dirToMissShootPosition * 40f;
        // }

        // Instantiate(pfBulletProjectileRaycast, shootPoint.position, Quaternion.identity).GetComponent<BulletProjectileRaycast>()
        //     .Setup(unitShootPosition);
    }

    private void reloadAction_OnReload(object sender, EventArgs e)
    {
        animator.SetTrigger("reload");
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
