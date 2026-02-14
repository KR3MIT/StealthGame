using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerCameraRotation : MonoBehaviour
{
    public float sensitivity;
    public float minPitch = -80f;
    public float maxPitch = 80f;
    public float lerpSpeed;
    public float meshRotationSpeed = 10f; // Speed for mesh to follow camera rotation

    private Transform cameraOffset;
    private Transform playerMesh;

    private CinemachineCamera cineCamera;
    private PlayerInput input;
    private PlayerClimbing climbing;
    private Animator animator;
    private Coroutine handednessCoroutine;

    private float pitch;
    private float cameraOffsetX;
    private bool rightHanded;

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        CameraRotation();
        UpdateMeshRotation();
    }

    private void Initialize()
    {
        cameraOffset = transform.GetChild(0);
        cineCamera = GetComponentInChildren<CinemachineCamera>();
        input = GetComponent<PlayerInput>();
        climbing = GetComponent<PlayerClimbing>();
        animator = GetComponentInChildren<Animator>();
        playerMesh = animator.transform;

        Cursor.lockState = CursorLockMode.Locked;

        cameraOffsetX = Mathf.Abs(cineCamera.transform.localPosition.x);

        input.actions["CameraHandedness"].performed += ctx => CameraHandedness();
    }

    public void CameraHandedness(bool handedness)
    {
        rightHanded = handedness;
        ApplyHandedness();
    }

    public void CameraHandedness()
    {
        rightHanded = !rightHanded;
        ApplyHandedness();
    }

    private void ApplyHandedness()
    {
        Vector3 pos = cineCamera.transform.localPosition;
        Vector3 targetPos = new Vector3(rightHanded ? cameraOffsetX : -cameraOffsetX, pos.y, pos.z);

        if (handednessCoroutine != null)
            StopCoroutine(handednessCoroutine);

        handednessCoroutine = StartCoroutine(LerpHandedness(targetPos));
    }

    private IEnumerator LerpHandedness(Vector3 targetPos)
    {
        Transform camTransform = cineCamera.transform;

        while (Vector3.Distance(camTransform.localPosition, targetPos) > 0.001f)
        {
            camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, targetPos, lerpSpeed * Time.deltaTime);
            yield return null;
        }

        camTransform.localPosition = targetPos;
        handednessCoroutine = null;
    }

    private void CameraRotation()
    {
        Vector2 inputVector = input.actions["Camera"].ReadValue<Vector2>();

        float yaw = inputVector.x * sensitivity * Time.deltaTime;
        pitch -= inputVector.y * sensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Always rotate the parent for camera control
        transform.Rotate(Vector3.up, yaw);

        // Camera pitch always works independently
        cameraOffset.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void UpdateMeshRotation()
    {
        if (climbing.isClimbing)
        {
            // If climbing, lock mesh to climb direction
            playerMesh.rotation = climbing.climbRotation;
        }
        else
        {
            // Smoothly rotate mesh to match parent rotation (camera yaw direction)
            Quaternion targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            playerMesh.rotation = Quaternion.Lerp(playerMesh.rotation, targetRotation, meshRotationSpeed * Time.deltaTime);
        }
    }
}
