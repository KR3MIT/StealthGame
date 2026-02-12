using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerClimbing : MonoBehaviour
{
    public LayerMask environmentMask;

    private PlayerInput input;
    private PlayerMovement playerMovement;
    private CharacterController controller;

    public bool isClimbing { get; private set; } = false;

    void Start()
    {
        Initialize();
        Inputs();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Initialize()
    {
        input = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
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
        if (inputVector.y > -0.1f)
        {
            StopAllCoroutines();
            playerMovement.enabled = true;
        }
        Debug.Log("Cancel Climb");
    }

    private void ClimbCheck()
    {
        var offset = transform.rotation * new Vector3(0f, 3f, 0.75f);
        
        var heightRay = Physics.Raycast(transform.localPosition + offset, -transform.up, out RaycastHit hitInfo, 5f, environmentMask);

        Debug.DrawRay(transform.localPosition + offset, -transform.up * hitInfo.distance, Color.red, 4f);

        Debug.Log(hitInfo.distance);

        if (!isClimbing) { 
            switch (hitInfo.distance)
            {
                case > 3.5f:
                    Debug.Log("just move");
                    break;
                case > 3f:
                    Debug.Log("vault");
                    break;
                case > 1f:
                    Debug.Log("climb");
                    break;
                case < 0.1f:
                    Debug.Log("cant");
                    break;
            }
        }
    }

    private IEnumerator Climb()
    {
        Debug.Log("climbing");
        isClimbing = true;
        yield return new WaitForSeconds(2f);
        isClimbing = false;

    }

    private IEnumerator Vault()
    {
        yield return new WaitForSeconds(1f);
    }
}
