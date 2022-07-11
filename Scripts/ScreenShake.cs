using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class ScreenShake : MonoBehaviour
{

    public static ScreenShake Instance { get; private set; }

    //public event EventHandler

    private CinemachineImpulseSource cinemachineImpulseSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more then one ScreenShake! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Shake(float intesnity = 1f)
    {
        cinemachineImpulseSource.GenerateImpulse(intesnity);
    }
}
