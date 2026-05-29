using System.Collections;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public float range = 10f;
    public float fov = 120f;
    public float checkFrequency = 0.2f;

    public LayerMask targetMask;
    public LayerMask obstMask;
    
    public bool inSight { get; private set; }

    public float alertness { get; private set; }
    public float alertnessRate = 1f;
    public float alertnessLossDelay = 1f;
    public float alertnessLossRate = 10f;


    private float maxAlertness = 100f;
    private float alertnessModifier;
    private float sightLostTimer = 0f;

    private Material gizmoMat;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();

        StartCoroutine(CheckRoutine());
    }

    void Update()
    {
        HandleAlertness();
    }

    private void Initialize()
    {
        gizmoMat = transform.GetChild(1).GetComponent<MeshRenderer>().material;
    }

    private IEnumerator CheckRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(checkFrequency);

        while (true)
        {
            yield return wait;
            FOVCheck();
        }
    }

    private void FOVCheck()
    {
        Collider[] rangeCheck = Physics.OverlapSphere(transform.position, range, targetMask);

        if (rangeCheck.Length != 0)
        {
            Transform target = rangeCheck[0].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < fov / 2)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstMask))
                    inSight = true;
                else
                    inSight = false;
            }
            else
                inSight = false;
        }
        else if (inSight)
            inSight = false;
    }

    private void HandleAlertness()
    {
        alertnessModifier = PlayerController.instance.visibility + GameManager.instance.alertnessVisibility;

        if (inSight)
        {
            sightLostTimer = 0f;
            alertness += alertnessRate * alertnessModifier * Time.deltaTime;
        }
        else
        {
            sightLostTimer += Time.deltaTime;
            if (sightLostTimer >= alertnessLossDelay)
                alertness -= alertnessRate * alertnessLossRate * Time.deltaTime;
        }

        alertness = Mathf.Clamp(alertness, 0, maxAlertness);
        gizmoMat.SetFloat("_Procentage", alertness / maxAlertness);
    }
}
