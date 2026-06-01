using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AlertOthers", story: "Alert Others", category: "Action", id: "6c0e0d7a5f4b435d860262d1efb02d52")]
public partial class AlertOthersAction : Action
{
    //made with the help of claude.ai
    [SerializeReference] public BlackboardVariable<float> Delay;
    private EnemyController _controller;
    private float _elapsed;

    protected override Status OnStart()
    {
        _controller = GameObject.GetComponent<EnemyController>();
        _controller.AlertOthers();
        _elapsed = 0f;
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        _elapsed += Time.deltaTime;
        if (_elapsed >= Delay.Value)
            return Status.Success;

        return Status.Running;
    }
}