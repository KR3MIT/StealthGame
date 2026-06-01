using Unity.Behavior;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //made with the help of claude.ai
    public float alertRadius = 20f;
    private BehaviorGraphAgent _agent;

    public LayerMask enemyMask;

    void Start()
    {
        _agent = GetComponent<BehaviorGraphAgent>();
    }

    public void AlertOthers()
    {
        Debug.Log("alerting others");
        transform.GetChild(0).GetComponent<Animator>().SetTrigger("AlertOthers");

        _agent.BlackboardReference.GetVariableValue("LastKnownLoc", out Vector3 lastKnownLoc);

        Collider[] colliders = Physics.OverlapSphere(transform.position, alertRadius, enemyMask);
        Debug.Log($"Found {colliders.Length} colliders in range");

        foreach (Collider col in colliders)
        {
            EnemyController enemy = col.GetComponentInParent<EnemyController>();
            if (enemy != null && enemy != this)
                enemy.GetAlerted(lastKnownLoc);
        }
    }

    public void GetAlerted(Vector3 lastKnownLoc)
    {
        Debug.Log("alerted by other");
        _agent.BlackboardReference.GetVariableValue("AlertState", out AlertState state);
        if (state == AlertState.Alert) return; // don't overwrite if already alerted

        _agent.BlackboardReference.SetVariableValue("LastKnownLoc", lastKnownLoc);
        _agent.BlackboardReference.SetVariableValue("AlertState", AlertState.Alert);
    }
}