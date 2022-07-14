using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Door : MonoBehaviour, IInteractable
{

    [SerializeField] private bool isOpen = false;

    private GridPosition gridPosition;
    private Animator animator;
    private Action OnInteractionComplete;
    private float timer;
    private bool isActive;
    private BoxCollider boxCollider;
    private bool canInteract = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);

        if (isOpen)
        {
            OpenDoor();
        }
        else 
        {
            CloseDoor();
        }
    }

    private void Update()
    {
        if(!isActive)
            return;

        timer -= Time.deltaTime;

        if(timer <= 0f)
        {
            isActive = false;
            OnInteractionComplete();
        }
    }

    public void Interact(Action OnInteractComplete)
    {
        this.OnInteractionComplete = OnInteractComplete;
        isActive = true;
        timer = 0.5f;

        if (isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }

    }

    private void OpenDoor()
    {
        isOpen = true;
        boxCollider.enabled = false;
        animator.SetBool("isOpen", isOpen);
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, true);
    }

    private void CloseDoor()
    {
        isOpen = false;
        boxCollider.enabled = true;
        animator.SetBool("isOpen", isOpen);
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false);

    }

    public bool CanInteract()
    {
        if (isOpen)
        {
            TestIfDoorIsBlocked();
        }
        else
        {
            canInteract = true;
        }

        return canInteract;
    }

    private void TestIfDoorIsBlocked()
    {
        GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        float rayCastOffsetDistance = 5f;

        RaycastHit rayOut;
        Physics.Raycast(worldPosition + Vector3.down * rayCastOffsetDistance, Vector3.up, out rayOut);
        if (rayOut.collider != null)
        {
            canInteract = false;
        }
        else
        {
            canInteract = true;
        }  
    }
}
