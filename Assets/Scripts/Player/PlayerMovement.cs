using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float walkSpeed;
    public float jumpHeight;
    public float gravity;
    public float acceleration = 10f; // Smoothing speed - higher = faster acceleration

    public bool canJump;

    public event System.Action OnJump;

    private CharacterController controller;
    private PlayerInput input;
    private PlayerClimbing climbing;

    private float verticalVelocity;
    private Vector3 currentVelocity; // Current smoothed velocity

    public bool isWalking {  get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
        Inputs();
    }

    // Update is called once per frame
    void Update()
    {
        if (!controller.enabled)
            return;

        Movement();
        Gravity();
    }

    private void Initialize()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();
        climbing = GetComponent<PlayerClimbing>();
    }

    private void Inputs()
    {
        if(canJump)
            input.actions["Jump/Climb"].performed += ctx => StartCoroutine(Jump());
        
        input.actions["Walk"].performed += ctx => isWalking = true;
        input.actions["Walk"].canceled += ctx => isWalking = false;
    }

    private void Gravity()
    {
        verticalVelocity += gravity * Time.deltaTime;
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    private void Movement()
    {
        if(climbing.isClimbing)
        {
            currentVelocity = Vector3.zero; // Reset velocity when climbing
            return;
        }

        var speed = isWalking ? walkSpeed : moveSpeed;

        Vector2 inputVector = input.actions["Movement"].ReadValue<Vector2>();
        Vector3 targetVelocity = transform.forward * inputVector.y + transform.right * inputVector.x;
        targetVelocity *= speed;

        // Smoothly interpolate from current velocity to target velocity
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, acceleration * Time.deltaTime);

        controller.Move(currentVelocity * Time.deltaTime);
    }

    private IEnumerator Jump()
    {
        yield return new WaitForEndOfFrame();

        if (controller.isGrounded && !climbing.isClimbing)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            OnJump?.Invoke();
        }
    }
}
