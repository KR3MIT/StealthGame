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

    public void SetState(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenu:
                Cursor.lockState = CursorLockMode.None;
                break;
            case GameState.Playing:
                Cursor.lockState = CursorLockMode.Locked;
                break;
        }
    }

    public void SetAlertness(Alertness alertness)
    {
        this.alertness = alertness;
    }
}
