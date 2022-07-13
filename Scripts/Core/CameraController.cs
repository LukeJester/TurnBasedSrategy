using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] float zoomSpeed = 5f;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    private const float MIN_FOLLOW_Y_OFFSET = 2f;
    private const float MAX_FOLLOW_Y_OFFSET = 12f;

    private CinemachineTransposer cinemachineTransposer;
    private Vector3 targetFollowOffset;

    private void Start()
    {
        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = cinemachineTransposer.m_FollowOffset;
    }

    void Update()
    {
        HandelMovment();

        HandelRotation();

        HandelZoom();
    }

    private void HandelMovment()
    {
        Vector2 inputMoveDirection = InputManager.Instance.GetCameraMoveVector();

        Vector3 moveVector = transform.forward * inputMoveDirection.y + transform.right * inputMoveDirection.x;
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }

    private void HandelRotation()
    {
        Vector3 reotationVector = new Vector3(0, 0, 0);

        reotationVector.y = InputManager.Instance.GetCameraRotateAmount();

        transform.eulerAngles += reotationVector * rotationSpeed * Time.deltaTime;
    }

    private void HandelZoom()
    {
        targetFollowOffset.y += InputManager.Instance.GetCameraZoomAmount();

        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);

        cinemachineTransposer.m_FollowOffset =
            Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * zoomSpeed);
    }
}
