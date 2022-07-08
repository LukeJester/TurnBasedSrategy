using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechWeapon : MonoBehaviour
{
    public enum WeaponTypes
    {
        DoubleGun,
        GrenadeLauncher,
        Shocker,
        ShockRifle,
        Sniper
    }
    
    [SerializeField] public WeaponTypes weaponTypes;
}
