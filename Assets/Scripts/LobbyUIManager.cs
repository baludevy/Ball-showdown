using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    public GameObject lobbyPlayerPrefab;
    public Transform lobbyPlayersParent;

    public void UpdateLobbyUI()
    {
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
}
