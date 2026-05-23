using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
public class PlayerDiving : MonoBehaviour
{
    public float diveVelocity = 25f;
    public float diveAirDrag = 3f;
    public float diveGroundDrag = 5f;
    public float diveUpwardVelocity = 30f;

    public bool isDiving { get; private set; }

    public event System.Action OnDive;

    private CharacterController controller;
    private PlayerInput input;
    private PlayerController playerController;
    private PlayerMovement movement;

    private Transform cameraOffset;

    private float checkLandDelay = 0.1f;
    private float elapsedTime;
    private float minDiveVelocity = 2.1f;

    private Vector3 diveVelocityVector;

    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDiving)
        {
            ApplyDiveMovement();
            CheckDiveLanding();
        }
    }
    public void Initialize()
    {
        isDiving = false;

        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();
        playerController = GetComponent<PlayerController>();
        movement = GetComponent<PlayerMovement>();
        cameraOffset = transform.GetChild(0);


        Inputs();
    }
    private void Inputs()
    {
        input.actions["Jump/Dive"].performed += ctx => StartCoroutine(Dive());
    }

    private void ApplyDiveMovement()
    {
        var drag = controller.isGrounded ? diveGroundDrag : diveAirDrag;

        diveVelocityVector *= Mathf.Max(0, 1 - drag * Time.deltaTime);
        controller.Move(diveVelocityVector * Time.deltaTime);
    }

    private void CheckDiveLanding()
    {
        if (elapsedTime < checkLandDelay) 
        {
            elapsedTime += Time.deltaTime; 
            return; 
        }

        if (diveVelocityVector.magnitude < minDiveVelocity)
        {
            isDiving = false;
            diveVelocityVector = Vector3.zero;
        }
    }

    private IEnumerator Dive()
    {
        elapsedTime = 0f;

        yield return new WaitForEndOfFrame();

        if (playerController.state != PlayerController.State.Standing && playerController.state != PlayerController.State.Crouching)
        {
            yield break;
        }

        if (isDiving)
        {
            yield break;
        }

        playerController.SetPlayerState(PlayerController.State.Prone);

        isDiving = true;

        Vector2 inputVector = input.actions["Movement"].ReadValue<Vector2>();
        Vector3 forward = cameraOffset.forward;
        forward.y = 0;
        forward = forward.normalized;

        Vector3 right = cameraOffset.right;
        Vector3 diveDirection = forward * inputVector.y + right * inputVector.x;

        // If no input, dive forward
        if (diveDirection.magnitude < 0.1f)
            diveDirection = forward * diveVelocity;
        else
            diveDirection = diveDirection.normalized * diveVelocity;

        diveVelocityVector = diveDirection;
        movement.currentVelocity = Vector3.zero;

        OnDive?.Invoke();
    }
}
