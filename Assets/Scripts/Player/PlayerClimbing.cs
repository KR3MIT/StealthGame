using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class PlayerClimbing : MonoBehaviour
{
    public LayerMask mask;
    public float climbSpeed = 5f;
    public float vaultSpeed = 2f;

    public event System.Action OnClimb;
    public event System.Action StopClimb;

    public bool isClimbing { get; private set; } = false;
    public float climbType { get; private set; }
    public Quaternion climbRotation { get; private set; }

    private PlayerInput input;
    private CharacterController controller;
    private PlayerController playerController;
    
    private Transform cameraOffset;

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        input = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>();
        cameraOffset = transform.GetChild(0);

        Inputs();
    }

    private void Inputs()
    {
        input.actions["Jump/Dive"].performed += ctx => ClimbCheck();
        input.actions["Movement"].performed += ctx => CancelClimb();
    }

    private void CancelClimb()
    {
        if (!isClimbing) 
            return;

        Vector2 inputVector = input.actions["Movement"].ReadValue<Vector2>();

        var currentPos = transform.position;

        if (inputVector.y < 0f)
        {
            StopAllCoroutines();
            isClimbing = false;
            StopClimb?.Invoke();
            StartCoroutine(SetStopPosition(currentPos, 1f));
        }
    }

    private IEnumerator SetStopPosition(Vector3 pos, float duration)
    {
        controller.enabled = false;

        float elapsedTime = 0f;

        var backwardsOffset = controller.radius * -transform.forward;
        var heightOffset = (controller.height / 2) * transform.up;
        var endPos = backwardsOffset + heightOffset;

        yield return new WaitForEndOfFrame();

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(pos, endPos, (elapsedTime / duration));

            elapsedTime += Time.deltaTime;
        }

        yield return new WaitForEndOfFrame();

        controller.enabled = true;
    }

    private void ClimbCheck()
    {
        if (playerController.state != PlayerController.State.Standing && playerController.state != PlayerController.State.Crouching)
            return;

        // Check if there's space above the player to climb
        var upCheckPos = transform.localPosition + new Vector3(0f, controller.height/2, 0f);
        Physics.Raycast(upCheckPos, transform.up, out RaycastHit upInfo, 1f, mask);

        if (upInfo.collider != null)
            return;

        // Check for a ledge in front of the player
        var forward = (transform.rotation = Quaternion.Euler(0, cameraOffset.rotation.eulerAngles.y, 0)).normalized;
        var offset = Vector3.zero;

        var heightRay = Physics.Raycast(transform.localPosition + offset, -transform.up, out RaycastHit hitInfo, 5f, mask);
        Debug.DrawRay(transform.localPosition + offset, -transform.up * hitInfo.distance, Color.red, 5f);

        Debug.Log(hitInfo.distance);

        // Check if the ledge is climbable based on distance
        if (!isClimbing)
        {
            switch (hitInfo.distance)
            {
                case > 1f:
                    Debug.Log("climbing");
                    break;
                
            }
        }
    }

    private IEnumerator Climb(Vector3 inputPos)
    {
        playerController.state = PlayerController.State.Climbing;

        climbType = 0;
        OnClimb?.Invoke();
        transform.rotation = Quaternion.Euler(0, cameraOffset.rotation.eulerAngles.y, 0);

        float elapsedTime = 0f;

        var currentPos = transform.position;

        var heightOffset = inputPos + (controller.height/2 + 0.1f) * transform.up;

        var forwardOffset = inputPos + transform.forward * 0.25f;

        var heightPos = currentPos + heightOffset;

        // Phase 1: Move from currentPos to heightPos (climb up)
        while (elapsedTime < climbSpeed / 2f)
        {
            transform.position = Vector3.Lerp(currentPos, heightPos, (elapsedTime / (climbSpeed / 2f)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = heightPos;
        currentPos = transform.position;
        elapsedTime = 0f;

        var forwardPos = currentPos + forwardOffset;

        // Phase 2: Move from heightPos to forwardPos (move forward onto ledge)
        while (elapsedTime < climbSpeed / 2f)
        {
            transform.position = Vector3.Lerp(currentPos, forwardPos, (elapsedTime / (climbSpeed / 2f)));
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = forwardPos;

        yield return new WaitForEndOfFrame();

        controller.enabled = true;
        isClimbing = false;

        playerController.state = PlayerController.State.Standing;
    }

    private IEnumerator Vault(Vector3 inputPos)
    {
        yield return null;
    }

    //private IEnumerator Climb(Vector3 inputPos)
    //{
    //    climbType = 0;
    //    OnClimb?.Invoke();

    //    var currentPos = transform.position;

    //    var heightOffset = (controller.height/2 + 0.25f) * transform.up;
    //    var forwardOffset = transform.forward * 0.25f;
    //    var offset = heightOffset + forwardOffset;

    //    // Calculate startPos based on ledge position, not current player position
    //    var ledgeHeight = inputPos.y - controller.height/2;
    //    var startPos = new Vector3(inputPos.x, ledgeHeight, inputPos.z) - forwardOffset;

    //    var endPos = inputPos + offset;

    //    float elapsedTime = 0f;

    //    // Phase 1: Move from currentPos to startPos (snap to ledge)
    //    while (elapsedTime < climbSpeed / 2f)
    //    {
    //        transform.position = Vector3.Lerp(currentPos, startPos, (elapsedTime / (climbSpeed / 2f)));

    //        elapsedTime += Time.deltaTime;

    //        yield return null;
    //    }

    //    transform.position = startPos;
    //    elapsedTime = 0f;

    //    // Phase 2: Move from startPos to endPos (climb up and over)
    //    while (elapsedTime < climbSpeed)
    //    {
    //        transform.position = Vector3.Lerp(startPos, endPos, (elapsedTime / (climbSpeed)));

    //        elapsedTime += Time.deltaTime;

    //        yield return null;
    //    }

    //    transform.position = endPos;

    //    controller.enabled = true;
    //    isClimbing = false;
    //}

    //private IEnumerator Vault(Vector3 inputPos)
    //{
    //    OnClimb?.Invoke();

    //    var currentPos = transform.position;

    //    var heightOffset = transform.up * 0.5f;
    //    var offset = heightOffset;

    //    var middlePos = inputPos + offset;


    //    float elapsedTime = 0f;

    //    // Phase 1: Move from currentPos to startPos (snap to ledge)
    //    while (elapsedTime < climbSpeed / 2f)
    //    {
    //        transform.position = Vector3.Lerp(currentPos, middlePos, (elapsedTime / (climbSpeed / 2f)));

    //        elapsedTime += Time.deltaTime;

    //        yield return null;
    //    }

    //    transform.position = middlePos;
    //    elapsedTime = 0f;

    //    if (!canLand)
    //    {
    //        while (elapsedTime < climbSpeed / 2f)
    //        {
    //            transform.position = Vector3.Lerp(middlePos, endPos, (elapsedTime / (climbSpeed)));

    //            elapsedTime += Time.deltaTime;

    //            yield return null;
    //        }

    //        transform.position = endPos;
    //    }
    //    else
    //    {
    //        transform.position = middlePos;
    //    }

    //    controller.enabled = true;
    //    isClimbing = false;
    //}
}
