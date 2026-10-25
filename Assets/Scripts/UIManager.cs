using System;
using UnityEngine;

[Serializable]
public enum UIState
{
    MainMenu,
    HostGame,
    JoinGame,
    Lobby,
    BallSelection,
    Waiting,
    Arena,
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameObject mainMenuUI;

    [SerializeField] private GameObject connectionUI;
    [SerializeField] private GameObject hostUI;
    [SerializeField] private GameObject joinUI;
    [SerializeField] private GameObject waitingUI;

    public LobbyUIManager lobbyUI;
    public BallSelectionUIManager ballSelectionUI;

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

    public void SwitchToUI(UIState state)
    {
        switch (state)
        {
            case UIState.MainMenu:
                mainMenuUI.SetActive(true);

                lobbyUI.gameObject.SetActive(false);
                connectionUI.SetActive(false);
                ballSelectionUI.gameObject.SetActive(false);
                waitingUI.SetActive(false);
                break;
            case UIState.HostGame:
                connectionUI.SetActive(true);
                hostUI.SetActive(true);
                joinUI.SetActive(false);

                mainMenuUI.SetActive(false);
                lobbyUI.gameObject.SetActive(false);
                ballSelectionUI.gameObject.SetActive(false);
                waitingUI.SetActive(false);
                break;
            case UIState.JoinGame:
                connectionUI.SetActive(true);
                hostUI.SetActive(false);
                joinUI.SetActive(true);

                mainMenuUI.SetActive(false);
                lobbyUI.gameObject.SetActive(false);
                ballSelectionUI.gameObject.SetActive(false);
                waitingUI.SetActive(false);
                break;
            case UIState.Lobby:
                lobbyUI.gameObject.SetActive(true);

                mainMenuUI.SetActive(false);
                connectionUI.SetActive(false);
                ballSelectionUI.gameObject.SetActive(false);
                waitingUI.SetActive(false);
                break;
            case UIState.BallSelection:
                ballSelectionUI.gameObject.SetActive(true);
                
                lobbyUI.gameObject.SetActive(false);
                mainMenuUI.SetActive(false);
                connectionUI.SetActive(false);
                waitingUI.SetActive(false);
                break;
            case UIState.Waiting:
                waitingUI.SetActive(true);
                
                ballSelectionUI.gameObject.SetActive(false);
                lobbyUI.gameObject.SetActive(false);
                mainMenuUI.SetActive(false);
                connectionUI.SetActive(false);
                break;
            case UIState.Arena:
                
                waitingUI.SetActive(false);
                ballSelectionUI.gameObject.SetActive(false);
                lobbyUI.gameObject.SetActive(false);
                mainMenuUI.SetActive(false);
                connectionUI.SetActive(false);
                break;
        }
    }

    public void ShowMainMenu()
    {
        SwitchToUI(UIState.MainMenu);
    }

    public void ShowHostGame()
    {
        SwitchToUI(UIState.HostGame);
    }

    public void ShowJoinGame()
    {
        SwitchToUI(UIState.JoinGame);
    }

    public void ShowLobby()
    {
        SwitchToUI(UIState.Lobby);
    }
}
