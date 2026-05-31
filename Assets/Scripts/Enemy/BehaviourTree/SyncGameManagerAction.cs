using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using static GameManager;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SyncGameManager", story: "Sync [AlertState] with GameManager", category: "Action", id: "f1b84c8933b3275b59381c4f5b0d1cfd")]
public partial class SyncGameManagerAction : Action
{
    [SerializeReference] public BlackboardVariable<AlertState> AlertState;
    private GameManager gameManager;
    protected override Status OnStart()
    {
        gameManager = GameManager.instance;
        //AlertState.Value = (AlertState)(int)gameManager.alertness;
        gameManager.SetAlertness((Alertness)AlertState.Value);
        return Status.Success;
    }
}

