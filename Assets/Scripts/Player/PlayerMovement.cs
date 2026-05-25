using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float walkSpeed;
    public float proneSpeed;
    public float acceleration = 10f;

    private CharacterController controller;
    private PlayerInput input; 
    private PlayerController playerController;
    private PlayerDiving diving;

    private Transform cameraOffset;

    public float speed;

    [HideInInspector]
    public Vector3 currentVelocity;

    public bool isWalking { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (!diving.isDiving && (
            playerController.state == PlayerController.State.Crouching || 
            playerController.state == PlayerController.State.Standing || 
            playerController.state == PlayerController.State.Prone )) 
            Movement();
    }

    public void Initialize()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();
        playerController = GetComponent<PlayerController>();
        diving = GetComponent<PlayerDiving>();
        cameraOffset = transform.GetChild(0);

        Inputs();
    }

    private void Inputs()
    {
        input.actions["Walk"].performed += ctx => isWalking = true;
        input.actions["Walk"].canceled += ctx => isWalking = false;
    }

    private void Movement()
    {
        if(playerController.state == PlayerController.State.Climbing)
        {
            currentVelocity = Vector3.zero;
            return;
        }

        switch (playerController.state)
        {
            case PlayerController.State.Standing:
                speed = isWalking? walkSpeed : moveSpeed;
                break;
            case PlayerController.State.Crouching:
                speed = walkSpeed; 
                break;
            case PlayerController.State.Prone:
                speed = proneSpeed;
                break;
            case PlayerController.State.Carrying:
                speed = walkSpeed;
                break;
        }

        Vector2 inputVector = input.actions["Movement"].ReadValue<Vector2>();
        Vector3 forward = cameraOffset.forward;   
        forward.y = 0;
        forward = forward.normalized;

        Vector3 right = cameraOffset.right;

        Vector3 targetVelocity = forward * inputVector.y + right * inputVector.x;
        targetVelocity *= speed;

        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, acceleration * Time.deltaTime);

        controller.Move(currentVelocity * Time.deltaTime);
    }
}
