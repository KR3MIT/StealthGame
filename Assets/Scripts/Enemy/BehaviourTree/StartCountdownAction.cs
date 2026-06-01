using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "StartCountdown", story: "Start Countdown [CurrentAlertState]", category: "Action", id: "fe86c25b9736151e962b3888f7de7520")]
public partial class StartCountdownAction : Action
{
    //made with the help of claude.ai
    [SerializeReference] public BlackboardVariable<AlertState> CurrentAlertState;
    private bool _countdownComplete;
    private EnemyVision _vision;

    protected override Status OnStart()
    {
        _countdownComplete = false;
        _vision = GameObject.GetComponent<EnemyVision>();
        GameManager.instance.OnCountDownEnded += OnCountdownEnded;
        GameManager.instance.StartCountdown();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // player spotted again, reset timer
        if (_vision.inSight)
        {
            GameManager.instance.StartCountdown(); // restarts from full
            return Status.Running;
        }

        if (_countdownComplete)
        {
            CurrentAlertState.Value = AlertState.Passive;
            return Status.Success;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        GameManager.instance.OnCountDownEnded -= OnCountdownEnded;
    }

    private void OnCountdownEnded()
    {
        _countdownComplete = true;
    }
}