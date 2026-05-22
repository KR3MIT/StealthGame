using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        MainMenu,
        Playing
    }

    [HideInInspector]
    public GameState gameState;

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
