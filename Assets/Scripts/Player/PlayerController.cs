using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum State
    {
        Standing,
        Crouching,
        Prone,
        Climbing,
        Carrying,
        Falling
    }

    public State state;

    private CharacterController cc; //charactercontroller
    private PlayerInput i; //playerinput
    private PlayerMovement pm; //playermovement

    private float targetHeight;
    private float heightLerpSpeed = 10f; // Speed of height transition (higher = faster)

    private bool isCrouching;
    private bool isProne;
    private bool isCarrying;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
        SetPlayerState(State.Standing); // Set default state
    }

    // Update is called once per frame
    void Update()
    {
        // Only check for falling if we have a reference to PlayerMovement
        if (pm == null) return;

        // Handle pose state changes (crouch/prone)
        HandlePoseInput();

        // Lerp the character controller height towards target height
        if (cc.height != targetHeight)
        {
            cc.height = Mathf.Lerp(cc.height, targetHeight, heightLerpSpeed * Time.deltaTime);
        }
    }

    private void Initialize()
    {
        cc = GetComponent<CharacterController>();
        i = GetComponent<PlayerInput>();
        pm = GetComponent<PlayerMovement>();

        i.actions["Crouch"].performed += ctx => HandleCrouchInput();
        i.actions["Prone"].performed += ctx => HandleProneInput();
    }

    private void HandleCrouchInput()
    {
        // Don't allow state changes while diving
        if (pm.isDiving) return;

        if (state == State.Standing)
        {
            SetPlayerState(State.Crouching);
        }
        else if (state == State.Crouching)
        {
            SetPlayerState(State.Standing);
        }
        else if (state == State.Prone)
        {
            SetPlayerState(State.Crouching);
        }
    }

    private void HandleProneInput()
    {
        // Don't allow state changes while diving
        if (pm.isDiving) return;

        if (state == State.Standing)
        {
            SetPlayerState(State.Prone);
        }
        else if (state == State.Prone)
        {
            SetPlayerState(State.Standing);
        }
        else if (state == State.Crouching)
        {
            SetPlayerState(State.Prone);
        }
    }

    private void HandlePoseInput()
    {
        // This is called in Update to handle pose state transitions
        // The actual input handling is done in the input callbacks above
    }

    public void SetPlayerState(State newState)
    {
        switch (newState)
        {
            case State.Standing:
                targetHeight = 2.0f;
                break;
            case State.Crouching:
                targetHeight = 1.0f;
                break;
            case State.Prone:
                targetHeight = 0.5f;
                break;
            case State.Climbing:
                // Set parameters for climbing
                break;
            case State.Carrying:
                // Set parameters for carrying
                break;
            case State.Falling:
                // Falling is now only a visual feature, no state logic needed
                break;
        }
        this.state = newState;
    }
}
