using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerClimbing : MonoBehaviour
{
    public LayerMask mask;
    public float climbSpeed = 5f;
    public float vaultSpeed = 2f;

    public event System.Action OnClimb;
    public event System.Action StopClimb;

    public bool isClimbing { get; private set; } = false;
    public float climbType { get; private set; }
    public Vector3 flatDir { get; private set; }
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
        playerController.OnClimb += () => ClimbCheck();
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
            StopClimb?.Invoke();
            StopAllCoroutines();
            StartCoroutine(SetStopPosition(currentPos, 1f));
        }
    }

    private IEnumerator SetStopPosition(Vector3 pos, float duration)
    {
        controller.enabled = false;

        float elapsedTime = 0f;

        var backwardsOffset = controller.radius * -flatDir;
        var heightOffset = (controller.height / 2) * transform.up;
        var endPos = backwardsOffset + heightOffset;

        yield return new WaitForEndOfFrame();

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(pos, endPos, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
        }

        yield return new WaitForEndOfFrame();

        isClimbing = false;

        playerController.state = PlayerController.State.Standing;

        controller.enabled = true;
    }

    private void ClimbCheck()
    {
        if (playerController.state != PlayerController.State.Standing && playerController.state != PlayerController.State.Crouching)
            return;

        // Check if there's space above the player to climb
        var upCheckPos = transform.localPosition + new Vector3(0f, controller.height / 2, 0f);
        Physics.Raycast(upCheckPos, transform.up, out RaycastHit upInfo, 1f, mask);
        Debug.DrawRay(upCheckPos, transform.up, Color.blue, controller.height / 2);

        if (upInfo.collider != null)
            return;

        // Flatten the camera's forward to the horizontal plane only
        Vector3 flatForward = cameraOffset.forward;
        flatForward.y = 0f;
        flatForward.Normalize();

        flatDir = flatForward;

        var heightOffset = transform.up * 3f;

        var offset = (flatDir * (controller.radius * 2f)) + heightOffset;

        var heightRay = Physics.Raycast(transform.localPosition + offset, -transform.up, out RaycastHit hitInfo, 5f, mask);
        Debug.DrawRay(transform.localPosition + offset, -transform.up * hitInfo.distance, Color.red, 5f);

        var wallHeight = hitInfo.distance;

        // Check if the ledge is climbable based on distance
        if (!isClimbing || heightRay == false)
        {
            if (wallHeight < 0.5f)
                return;
            
            else if (wallHeight >= 1f && wallHeight < 2.5f)
                StartCoroutine(Climb(hitInfo.point, climbSpeed, 0));
            
            else if (wallHeight >= 2.5f && wallHeight < 3.5f)
                StartCoroutine(Climb(hitInfo.point, vaultSpeed, 1));
            
            else
                return;
        }
    }

    private IEnumerator Climb(Vector3 inputPos, float climbSpeed, int climbtype)
    {
        var inputState = playerController.state;

        playerController.state = PlayerController.State.Climbing;

        climbType = climbtype;
        OnClimb?.Invoke();

        float elapsedTime = 0f;

        var currentPos = transform.position;

        var forwardOffset = inputPos - (flatDir * controller.radius * 2f);

        forwardOffset.y = 0f;

        var heightOffset = transform.up * (inputPos.y + ((controller.height / 2f) + 0.1f));

        var heightPos = forwardOffset + heightOffset;

        // Phase 1: Move from currentPos to heightPos (climb up)
        while (elapsedTime < climbSpeed)
        {
            transform.position = Vector3.Lerp(currentPos, heightPos, (elapsedTime / climbSpeed));

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = heightPos;

        var forwardPos = inputPos + (transform.up * ((controller.height / 2) + 0.1f));

        elapsedTime = 0f;

        // Phase 2: Move from heightPos to endPos (climb over)
        while (elapsedTime < (climbSpeed / 2f))
        {
            transform.position = Vector3.Lerp(heightPos, forwardPos, (elapsedTime / (climbSpeed / 2f)));

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = forwardPos;

        yield return new WaitForEndOfFrame();

        controller.enabled = true;
        isClimbing = false;
        playerController.state = inputState;
    }

    //yield return new WaitForEndOfFrame();

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
