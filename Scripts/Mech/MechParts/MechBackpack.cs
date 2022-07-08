using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechBackpack : MonoBehaviour
{
    public Transform Mount_Weapon_Top;
    public Transform Mount_Antenna;
    public Transform Mount_Wheel_Top;

    public enum BackpackTypes
    {
        GunPlatform,
        Ragular
    }

    [SerializeField] public BackpackTypes backpackTypes;
}
