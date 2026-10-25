using UnityEngine;

public enum GameState
{
    None,
    Lobby,
    BallSelect,
    Showdown
}

public class GameManager : MonoBehaviour
{
    public GameState gameState = GameState.None;

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeGameState(GameState newGameState)
    {
        gameState = newGameState;

        HandleGameStateChange();

        if (NetworkManager.Instance.isHost)
        {
            // broadcast gamestate change
        }
    }

    public void HandleGameStateChange()
    {
        UIManager ui = UIManager.Instance;

        switch (gameState)
        {
            case GameState.None:
                break;
        }
    }
}
