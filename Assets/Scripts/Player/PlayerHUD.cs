using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    private Canvas canvas;
    private TMP_Text visText;
    private TMP_Text countdownText;
    private TMP_Text healthText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        canvas = GetComponentInChildren<Canvas>();
        visText = canvas.transform.GetChild(0).GetComponent<TMP_Text>();
        countdownText = canvas.transform.GetChild(1).GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        SetText();
    }

    private void SetText()
    {
        visText.text = (PlayerController.instance.visibility * 100).ToString("F0") + "%";
        countdownText.text = GameManager.instance.currentCountDown > 0 ? "Alert!\n" + GameManager.instance.currentCountDown.ToString("F1") : "";
    }
}
