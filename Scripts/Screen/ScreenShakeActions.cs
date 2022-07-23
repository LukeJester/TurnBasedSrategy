using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScreenShakeActions : MonoBehaviour
{
    private void Start()
    {
        ShootAction.OnAnyShoot += ShootAction_OnAnyShoot;
        GrenadeProjectile.OnAnyGrenadeExploaded += GrenadeProjectile_OnAnyGrenadeExploaded;
        MeleeAction.OnAnyMelleHit += MeleeAction_OnAnyMelleHit;
    }

    private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        ScreenShake.Instance.Shake();
    }

    private void GrenadeProjectile_OnAnyGrenadeExploaded(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(5f);
    }

    private void MeleeAction_OnAnyMelleHit(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(2);
    }

}
