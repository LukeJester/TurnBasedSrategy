using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechChassi : MonoBehaviour
{
    public Transform Mount_Top;

    public enum ChassiTypes
    {
        Spider,
        Tank,
        Buggies,
        LegsWithTred
    }
    [SerializeField] public ChassiTypes chassiTypes;
}
