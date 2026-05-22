using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerAnimations : MonoBehaviour
{
    private Animator a; //animator
    private Transform m; //playerMesh
    private CharacterController cc; //characterController
    private PlayerMovement pm; //movement
    private PlayerClimbing c; //climbing
    private PlayerController pc; //playerController

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
        Inputs();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        SetStates();
    }

    private void Initialize()
    {
        a = GetComponentInChildren<Animator>();
        m = a.transform;
        cc = GetComponent<CharacterController>();
        pm = GetComponent<PlayerMovement>();
        c = GetComponent<PlayerClimbing>();
        pc = GetComponent<PlayerController>();
    }

    private void Inputs()
    {
        pm.OnDive += () => a.SetTrigger("Dive");
        c.OnClimb += () => a.SetTrigger("Climb");
        c.StopClimb += () => a.SetTrigger("StopClimb");
    }

    private void SetStates()
    {
        bool isCrouching = pc.state == PlayerController.State.Crouching;
        bool isProne = pc.state == PlayerController.State.Prone;
        bool isClimbing = pc.state == PlayerController.State.Climbing;
        bool isCarrying = pc.state == PlayerController.State.Carrying;
        bool isFalling = pc.state == PlayerController.State.Falling;

        a.SetBool("isCrouching", isCrouching);
        a.SetBool("isProne", isProne);
        a.SetBool("isClimbing", isClimbing);
        a.SetBool("isCarrying", isCarrying);
        a.SetBool("isFalling", isFalling);
    }

    private void Movement()
    {

        //if (c.isClimbing)
        //    return;
        
        //var speedModifier = pm.isWalking ? 0f : 1f;

        //Vector2 inputVector = i.actions["Movement"].ReadValue<Vector2>();

        //a.SetBool("IsMoving", inputVector.magnitude > 0.1f);

        //float targetX = inputVector.x + inputVector.x * speedModifier;
        //float targetY = inputVector.y + inputVector.y * speedModifier;

        //smoothMoveX = Mathf.Lerp(smoothMoveX, targetX, animSmoothSpeed * Time.deltaTime);
        //smoothMoveY = Mathf.Lerp(smoothMoveY, targetY, animSmoothSpeed * Time.deltaTime);

        //a.SetFloat("MoveX", smoothMoveX);
        //a.SetFloat("MoveY", smoothMoveY);
    }
}
