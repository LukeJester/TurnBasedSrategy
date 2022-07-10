using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : MonoBehaviour
{
    // Weapon Stats
    [SerializeField] int maxShootDistance = 4;
    [SerializeField] int weaponDamage = 50;

    //For SpawnedBullet
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform shootPointTransform;

    //For shooting multiple rockets, I can change the private Transform shootPointTransform; into a list and place the rocket positions inside of where the rockets will sit

    // Start is called before the first frame update 
    void Start()
    {
        
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

}
