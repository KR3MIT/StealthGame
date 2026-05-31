using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CheckAlertness", story: "If [Alertness] > [AlertThreshold]", category: "Action", id: "f5b30e3561d1ced7c4c69fbe270bcabb")]
public partial class CheckAlertnessAction : Action
{
    [SerializeReference] public BlackboardVariable<EnemyAlertness> Alertness;
    [SerializeReference] public BlackboardVariable<float> AlertThreshold;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Alertness.Value.alertness >= AlertThreshold.Value)
            return Status.Success;
        else
            return Status.Failure;
    }
    protected override void OnEnd()
    {
    }
}

