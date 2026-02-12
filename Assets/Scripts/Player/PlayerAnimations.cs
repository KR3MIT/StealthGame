using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimations : MonoBehaviour
{
    public float animSmoothSpeed = 10f;

    private Animator animator;
    private PlayerInput input;
    private CharacterController controller;
    private PlayerMovement playerMovement;

    private float smoothMoveX;
    private float smoothMoveY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
        Inputs();
    }

    // Update is called once per frame
    void Update()
    {
        MovementAnim();
    }

    private void Initialize()
    {
        animator = GetComponentInChildren<Animator>();
        input = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Inputs()
    {
        input.actions["Jump/Climb"].performed += ctx => Jump();
    }

    private void Jump()
    {
        if(controller.isGrounded)
            animator.SetTrigger("Jump");
    }

    private void MovementAnim()
    {
        animator.SetBool("IsGrounded", controller.isGrounded);
        animator.SetBool("isWalking", playerMovement.isWalking);

        var speedModifier = playerMovement.isWalking ? 0f : 1f;

        Vector2 inputVector = input.actions["Movement"].ReadValue<Vector2>();

        animator.SetBool("IsMoving", inputVector.magnitude > 0.1f);

        float targetX = inputVector.x + inputVector.x * speedModifier;
        float targetY = inputVector.y + inputVector.y * speedModifier;

        smoothMoveX = Mathf.Lerp(smoothMoveX, targetX, animSmoothSpeed * Time.deltaTime);
        smoothMoveY = Mathf.Lerp(smoothMoveY, targetY, animSmoothSpeed * Time.deltaTime);

        animator.SetFloat("MoveX", smoothMoveX);
        animator.SetFloat("MoveY", smoothMoveY);
    }
}
