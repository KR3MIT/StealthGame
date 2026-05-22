using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float walkSpeed;
    public float proneSpeed;
    public float diveVelocity;
    public float diveDrag = 0.5f; // Friction that slows down dive over time (lower = faster dive)
    public float diveUpwardVelocity = 0.2f; // Upward component of dive (lower = flatter trajectory)
    public float minDiveVelocity = 0.5f; // Minimum velocity to stop dive (backup check)
    public float minMovementVelocityForDive = 2f; // Minimum movement speed required to dive (prevents instant dive stop when slow)
    public float gravity;
    public float acceleration = 10f;

    public event System.Action OnDive;

    private CharacterController cc; //CharacterController
    private PlayerInput i; //PlayerInput
    private PlayerController pc; //PlayerController

    private Transform co; //cameraOffset

    private float mdv = 0.5f; // Minimum velocity to stop dive (backup check)
    private float vv; //verticalVelocity
    private float s; //speed

    private Vector3 currentVelocity { get; set; }
    private Vector3 diveVelocityVector; // Independent dive velocity
    public bool isDiving { get; private set; }
    public bool isWalking { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
        Inputs();

        isDiving = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!cc.enabled) return;

        if (isDiving) 
        { 
            ApplyDiveMovement();
            CheckDiveLanding();
        }
        else
        {
            Movement();
        }

        Gravity();
    }

    private void Initialize()
    {
        cc = GetComponent<CharacterController>();
        i = GetComponent<PlayerInput>();
        pc = GetComponent<PlayerController>();
        co = transform.GetChild(0);
    }

    private void DiveCheck()
    {
        if (cc.isGrounded)
        {
            isDiving = false;
            currentVelocity = Vector3.zero;
        }
    }

    private void ApplyDiveMovement()
    {
        // Apply drag/friction to the dive velocity
        diveVelocityVector *= Mathf.Max(0, 1 - diveDrag * Time.deltaTime);

        // Move based on independent dive velocity (ignores input)
        cc.Move(diveVelocityVector * Time.deltaTime);
    }

    private void CheckDiveLanding()
    {
        // End dive if grounded or velocity is below minimum threshold
        if (cc.isGrounded || diveVelocityVector.magnitude < minDiveVelocity)
        {
            isDiving = false;
            diveVelocityVector = Vector3.zero;
            // Don't reset currentVelocity here - let player have immediate control
            vv = 0;
            // Stay in Prone state, let player control transitions via input
        }
    }

    private void Inputs()
    {
        i.actions["Jump/Dive"].performed += ctx => StartCoroutine(Dive());
        i.actions["Walk"].performed += ctx => isWalking = true;
        i.actions["Walk"].canceled += ctx => isWalking = false;
    }

    private void Gravity()
    {
        if (cc.isGrounded)
        {
            vv = 0;
        }
        else
        {
            vv += gravity * Time.deltaTime;
        }
        cc.Move(Vector3.up * vv * Time.deltaTime);
    }

    private void Movement()
    {
        if(pc.state == PlayerController.State.Climbing)
        {
            currentVelocity = Vector3.zero;
            return;
        }

        switch (pc.state)
        {
            case PlayerController.State.Standing:
                s = isWalking? walkSpeed : moveSpeed;
                break;
            case PlayerController.State.Crouching:
                s = walkSpeed; 
                break;
            case PlayerController.State.Prone:
                s = proneSpeed;
                break;
        }

        Vector2 inputVector = i.actions["Movement"].ReadValue<Vector2>();

        Vector3 forward = co.forward;   
        forward.y = 0;
        forward = forward.normalized;

        Vector3 right = co.right;

        Vector3 targetVelocity = forward * inputVector.y + right * inputVector.x;
        targetVelocity *= s;

        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, acceleration * Time.deltaTime);

        cc.Move(currentVelocity * Time.deltaTime);
    }

    private IEnumerator Dive()
    {
        yield return new WaitForEndOfFrame();

        // Only allow dive from Standing or Crouching
        if(pc.state != PlayerController.State.Standing && pc.state != PlayerController.State.Crouching)
        {
            yield break;
        }

        if(isDiving)
        {
            yield break;
        }

        // Require minimum movement velocity to perform dive
        if (currentVelocity.magnitude < minMovementVelocityForDive)
        {
            yield break;
        }

        pc.SetPlayerState(PlayerController.State.Prone);

        isDiving = true;

        Vector2 inputVector = i.actions["Movement"].ReadValue<Vector2>();

        // Use only horizontal directions, ignore vertical camera rotation
        Vector3 forward = co.forward;
        forward.y = 0;
        forward = forward.normalized;

        Vector3 right = co.right;

        Vector3 diveDirection = forward * inputVector.y + right * inputVector.x;

        // If no input, dive forward
        if (diveDirection.magnitude < 0.1f)
        {
            diveDirection = forward * diveVelocity;
        }
        else
        {
            diveDirection = diveDirection.normalized * diveVelocity;
        }

        // Set independent dive velocity
        vv = diveUpwardVelocity; // Add upward velocity component (lower = flatter dive)
        diveVelocityVector = diveDirection;
        currentVelocity = Vector3.zero; // Reset normal movement velocity

        OnDive?.Invoke();
    }
}
