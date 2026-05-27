using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using static GameManager;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SyncAlertness", story: "Sync [AlertnessEnum] with GameManager", category: "Action", id: "7ac721c94a0d0accd0ad688474658ca2")]
public partial class SyncAlertnessAction : Action
{
    [SerializeReference] public BlackboardVariable<AlertnessEnum> AlertnessEnum;

    protected override Status OnStart()
    {
        AlertnessEnum.Value = (AlertnessEnum)(int)GameManager.instance.alertness;
        return Status.Success;
    }
}

