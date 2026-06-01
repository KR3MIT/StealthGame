using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Player InSight", story: "[Player] [InSight]", category: "Action", id: "62070e9500d5718f15af119a7d6507c9")]
public partial class PlayerInSightAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<EnemyVision> InSight;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if(InSight.Value.inSight)
            return Status.Success;
        else
            return Status.Failure;
    }

    protected override void OnEnd()
    {
    }
}

