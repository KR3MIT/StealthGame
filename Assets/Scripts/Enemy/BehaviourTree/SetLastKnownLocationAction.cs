using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Set last known location", story: "Set [LastKnownLocation]", category: "Action", id: "13ab14b123b7a54910877477f0300367")]
public partial class SetLastKnownLocationAction : Action
{
    [SerializeReference] public BlackboardVariable<Vector3> LastKnownLocation;
    [SerializeReference] public BlackboardVariable<EnemyVision> Vision;

    protected override Status OnStart()
    {
        if(Vision.Value.inSight)
            LastKnownLocation.Value = PlayerController.instance.transform.position;

        return Status.Success;
    }
}

