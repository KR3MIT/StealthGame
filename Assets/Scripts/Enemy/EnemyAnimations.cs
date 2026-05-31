using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAnimations : MonoBehaviour
{
    private EnemyAlertness alertness;

    private Canvas canvas;
    private Material gizmoMat;
    private TMP_Text alertnessText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        alertness = GetComponent<EnemyAlertness>();

        canvas = transform.GetChild(1).GetComponent<Canvas>();
        gizmoMat = canvas.transform.GetChild(0).GetComponent<Image>().material;
        alertnessText = canvas.transform.GetChild(1).GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 player = PlayerController.instance.transform.position;
        Vector3 target = new Vector3(player.x, transform.position.y, player.z);

        canvas.transform.LookAt(target);
        gizmoMat.SetFloat("_Procentage", alertness.alertness);

        if (alertness.alertness > 0.001f)
        {
            alertnessText.text = alertness.alertness.ToString("F2") + "%";
            alertnessText.gameObject.SetActive(true);
        }
        else
            alertnessText.gameObject.SetActive(false);
    }
}
