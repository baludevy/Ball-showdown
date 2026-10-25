using System;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    None,
    Lobby,
    BallSelect,
    DirectionChoose,
    Showdown
}

public class Player
{
    public ushort id;
    public string username;

    public BallEntry selectedBall;
    public GameObject ball;
    
    public Vector2 direction;
}

public class GameManager : MonoBehaviour
{
    public GameState gameState = GameState.None;

    [NonSerialized] public List<Player> players = new List<Player>();

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

    public void ChangeGameState(GameState newGameState, bool broadcast = true)
    {
        gameState = newGameState;

        HandleGameStateChange();

        if (NetworkManager.Instance.isHost && broadcast)
        {
            ServerSend.CurrentGameState(gameState);
        }
    }

    private int locksNeeded;
    private readonly HashSet<ushort> lockedPlayers = new HashSet<ushort>();

    public void LockDirection(Vector2 direction, ushort playerId)
    {
        if (gameState != GameState.DirectionChoose ||
            direction.sqrMagnitude < 0.0001f ||
            !lockedPlayers.Add(playerId))
            return;

        Ball ball = BallManager.Instance.GetBall(playerId);

        if (ball == null)
        {
            lockedPlayers.Remove(playerId);
            return;
        }

        ball.direction = direction.normalized;
        locksNeeded--;

        if (locksNeeded == 0)
        {
            ChangeGameState(GameState.Showdown);
        }
    }

    public void HandleGameStateChange()
    {
        UIManager ui = UIManager.Instance;

        if(gameState != GameState.Showdown && gameState != GameState.DirectionChoose)
            BallManager.Instance.Clear();
        
        switch (gameState)
        {
            case GameState.None:
                BallArena.Instance.ClearWalls();
                ui.SwitchToUI(UIState.MainMenu);
                break;
            case GameState.Lobby:
                BallManager.Instance.Clear();
                BallArena.Instance.ClearWalls();
                
                ui.SwitchToUI(UIState.Lobby);
                break;
            case GameState.BallSelect:
                ui.SwitchToUI(UIState.BallSelection);

                if (NetworkManager.Instance.isHost)
                {
                    BallSelectionManager.Instance.ResetSelections();
                    BallSelectionManager.Instance.needsSelection = players.Count;
                    BallSelectionManager.Instance.SendSelectableBalls();
                }
                break; 
            case GameState.DirectionChoose:
                BallArena.Instance.SetupArena(BallManager.Instance.spawnedBalls);
                
                locksNeeded = players.Count;
                lockedPlayers.Clear();
                ui.SwitchToUI(UIState.DirectionChoose);
                
                break;
            case GameState.Showdown:
                Debug.Log("ball mothafucking showdown");
                
                ui.SwitchToUI(UIState.Arena);
                foreach (Ball ball in BallManager.Instance.spawnedBalls)
                {
                    ball.move = true;
                    ball.arrow.gameObject.SetActive(false);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void AddPlayer(Player player)
    {
        players.Add(player);
        UIManager.Instance.lobbyUI.UpdateLobbyUI();
    }

    public void RemovePlayer(ushort id)
    {
        Player player = GetPlayer(id);

        if (player == null)
            return;

        bool wasAwaitingDirection = NetworkManager.Instance.isHost &&
            gameState == GameState.DirectionChoose &&
            !lockedPlayers.Contains(id);

        players.Remove(player);
        BallManager.Instance.DestroyBall(id);

        if (wasAwaitingDirection)
        {
            locksNeeded = Mathf.Max(0, locksNeeded - 1);

            if (locksNeeded == 0)
                ChangeGameState(GameState.Showdown);
        }

        UIManager.Instance.lobbyUI.UpdateLobbyUI();
    }

    public Player GetPlayer(ushort id) => players.Find(x => x.id == id);
}
