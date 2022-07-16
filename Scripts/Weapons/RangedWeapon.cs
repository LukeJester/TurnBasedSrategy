using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : MonoBehaviour
{
    // Weapon Stats
    [SerializeField] int maxShootDistance = 4;
    [SerializeField] int weaponDamage = 50;
    [SerializeField] int APCost = 1;
    [SerializeField] int accuracy = 95;
    [SerializeField] int reloadAPCost = 1;
    [SerializeField] int maxAmmoPerClip = 5;

    //For SpawnedBullet
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform shootPointTransform;
    //musssle flach VFX , can be smoke for rocket
    // shoot sound
    //animation overide for different weapons

    private int currentAmmoInClip;

    //For shooting multiple rockets, I can change the private Transform shootPointTransform; into a list and place the rocket positions inside of where the rockets will sit
   
    private void Awake()
    {
        currentAmmoInClip = maxAmmoPerClip; // make sure that reenabling it/switching weapons does not reset the AP
    }

    public int GetWeaponDamage()
    {
        return weaponDamage;
    }

    public int GetWeaponRange()
    {
        return maxShootDistance;
    }

    public Transform GetBulletProjectilePrefab()
    {
        return bulletProjectilePrefab;
    }

    public Transform GetShootPointTransform()
    {
        return shootPointTransform;
    }

    public int GetAPCost()
    {
        return APCost;
    }

    public int GetReloadAPCost()
    {
        return reloadAPCost;
    }

    public int GetAccuracy()
    {
        return accuracy;
    }

    public void SubtractAmmoFromClip(int ammoSpent) // need to check for negative ammo
    {
        currentAmmoInClip -= ammoSpent;
        if (currentAmmoInClip <= 0)
            Debug.Log("Out of Ammo");
    }

    public bool HasEnoughAmmoToShoot()
    {
        if (currentAmmoInClip <= 0)
            return false;
        else return true;
    }

    public void Reload()
    {
        currentAmmoInClip = maxAmmoPerClip;
    }

    public bool HasFullClip()
    {
        if (currentAmmoInClip == maxAmmoPerClip)
            return true;
        else return false;
    }
}
