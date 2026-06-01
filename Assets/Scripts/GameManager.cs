using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public event System.Action OnCountDownEnded;

    [SerializeField] private float countDown;
    public float currentCountDown { get; private set; }

    private Coroutine _routine;
    public enum GameState
    {
        MainMenu,
        Playing
    }
    public GameState gameState;
    
    public enum Alertness
    {
        Passive,
        Alert,
        Aggressive
    }
    public Alertness alertness;

    public float passiveVisibility, alertVisibility, aggressiveVisibility;
    public float alertnessVisibility { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(this);
    }

    void Start()
    {
        SetState(GameState.Playing);
        SetAlertness(Alertness.Passive);
    }

    public void SetState(GameState _state)
    {
        switch (_state)
        {
            case GameState.MainMenu:
                Cursor.lockState = CursorLockMode.None;
                break;
            case GameState.Playing:
                Cursor.lockState = CursorLockMode.Locked;
                break;
        }
    }

    public void SetAlertness(Alertness _alertness)
    {
        alertness = _alertness;
        SetAlertnessVisibility(alertness);
    }

    private void SetAlertnessVisibility(Alertness _alertness)
    {
        switch (_alertness)
        {
            case Alertness.Passive:
                alertnessVisibility = passiveVisibility;
                break;
            case Alertness.Alert:
                alertnessVisibility = alertVisibility;
                break;
            case Alertness.Aggressive:
                alertnessVisibility = aggressiveVisibility;
                break;
            default:
                alertnessVisibility = 1f;
                break;
        }
    }

    public void StartCountdown()
    {
        if (_routine != null)
            StopCoroutine(_routine);

        _routine = StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        currentCountDown = countDown;

        while (currentCountDown > 0f)
        {
            currentCountDown -= Time.deltaTime;
            yield return null;
        }

        currentCountDown = 0f;
        _routine = null;
        OnCountDownEnded?.Invoke();
    }
}
