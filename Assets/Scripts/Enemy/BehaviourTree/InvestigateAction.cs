using System;
using System.Collections;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Investigate", story: "Investigate [LastKnownLoc] for [Player]", category: "Action", id: "f1bf24ff509d089b8da10685f2322540")]
public partial class InvestigateAction : Action
{
    // made with the help of claude.ai
    [SerializeReference] public BlackboardVariable<Vector3> LastKnownLoc;
    [SerializeReference] public BlackboardVariable<GameObject> Player;

    private float _rotationSpeed = 90f;
    private float _pauseDuration = 1f;
    private bool _isComplete;
    private MonoBehaviour _owner;

    private EnemyVision _vision;
    private EnemyAlertness _alertness;

    protected override Status OnStart()
    {
        _isComplete = false;
        _vision = GameObject.GetComponent<EnemyVision>();
        _alertness = GameObject.GetComponent<EnemyAlertness>();
        _owner = GameObject.GetComponent<MonoBehaviour>();
        _owner.StartCoroutine(InvestigateRoutine(_owner.transform, LastKnownLoc));
        return Status.Running;
    }
    protected override Status OnUpdate()
    {
        if (_vision.inSight && _alertness.alertness >= 0.8f)
            return Status.Success;

        return _isComplete ? Status.Success : Status.Running;
    }
    protected override void OnEnd()
    {
        _isComplete = false;
    }

    private IEnumerator InvestigateRoutine(Transform transform, Vector3 investigatePosition)
    {
        yield return RotateTowards(transform, investigatePosition);

        float firstAngle = UnityEngine.Random.Range(110f, 130f) * (UnityEngine.Random.value > 0.5f ? 1f : -1f);
        yield return RotateByAngle(transform, firstAngle);
        yield return new WaitForSeconds(_pauseDuration);

        float secondAngle = UnityEngine.Random.Range(220f, 260f) * -Mathf.Sign(firstAngle);
        yield return RotateByAngle(transform, secondAngle);
        yield return new WaitForSeconds(_pauseDuration);

        _isComplete = true;
    }
    private IEnumerator RotateTowards(Transform transform, Vector3 targetPosition)
    {
        Vector3 dir = (targetPosition - transform.position).normalized;
        dir.y = 0f;
        Quaternion targetRot = Quaternion.LookRotation(dir);

        while (Quaternion.Angle(transform.rotation, targetRot) > 0.5f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, _rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator RotateByAngle(Transform transform, float angle)
    {
        Quaternion targetRot = transform.rotation * Quaternion.Euler(0f, angle, 0f);

        while (Quaternion.Angle(transform.rotation, targetRot) > 0.5f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, _rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }
}

