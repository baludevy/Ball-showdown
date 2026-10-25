using TMPro;
using UnityEngine;

public class LobbyPlayerUIElement : MonoBehaviour
{
    [SerializeField] private TMP_Text username;

    public void SetUsername(string username)
    {
        this.username.text = username;
    }
}