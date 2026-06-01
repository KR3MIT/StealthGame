using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Player InSight", story: "[Player] is in [Vision]", category: "Action", id: "62070e9500d5718f15af119a7d6507c9")]
public partial class PlayerInSightAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<EnemyVision> Vision;
    private EnemyVision vision;
    protected override Status OnStart()
    {
        vision = GameObject.GetComponent<EnemyVision>();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if(vision.inSight)
            return Status.Success;
        else
            return Status.Failure;
    }
}