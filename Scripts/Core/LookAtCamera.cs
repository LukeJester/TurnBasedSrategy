using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] bool invert;

    private Transform cameraTransform;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        if(invert)
        {
            Vector3 directionToCamera = (cameraTransform.position - transform.position).normalized;

            transform.LookAt(transform.position + directionToCamera * -1);
        }
        else
        {
            transform.LookAt(cameraTransform);
        }
    }
}
