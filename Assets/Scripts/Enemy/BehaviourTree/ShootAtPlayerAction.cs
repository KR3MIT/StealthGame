using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ShootAtPlayer", story: "Shoot At [Player]", category: "Action", id: "624e54f2ae493e27e3825499cf77f985")]
public partial class ShootAtPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

