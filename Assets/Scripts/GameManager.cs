using System;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    None,
    Lobby,
    BallSelect,
    Showdown
}

public class Player
{
    public ushort id;
    public string username;
}

public class GameManager : MonoBehaviour
{
    public GameState gameState = GameState.None;

    public List<Player> players = new List<Player>();

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

    public void HandleGameStateChange()
    {
        UIManager ui = UIManager.Instance;

        switch (gameState)
        {
            case GameState.None:
                ui.SwitchToUI(UIState.MainMenu);
                break;
            case GameState.Lobby:
                ui.SwitchToUI(UIState.Lobby);
                break;
            case GameState.BallSelect:
            case GameState.Showdown:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void AddPlayer(Player player)
    {
        players.Add(player);
    }

    public void RemovePlayer(ushort id)
    {
        players.Remove(GetPlayer(id));
    }

    public Player GetPlayer(ushort id) => players.Find(x => x.id == id);
}
