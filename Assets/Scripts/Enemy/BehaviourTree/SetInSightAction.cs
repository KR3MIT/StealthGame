using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetInSight", story: "Set [InSight]", category: "Action", id: "1c9ae64e210540c6df831bb174ff5ff9")]
public partial class SetInSightAction : Action
{
    [SerializeReference] public BlackboardVariable<bool> InSight;
    private EnemyVision vision;
    protected override Status OnStart()
    {
        vision = GameObject.GetComponent<EnemyVision>();

        InSight.Value = vision.inSight;
        return Status.Success;
    }
}

