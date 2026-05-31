using Unity.VisualScripting;
using UnityEngine;

public class EnemyAlertness : MonoBehaviour
{
    public float alertness { get; private set; }
    public float alertnessRate = 0.01f;
    public float alertnessLossDelay = 1f;
    public float alertnessLossRate = 0.5f;
    private float falloff = 2f; //falloff = 1 → linear, falloff = 2 → slow at distance, falloff = 0.5 → aggressive at distance.

    private float maxAlertness = 1f;
    private float alertnessModifier;
    private float sightLostTimer = 0f;

    private EnemyVision vision;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        vision = GetComponent<EnemyVision>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleAlertness();
    }
    private void HandleAlertness()
    {
        alertnessModifier = PlayerController.instance.visibility + GameManager.instance.alertnessVisibility;

        if (vision.inSight)
        {
            sightLostTimer = 0f;

            float distanceFactor = Mathf.Pow(1f - Mathf.Clamp01(vision.distToTarget / vision.range), falloff);

            alertness += (alertnessRate + alertnessModifier) * distanceFactor * Time.deltaTime;
        }
        else
        {
            sightLostTimer += Time.deltaTime;
            if (sightLostTimer >= alertnessLossDelay)
                alertness -= (alertnessRate + alertnessLossRate) * Time.deltaTime;
        }

        alertness = Mathf.Clamp(alertness, 0, maxAlertness);
    }
}
