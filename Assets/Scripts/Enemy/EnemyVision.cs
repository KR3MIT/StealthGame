using System.Collections;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public float range = 10f;
    public float FOV = 120f;
    public float checkFrequency = 0.2f;

    public LayerMask TargetMask;
    public LayerMask obstMask;

    public bool inSight;

    public float alertness { get; private set; }
    public float alertnessRate = 1f;

    private float maxAlertness = 100f;
    private float alertnessModifier;

    private GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();

        StartCoroutine(CheckRoutine());
    }
    private void Initialize()
    {
        player = PlayerController.instance.gameObject;
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
        Collider[] rangeCheck = Physics.OverlapSphere(transform.position, range, TargetMask);

        if (rangeCheck.Length != 0)
        {
            Transform target = rangeCheck[0].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < FOV / 2)
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
}
