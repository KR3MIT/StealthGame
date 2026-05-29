using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetTransform", story: "Set [firstTransform] to [secondTransform]", category: "Action", id: "21d894c3bd2fc6a60e0df16164fd80bd")]
public partial class SetTransformAction : Action
{
    [SerializeReference] public BlackboardVariable<Vector3> FirstTransform;
    [SerializeReference] public BlackboardVariable<Transform> SecondTransform;
    protected override Status OnStart()
    {
        FirstTransform.Value = SecondTransform.Value.position;
        return Status.Success;
    }
}

