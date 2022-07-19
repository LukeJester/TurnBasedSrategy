using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    // Weapon Stats
    [SerializeField] int maxRange = 1;
    [SerializeField] int weaponDamage = 100;
    [SerializeField] int APCost = 1;
    [SerializeField] int accuracy = 95;
    // hit sound
    // hit effect
    //animation overide for different weapons

    public int GetWeaponDamage()
    {
        return weaponDamage;
    }

    public int GetWeaponRange()
    {
        return maxRange;
    }

    public int GetAPCost()
    {
        return APCost;
    }

    public int GetAccuracy()
    {
        return accuracy;
    }
}
