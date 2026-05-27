using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public enum State
    {
        Standing,
        Crouching,
        Prone,
        Climbing,
        Carrying
    }

    public State state;

    private CharacterController controller;
    private PlayerInput input;
    private PlayerDiving diving;

    private float elapsedDiveTime;
    private float elapsedTime;
    private float stateChangeDelay = 0.25f; 
    private float verticalVelocity;

    public event System.Action OnDive;
    public event System.Action OnClimb;
    public event System.Action OnStateChange;

    private Transform mesh;
    public bool isAiming { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(this);
    }

    void Start()
    {

        Initialize();
    }

    public void Initialize()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();
        diving = GetComponent<PlayerDiving>();
        mesh = GetComponentInChildren<Animator>().transform;

        elapsedDiveTime = 0f;
        elapsedTime = 0f;

        SetPlayerState(State.Crouching);

        input.actions["Crouch"].performed += ctx => HandleCrouchInput();
        input.actions["Prone"].performed += ctx => HandleProneInput();
        input.actions["Climb/Dive"].performed += ctx => HandleClimbDiveInput(ctx);
        input.actions["SecondaryAction"].performed += ctx => isAiming = true;
        input.actions["SecondaryAction"].canceled += ctx => isAiming = false;
    }

    void Update()
    {
        elapsedDiveTime += Time.deltaTime;
        elapsedTime += Time.deltaTime;

        Gravity();
    }
    private void Gravity()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = 0;
        }
        else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    public void HandleClimbDiveInput(InputAction.CallbackContext ctx)
    {
        if (ctx.interaction is PressInteraction)
            OnClimb.Invoke();

        else if (ctx.interaction is MultiTapInteraction)
            OnDive.Invoke();
    }

    #region chatslop

    private void HandleCrouchInput()
    {
        if(elapsedTime < stateChangeDelay)
            return;

        if (diving.isDiving || state == State.Climbing) 
            return;

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
        
        elapsedTime = 0f;
    }

    private void HandleProneInput()
    {
        if (elapsedTime < stateChangeDelay)
            return;

        if (diving.isDiving || state == State.Carrying || state == State.Climbing) 
            return;

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

        elapsedTime = 0f;
    }

    #endregion

    public void SetPlayerState(State newState)
    {
        StopAllCoroutines();

        switch (newState)
        {
            case State.Standing:
                StartCoroutine(SetHeight(2.0f,0.5f)); 
                break;

            case State.Crouching:
                StartCoroutine(SetHeight(1.0f,0.5f)); 
                break;

            case State.Prone:
                StartCoroutine(SetHeight(0.5f,0.5f)); 
                break;

            case State.Climbing: 
                break;

            case State.Carrying: 
                StartCoroutine(SetHeight(2.0f,0.5f));
                break;
        }
        this.state = newState;
        OnStateChange.Invoke();
    }

    private IEnumerator SetHeight(float targetHeight, float speed)
    {
        elapsedDiveTime = 0f;

        while (elapsedDiveTime < speed)
        {
            elapsedDiveTime += Time.deltaTime;
            controller.height = Mathf.Lerp(controller.height, targetHeight, (elapsedDiveTime / speed));

            var targetOffset = new Vector3(0f, -(controller.height / 2f), 0f);
            mesh.localPosition = Vector3.Lerp(mesh.localPosition, targetOffset, (elapsedDiveTime / speed));
            yield return null;
        }

        controller.height = targetHeight; 
    }
}