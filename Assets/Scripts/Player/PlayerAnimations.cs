using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimations : MonoBehaviour
{
    private Animator animator;
    private Transform mesh;
    private CharacterController controller;
    private PlayerController playerController;
    private PlayerMovement movement;
    private PlayerClimbing climbing;
    private PlayerDiving diving;
    private PlayerInput input;
    private Transform cameraOffset;

    private float rotationSpeed = 720f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerController.isAiming)
            AimMovement();
        else
            Movement();
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
        input = GetComponent<PlayerInput>();
        cameraOffset = transform.GetChild(0);

        Inputs();
    }

    private void Inputs()
    {
        diving.OnDive += () => animator.SetTrigger("Dive");
        climbing.OnClimb += () => animator.SetTrigger("Climb");
        climbing.StopClimb += () => animator.SetTrigger("StopClimb");

        input.actions["SecondaryAction"].performed += ctx => SetAim();
        input.actions["SecondaryAction"].canceled += ctx => SetAim();
        playerController.OnStateChange += () => SetStance();
    }

    private void SetStance()
    {
        animator.SetBool("isCrouching", playerController.state == PlayerController.State.Crouching);
        animator.SetBool("isProne", playerController.state == PlayerController.State.Prone);
        animator.SetBool("isClimbing", playerController.state == PlayerController.State.Climbing);
        animator.SetBool("isCarrying", playerController.state == PlayerController.State.Carrying);
        animator.SetBool("isGrounded", controller.isGrounded);
    }

    private void SetAim()
    {
        animator.SetBool("isAiming", playerController.isAiming);
    }

    private void Movement()
    {
        Vector3 velocity = movement.currentVelocity;

        Vector3 flatVelocity = new Vector3(velocity.x, 0f, velocity.z);

        if (flatVelocity.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatVelocity);
            mesh.rotation = Quaternion.RotateTowards(mesh.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

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

    private void AimMovement()
    {
        Vector3 forward = cameraOffset.forward;
        forward.y = 0;
        forward = forward.normalized;

        Quaternion targetRotation = Quaternion.LookRotation(forward);
        mesh.rotation = Quaternion.RotateTowards(mesh.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
