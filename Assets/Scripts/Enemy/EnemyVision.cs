using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyVision : MonoBehaviour
{
    public float range = 10f;
    public float fov = 120f;
    public float checkFrequency = 0.2f;

    public LayerMask targetMask;
    public LayerMask obstMask;

    public bool inSight {  get; private set; }
    public float distToTarget {  get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(CheckRoutine());
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
                float _distToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, _distToTarget, obstMask))
                {
                    inSight = true;
                    distToTarget = _distToTarget; // only set when actually visible
                }
                else
                {
                    inSight = false;
                    distToTarget = 0f;
                }
            }
            else
            {
                inSight = false;
                distToTarget = 0f;
            }
        }
        else
        {
            inSight = false;
            distToTarget = 0f;
        }
    }
}