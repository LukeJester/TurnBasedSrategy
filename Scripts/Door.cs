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

    private void Awake()
    {
        animator = GetComponent<Animator>();
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
        animator.SetBool("isOpen", isOpen);
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, true);
    }

    private void CloseDoor()
    {
        isOpen = false;
        animator.SetBool("isOpen", isOpen);
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false);

    }
}
