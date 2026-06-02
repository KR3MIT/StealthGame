using TMPro;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAnimations : MonoBehaviour
{
    //made with the help of claude.ai

    private Canvas canvas;
    private Material gizmoMat;
    private TMP_Text alertnessText;

    private BehaviorGraphAgent _agent;
    private Animator animator;
    private EnemyAlertness alert;
    private NavMeshAgent agent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        alert = GetComponent<EnemyAlertness>();
        _agent = GetComponent<BehaviorGraphAgent>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        canvas = transform.GetChild(1).GetComponent<Canvas>();
        Image gizmoImage = canvas.transform.GetChild(0).GetComponent<Image>();
        gizmoMat = new Material(gizmoImage.material); // unique copy
        gizmoImage.material = gizmoMat;               // assign back
        alertnessText = canvas.transform.GetChild(1).GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 player = PlayerController.instance.transform.position;

        canvas.transform.LookAt(player);
        gizmoMat.SetFloat("_Procentage", alert.alertness);

        if (alert.alertness > 0.001f)
        {
            alertnessText.text = alert.alertness.ToString("F2") + "%";
            alertnessText.gameObject.SetActive(true);
        }
        else
            alertnessText.gameObject.SetActive(false);

        AlertStateSync();
    }

    private void AlertStateSync()
    {
        _agent.BlackboardReference.GetVariableValue("AlertState", out AlertState state);
        
        animator.SetBool("isPassive", state == AlertState.Passive);
        animator.SetBool("isAlert", state == AlertState.Alert);
        animator.SetBool("isGoTo", state == AlertState.GoTo);
        animator.SetBool("isAggressive", state == AlertState.Aggressive);

        animator.SetFloat("Alertness", alert.alertness);

        animator.SetFloat("Speed", agent.velocity.magnitude / agent.speed);
    }
}
