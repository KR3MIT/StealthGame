using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "InvestigatePosition", story: "Investigave [LastKnownPos]", category: "Action", id: "c15a5b95eb278b0e22f2673ddf67db5d")]
public partial class InvestigatePositionAction : Action
{
    [SerializeReference] public BlackboardVariable<Vector3> LastKnownPos;
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public EnemyVision vision;

    protected override Status OnStart()
    {
        Initialize();
        return Status.Running;
    }

    private void Initialize()
    {
        vision = Self.Value.GetComponent<EnemyVision>();
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

