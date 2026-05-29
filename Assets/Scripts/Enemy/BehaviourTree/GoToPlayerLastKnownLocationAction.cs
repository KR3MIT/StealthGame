using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Go to Player last known location", story: "Move [Self] to Player [LastKnownPos]", category: "Action", id: "5e44fecf640cdca8199ab6b5150b2302")]
public partial class GoToPlayerLastKnownLocationAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Vector3> LastKnownPos;
    private NavMeshAgent nav;

    protected override Status OnStart()
    {
        Initialize();

        nav.stoppingDistance = 0.1f;
        nav.SetDestination(LastKnownPos.Value);

        return Status.Running;
    }

    private void Initialize()
    {
        nav = Self.Value.GetComponent<NavMeshAgent>();
    }

    protected override Status OnUpdate()
    {
        var distanceToTarget = Vector3.Distance(Self.Value.transform.position, LastKnownPos.Value);

        if (distanceToTarget < 0.1f)
        {
            nav.ResetPath();
            return Status.Success;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

