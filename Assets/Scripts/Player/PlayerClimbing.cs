using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerClimbing : MonoBehaviour
{
    public LayerMask mask;
    public float climbTime = 2f;
    public float vaultTime = 1f;

    private PlayerInput input;
    private CharacterController controller;

    public float climbDistance { get; private set; }
    public bool isClimbing { get; private set; } = false;

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

            StartCoroutine(SetStopPosition(currentPos));
        }
    }

    private IEnumerator SetStopPosition(Vector3 pos)
    {
        controller.enabled = false;
        yield return new WaitForEndOfFrame();
        transform.position = pos + new Vector3 (0, (controller.height / 2), 0);
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

        climbDistance = hitInfo.distance;

        if (!isClimbing) 
        { 
            switch (hitInfo.distance)
            {
                case > 3.5f:
                    break;
                case > 3f:
                    StartCoroutine(Vault(hitInfo.point));
                    break;
                case > 1f:
                    StartCoroutine(Climb(hitInfo.point));
                    break;
                case < 0.1f:
                    break;
            }
        }
    }

    private IEnumerator Climb(Vector3 inputPos)
    {
        var currentLoc = transform.position;

        var targetPos = new Vector3(currentLoc.x, inputPos.y, currentLoc.z);

        var heightOffset = (controller.height/2 + 0.25f) * transform.up;
        var forwarwdOffset =  transform.forward * 0.25f;

        var offset = heightOffset + forwarwdOffset;

        float elapsedTime = 0f;

        isClimbing = true;
        controller.enabled = false;

        while (elapsedTime < climbTime)
        {
            transform.position = Vector3.Lerp(currentLoc, targetPos + heightOffset, (elapsedTime / climbTime));
            
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }

        transform.position = inputPos + offset;

        controller.enabled = true;
        isClimbing = false;
    }

    private IEnumerator Vault(Vector3 inputPos)
    {
        var currentLoc = transform.position;

        var targetPos = new Vector3(currentLoc.x, inputPos.y, currentLoc.z);

        var heightOffset = (controller.height / 2 + 0.25f) * transform.up;
        var forwarwdOffset = transform.forward * 0.25f;

        var offset = heightOffset + forwarwdOffset;

        float elapsedTime = 0f;

        isClimbing = true;
        controller.enabled = false;

        while (elapsedTime < vaultTime)
        {
            transform.position = Vector3.Lerp(currentLoc, targetPos + heightOffset, (elapsedTime / vaultTime));

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = inputPos + offset;

        controller.enabled = true;
        isClimbing = false;
    }
}
