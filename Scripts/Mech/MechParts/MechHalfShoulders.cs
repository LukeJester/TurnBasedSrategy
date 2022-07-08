using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechHalfShoulders : MonoBehaviour
{

    [SerializeField] Transform Mount_Weapon;

    public enum WeaponTypes
    {
        HalfShoulder,
        ShoulderShield
    }
    
    [SerializeField] public WeaponTypes weaponTypes;
}
