using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MinusOneAlertness", story: "if Alertness < [Threshold]", category: "Action", id: "bafeb6dfb228d58f05190385f55778da")]
public partial class MinusOneAlertnessAction : Action
{
    //made with the help of claude.ai
    [SerializeReference] public BlackboardVariable<float> Threshold;
    private EnemyAlertness alert;

    protected override Status OnStart()
    {
        alert = GameObject.GetComponent<EnemyAlertness>();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (alert.alertness <= Threshold.Value)
            return Status.Success;
        else
            return Status.Failure;
    }
}