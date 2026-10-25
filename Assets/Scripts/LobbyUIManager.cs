using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    public GameObject lobbyPlayerPrefab;
    public Transform lobbyPlayersParent;

    public GameObject startButton;

    public void UpdateLobbyUI()
    {
        startButton.gameObject.SetActive(NetworkManager.Instance.isHost);

        for (int i = 0; i < lobbyPlayersParent.childCount; i++)
        {
            Destroy(lobbyPlayersParent.GetChild(i).gameObject);
        }

        foreach (Player player in GameManager.Instance.players)
        {
            LobbyPlayerUIElement lobbyPlayerUIElement = Instantiate(lobbyPlayerPrefab, lobbyPlayersParent).GetComponent<LobbyPlayerUIElement>();

            lobbyPlayerUIElement.SetUsername(player.username);
        }
    }

    public void StartGame()
    {
        GameManager.Instance.ChangeGameState(GameState.BallSelect);
    }
}
