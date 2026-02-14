using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerClimbing : MonoBehaviour
{
    public LayerMask mask;
    public float climbSpeed = 2f;
    public float vaultSpeed = 1f;

    public event System.Action OnClimb;
    public event System.Action StopClimb;

    private PlayerInput input;
    private CharacterController controller;
    public bool isClimbing { get; private set; } = false;
    public Quaternion climbRotation { get; private set; } // Store climbing direction

    void Start()
    {
        Initialize();
        Inputs();
    }

    private void Initialize()
    {
        input = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
    }

    private void Inputs()
    {
        input.actions["Jump/Climb"].performed += ctx => ClimbCheck();
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
            StartCoroutine(SetStopPosition(currentPos));
        }
    }

    private IEnumerator SetStopPosition(Vector3 pos)
    {
        controller.enabled = false;
        yield return new WaitForEndOfFrame();
        var backwardsOffset = -transform.forward * 0.5f;
        var heightOffset = (controller.height / 2) * transform.up;
        var offset = backwardsOffset + heightOffset;
        transform.position = pos + offset;
        controller.enabled = true;
    }

    private void ClimbCheck()
    {
        var checkPos = transform.localPosition + new Vector3(0f, controller.height/2, 0f);
        Physics.Raycast(checkPos, transform.up, out RaycastHit upInfo, 1f, mask);

        Debug.DrawRay(checkPos, transform.up * upInfo.distance, Color.blue, 4);

        if (upInfo.collider != null)
            return;

        var offset = transform.rotation * new Vector3(0f, 3f, 0.5f);
        
        var heightRay = Physics.Raycast(transform.localPosition + offset, -transform.up, out RaycastHit hitInfo, 5f, mask);

        Debug.DrawRay(transform.localPosition + offset, -transform.up * hitInfo.distance, Color.red, 4f);

        if (!isClimbing) 
        { 
            switch (hitInfo.distance)
            {
                case > 3.5f:
                    break;
                case > 3f:

                    break;
                case > 1f:
                    isClimbing = true;
                    climbRotation = transform.rotation; // Lock current rotation for climbing
                    controller.enabled = false;
                    StartCoroutine(Climb(hitInfo.point));
                    break;
                case < 0.1f:
                    break;
            }
        }
    }

    private IEnumerator Climb(Vector3 inputPos)
    {
        OnClimb?.Invoke();

        var currentPos = transform.position;

        var heightOffset = (controller.height/2 + 0.25f) * transform.up;
        var forwardOffset = transform.forward * 0.25f;
        var offset = heightOffset + forwardOffset;

        // Calculate startPos based on ledge position, not current player position
        var ledgeHeight = inputPos.y - controller.height/2;
        var startPos = new Vector3(inputPos.x, ledgeHeight, inputPos.z) - forwardOffset;
        
        var endPos = inputPos + offset;

        float elapsedTime = 0f;

        // Phase 1: Move from currentPos to startPos (snap to ledge)
        while (elapsedTime < climbSpeed / 2f)
        {
            transform.position = Vector3.Lerp(currentPos, startPos, (elapsedTime / (climbSpeed / 2f)));
            
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }

        transform.position = startPos;
        elapsedTime = 0f;

        // Phase 2: Move from startPos to endPos (climb up and over)
        while (elapsedTime < climbSpeed)
        {
            transform.position = Vector3.Lerp(startPos, endPos, (elapsedTime / (climbSpeed)));
            
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }

        transform.position = endPos;

        controller.enabled = true;
        isClimbing = false;
    }

    private IEnumerator Vault(Vector3 inputPos)
    {
        var currentLoc = transform.position;

        var targetPos = new Vector3(currentLoc.x, inputPos.y, currentLoc.z);

        var heightOffset = (controller.height / 2) * transform.up;

        float elapsedTime = 0f;

        isClimbing = true;
        controller.enabled = false;

        while (elapsedTime < vaultSpeed)
        {
            transform.position = Vector3.Lerp(currentLoc, targetPos + heightOffset, (elapsedTime / vaultSpeed));

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = inputPos;

        controller.enabled = true;
        isClimbing = false;
    }
}
