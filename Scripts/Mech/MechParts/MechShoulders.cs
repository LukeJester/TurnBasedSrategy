using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechShoulders : MonoBehaviour
{

    [SerializeField] Transform Mount_cockpit;
    [SerializeField] Transform Mount_Weapon_L;
    [SerializeField] Transform Mount_Weapon_R;

    public enum WeaponTypes
    {
        Shoulders
    }
    
    [SerializeField] public WeaponTypes weaponTypes;
}
