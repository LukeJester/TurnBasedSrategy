using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechController : MonoBehaviour
{
    [SerializeField] Animator animator;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            animator.SetBool("isWalkingForwards", true) ;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            animator.SetBool("isWalkingBackwards", true);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            animator.SetBool("isWalkingLeft", true);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            animator.SetBool("isWalkingRight", true);
        }
        else
        {
            animator.SetBool("isWalkingForwards", false);
            animator.SetBool("isWalkingBackwards", false);
            animator.SetBool("isWalkingLeft", false);
            animator.SetBool("isWalkingRight", false);
        }
    }
}
