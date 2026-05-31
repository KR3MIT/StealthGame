using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Set LastKnownLocation", story: "Set [Player] [LastKnownLocation]", category: "Action", id: "a0be08ae18a216bbda6babbbc0c1d966")]
public partial class SetLastKnownLocationAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<Vector3> LastKnownLocation;

    protected override Status OnStart()
    {
        LastKnownLocation.Value = Player.Value.transform.position;
        return Status.Success;
    }
}

