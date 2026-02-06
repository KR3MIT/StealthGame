using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float jumpHeight;
    public float gravity = -20f;

    private CharacterController controller;
    private PlayerInput input;
    private float verticalVelocity;

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
        Gravity();
    }

    private void Initialize()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();
    }

    private void Inputs()
    {
        input.actions["Jump/Climb"].performed += ctx => Jump();
    }

    private void Gravity()
    {
        verticalVelocity += gravity * Time.deltaTime;
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    private void Movement()
    {
        Vector2 inputVector = input.actions["Movement"].ReadValue<Vector2>();
        Vector3 moveVector = transform.forward * inputVector.y + transform.right * inputVector.x;
        moveVector *= moveSpeed;

        controller.Move(moveVector * Time.deltaTime);
    }

    private void Jump()
    {
        if(controller.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}
