using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "VisibilityThreshold", story: "if [visibility] greater than [threshold]", category: "Action", id: "d07c3e4e92fa8a44f4ff79c9cd9d56af")]
public partial class VisibilityThresholdAction : Action
{
    [SerializeReference] public BlackboardVariable<EnemyVision> Visibility;
    [SerializeReference] public BlackboardVariable<float> Threshold;
    protected override Status OnStart()
    {
        if (Visibility.Value.alertness >= Threshold.Value)
            return Status.Success; 
        else
            return Status.Failure;
    }
}

