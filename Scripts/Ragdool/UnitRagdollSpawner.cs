using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitRagdollSpawner : MonoBehaviour
{
    [SerializeField] private Transform ragdollPrefab;
    [SerializeField] private Transform originalRootBones;

    private HealthSystem healthSystem;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();

        healthSystem.OnDead += healthSystem_OnDead;
    }

    private void healthSystem_OnDead(object sender, EventArgs e)
    {
        Transform ragDollTransform =  Instantiate(ragdollPrefab, transform.position, transform.rotation);
        UnitRagdoll unitRagdoll = ragDollTransform.GetComponent<UnitRagdoll>();
        unitRagdoll.SetUp(originalRootBones);
    }
}
