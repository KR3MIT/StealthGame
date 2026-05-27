using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
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
        this.alertness = _alertness;
        SetAlertnessVisibility(_alertness);
    }

    public void SetAlertnessVisibility(Alertness _alertness)
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
}
