using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using static GameManager;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetAlertness", story: "Set [AlertnessEnum] on GameManager", category: "Action", id: "cdd663f1f831e03078fc05f0240f1743")]
public partial class SetAlertnessAction : Action
{
    [SerializeReference] public BlackboardVariable<AlertnessEnum> AlertnessEnum;

    protected override Status OnStart()
    {
        GameManager.instance.SetAlertness((Alertness)(int)AlertnessEnum.Value);
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

