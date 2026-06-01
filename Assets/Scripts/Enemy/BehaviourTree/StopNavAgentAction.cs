using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Stop NavAgent", story: "Stop NavAgent", category: "Action", id: "9a152337bb4de3c6768558d3033ec8c2")]
public partial class StopNavAgentAction : Action
{
    protected override Status OnStart()
    {
        var agent = GameObject.GetComponent<NavMeshAgent>();
        agent.isStopped = true;
        agent.ResetPath();
        agent.velocity = Vector3.zero;
        return Status.Success;
    }
}