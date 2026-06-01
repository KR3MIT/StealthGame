using System;
using System.Collections;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Investigate", story: "Investigate [LastKnownLoc] for [Player]", category: "Action", id: "f1bf24ff509d089b8da10685f2322540")]
public partial class InvestigateAction : Action
{
    [SerializeReference] public BlackboardVariable<Vector3> LastKnownLoc;
    [SerializeReference] public BlackboardVariable<float> PauseDuration;
    [SerializeReference] public BlackboardVariable<float> RotationSpeed;
    [SerializeReference] public BlackboardVariable<float> Threshold;

    private bool _isComplete;
    private bool _isNavigating;
    private MonoBehaviour _owner;
    private NavMeshAgent _agent;
    private EnemyVision _vision;
    private EnemyAlertness _alertness;
    private Coroutine _routine;

    protected override Status OnStart()
    {
        _isComplete = false;
        _isNavigating = true;
        _vision = GameObject.GetComponent<EnemyVision>();
        _alertness = GameObject.GetComponent<EnemyAlertness>();
        _agent = GameObject.GetComponent<NavMeshAgent>();
        _owner = GameObject.GetComponent<MonoBehaviour>();

        // navigate to last known location first
        _agent.isStopped = false;
        _agent.SetDestination(LastKnownLoc.Value);

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // player found during investigation
        if (_vision.inSight && _alertness.alertness >= Threshold.Value)
        {
            if (_routine != null) _owner.StopCoroutine(_routine);
            return Status.Success;
        }

        // still navigating to last known location
        if (_isNavigating)
        {
            if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                _isNavigating = false;
                _agent.isStopped = true;
                _agent.ResetPath();
                _routine = _owner.StartCoroutine(InvestigateRoutine(_owner.transform));
            }
            return Status.Running;
        }

        return _isComplete ? Status.Failure : Status.Running;
    }

    protected override void OnEnd()
    {
        _isComplete = false;
        _isNavigating = false;
        if (_routine != null)
        {
            _owner.StopCoroutine(_routine);
            _routine = null;
        }
        if (_agent != null)
        {
            _agent.isStopped = true;
            _agent.ResetPath();
        }
    }

    private IEnumerator InvestigateRoutine(Transform t)
    {
        yield return new WaitForSeconds(PauseDuration.Value);

        float firstAngle = UnityEngine.Random.Range(110f, 130f) * (UnityEngine.Random.value > 0.5f ? 1f : -1f);
        yield return RotateByAngle(t, firstAngle);
        yield return new WaitForSeconds(PauseDuration.Value);

        float secondAngle = UnityEngine.Random.Range(220f, 260f) * -Mathf.Sign(firstAngle);
        yield return RotateByAngle(t, secondAngle);
        yield return new WaitForSeconds(PauseDuration.Value);

        _isComplete = true;
    }

    private IEnumerator RotateByAngle(Transform t, float angle)
    {
        Quaternion targetRot = t.rotation * Quaternion.Euler(0f, angle, 0f);

        while (Quaternion.Angle(t.rotation, targetRot) > 0.5f)
        {
            t.rotation = Quaternion.RotateTowards(t.rotation, targetRot, RotationSpeed.Value * Time.deltaTime);
            yield return null;
        }
    }
}