using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
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
    private PlayerMovement movement;
    private PlayerDiving diving;
    private PlayerClimbing climbing;

    private float elapsedDiveTime;
    private float elapsedTime;
    private float stateChangeDelay = 0.25f; 
    private float verticalVelocity;

    void Start()
    {

        Initialize();
    }

    public void Initialize()
    {

        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();
        movement = GetComponent<PlayerMovement>();
        diving = GetComponent<PlayerDiving>();
        climbing = GetComponent<PlayerClimbing>();

        elapsedDiveTime = 0f;
        elapsedTime = 0f;

        SetPlayerState(State.Standing);

        input.actions["Crouch"].performed += ctx => HandleCrouchInput();
        input.actions["Prone"].performed += ctx => HandleProneInput();
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

                break;
        }
        this.state = newState;
    }

    private IEnumerator SetHeight(float targetHeight, float speed)
    {
        elapsedDiveTime = 0f;

        while (elapsedDiveTime < speed)
        {
            elapsedDiveTime += Time.deltaTime;
            controller.height = Mathf.Lerp(controller.height, targetHeight, (elapsedDiveTime / speed));
            yield return null;
        }

        controller.height = targetHeight; 
    }
}