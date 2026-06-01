using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Set start position", story: "Set [startpos] [startrot] to initial [self]", category: "Action", id: "c25aec75f4e5902ffcf4c09757b57dce")]
public partial class SetStartPositionAction : Action
{
    [SerializeReference] public BlackboardVariable<Vector3> Startpos;
    [SerializeReference] public BlackboardVariable<Vector3> Startrot;
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    protected override Status OnStart()
    {
        Startpos.Value = GameObject.transform.position;
        Startrot.Value = GameObject.transform.rotation.eulerAngles;
        return Status.Success;
    }
}

