using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float walkSpeed;
    public float jumpHeight;
    public float gravity;

    private CharacterController controller;
    private PlayerInput input;
    private PlayerClimbing climb;

    private float verticalVelocity;

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
        climb = GetComponent<PlayerClimbing>();
    }

    private void Inputs()
    {
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
        if(climb.isClimbing)
            return;

        var speed = isWalking ? walkSpeed : moveSpeed;

        Vector2 inputVector = input.actions["Movement"].ReadValue<Vector2>();
        Vector3 moveVector = transform.forward * inputVector.y + transform.right * inputVector.x;
        moveVector *= speed;

        controller.Move(moveVector * Time.deltaTime);
    }

    private IEnumerator Jump()
    {
        yield return new WaitForEndOfFrame();

        if (controller.isGrounded && !climb.isClimbing)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}
