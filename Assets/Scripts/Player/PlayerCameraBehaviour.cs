using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;


public class PlayerCameraBehaviour : MonoBehaviour
{
    public float zoomedFov = 60f;
    public float lerpSpeed = 5f;

    private CinemachineCamera cineCamera;
    private PlayerInput input;

    private float defaultFov;
    private Coroutine lerpFovCoroutine;

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        cineCamera = GetComponentInChildren<CinemachineCamera>();
        input = GetComponent<PlayerInput>();

        defaultFov = cineCamera.Lens.FieldOfView;

        Inputs();
    }

    private void Inputs()
    {
        input.actions["SecondaryAction"].performed += ctx => SetFov(zoomedFov);
        input.actions["SecondaryAction"].canceled += ctx => SetFov(defaultFov);
    }

    public void SetFov(float fov)
    {
        if (lerpFovCoroutine != null)
            StopCoroutine(lerpFovCoroutine);

        lerpFovCoroutine = StartCoroutine(LerpFov(fov));
    }

    private IEnumerator LerpFov(float targetFov)
    {
        while (Mathf.Abs(cineCamera.Lens.FieldOfView - targetFov) > 0.01f)
        {
            var lens = cineCamera.Lens;
            lens.FieldOfView = Mathf.Lerp(lens.FieldOfView, targetFov, lerpSpeed * Time.deltaTime);
            cineCamera.Lens = lens;
            yield return null;
        }

        var finalLens = cineCamera.Lens;
        finalLens.FieldOfView = targetFov;
        cineCamera.Lens = finalLens;
        lerpFovCoroutine = null;
    }
}
