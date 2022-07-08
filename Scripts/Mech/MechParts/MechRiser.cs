using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechRiser : MonoBehaviour
{

    [SerializeField] Transform Mount_cockpit;


    public enum WeaponTypes
    {
        Riser
    }
    
    [SerializeField] public WeaponTypes weaponTypes;
}
