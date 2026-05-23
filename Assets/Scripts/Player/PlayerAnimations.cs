using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private Animator animator;
    private Transform mesh;
    private CharacterController controller;
    private PlayerController playerController;
    private PlayerMovement movement;
    private PlayerClimbing climbing;
    private PlayerDiving diving;

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
        animator = GetComponentInChildren<Animator>();
        mesh = animator.transform;
        controller = GetComponent<CharacterController>();
        movement = GetComponent<PlayerMovement>();
        climbing = GetComponent<PlayerClimbing>();
        playerController = GetComponent<PlayerController>();
        diving = GetComponent<PlayerDiving>();
    }

    private void Inputs()
    {
        diving.OnDive += () => animator.SetTrigger("Dive");
        climbing.OnClimb += () => animator.SetTrigger("Climb");
        climbing.StopClimb += () => animator.SetTrigger("StopClimb");
    }

    private void SetStates()
    {
        bool isCrouching = playerController.state == PlayerController.State.Crouching;
        bool isProne = playerController.state == PlayerController.State.Prone;
        bool isClimbing = playerController.state == PlayerController.State.Climbing;
        bool isCarrying = playerController.state == PlayerController.State.Carrying;

        animator.SetBool("isCrouching", isCrouching);
        animator.SetBool("isProne", isProne);
        animator.SetBool("isClimbing", isClimbing);
        animator.SetBool("isCarrying", isCarrying);
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
