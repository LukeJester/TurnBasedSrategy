using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject actionGameObject;

    private void Start()
    {
        BaseAction.OnAnyActionStart += BaseAction_OnAnyActionStart;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;
    }

    private void ShowActionCamera()
    {
        actionGameObject.SetActive(true);
    }

    private void HideActionCamera()
    {
        actionGameObject.SetActive(false);
    }

    private void BaseAction_OnAnyActionStart(object sender, EventArgs e)
    {
        switch(sender)
        {
            case ShootAction shootAction:
                Unit shooterUnit = shootAction.GetUnit();
                Unit targetUnit = shootAction.GetTargetUnit();

                Vector3 cameraCharacterheight = Vector3.up * 1.7f;

                Vector3 shootDirection = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition() ).normalized;

                float shoulderOffsetAmount = 0.5f;
                Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDirection * shoulderOffsetAmount;

                Vector3 actionCameraPosition = shooterUnit.GetWorldPosition() + cameraCharacterheight + shoulderOffset + (shootDirection * -1);

                actionGameObject.transform.position = actionCameraPosition;
                actionGameObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraCharacterheight);
                ShowActionCamera();
                break;
        }
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                HideActionCamera();
                break;
        }
    }

}
