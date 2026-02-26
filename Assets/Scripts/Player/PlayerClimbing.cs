using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerClimbing : MonoBehaviour
{
    public LayerMask mask;
    public float climbSpeed = 2f;
    public float vaultSpeed = 1f;

    [HideInInspector] public int climbType = 0;

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

        var offset = transform.rotation * new Vector3(0f, 3f, 0.75f);
        
        var heightRay = Physics.Raycast(transform.localPosition + offset, -transform.up, out RaycastHit hitInfo, 5f, mask);

        Debug.DrawRay(transform.localPosition + offset, -transform.up * hitInfo.distance, Color.red, 4f);

        bool canLand = false;

        var offsetTwo = transform.rotation * new Vector3(0f, controller.height / 2, 1.75f);

        var heightRayTwo = Physics.Raycast(transform.localPosition + offsetTwo, -transform.up, out RaycastHit hitInfoTwo, 2f, mask);

        Debug.DrawRay(transform.localPosition + offsetTwo, -transform.up * 2f, Color.orange, 2f);

        if(hitInfoTwo.collider != null) {canLand = true;}

        var endPos = transform.localPosition + offsetTwo - new Vector3(0f, controller.height / 2, 0f);

        if (!isClimbing)
        {
            switch (hitInfo.distance)
            {
                case > 3.5f:
                    break;
                case > 3f:
                    isClimbing = true;
                    climbRotation = transform.rotation;
                    controller.enabled = false;
                    StartCoroutine(Vault(hitInfo.point, endPos, canLand));
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
        climbType = 0;
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

    private IEnumerator Vault(Vector3 inputPos,Vector3 exitPos, bool canLand)
    {
        climbType = canLand ? 1 : 2;
        OnClimb?.Invoke();

        var currentPos = transform.position;

        var heightOffset = transform.up * 0.5f;
        var forwardOffset = canLand ? (transform.forward * 0.25f) : (-transform.forward * 0.5f);
        var offset = heightOffset + forwardOffset;

        var middlePos = inputPos + offset;

        var endPos = exitPos + ((transform.forward * 1.25f) + -transform.up * 0.25f);

        float elapsedTime = 0f;

        // Phase 1: Move from currentPos to startPos (snap to ledge)
        while (elapsedTime < climbSpeed / 2f)
        {
            transform.position = Vector3.Lerp(currentPos, middlePos, (elapsedTime / (climbSpeed / 2f)));

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = middlePos;
        elapsedTime = 0f;

        if (!canLand)
        {
            while (elapsedTime < climbSpeed / 2f)
            {
                transform.position = Vector3.Lerp(middlePos, endPos, (elapsedTime / (climbSpeed)));

                elapsedTime += Time.deltaTime;

                yield return null;
            }

            transform.position = endPos;
        }
        else
        {
            transform.position = middlePos;
        }

        controller.enabled = true;
        isClimbing = false;
    }
}
