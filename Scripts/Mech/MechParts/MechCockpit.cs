using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechCockpit : MonoBehaviour
{
    public Transform Mount_Top;
    public Transform Mount_Backpack;
    public Transform Mount_Weapon_Left;
    public Transform Mount_Weapon_Right;


    public enum CockpitTypes
    {
        Closed,
        GunPlatform
    }
    [SerializeField] public CockpitTypes cockpitTypes;
}
