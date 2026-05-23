using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance;
    public enum GameState
    {
        MainMenu,
        Playing
    }

    [HideInInspector]
    public GameState gameState;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        SetState(GameState.Playing);
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
}
