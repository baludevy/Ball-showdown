using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public Client Client;
    public Server Server;

    [SerializeField] private ConnectionUIManager connectionUIManager;

    public bool isHost;
    
    public static NetworkManager Instance;

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

    private void FixedUpdate()
    {
        Server?.Update();
        Client?.Update();
    }

    public void Host()
    {
        Server = new Server();
        Server.Start();

        isHost = true;
        
        GameManager.Instance.AddPlayer(new Player
        {
            id = 0,
            username = connectionUIManager.GetName()
        });
        
        GameManager.Instance.ChangeGameState(GameState.Lobby, broadcast: false);
    }

    public void Join(string ip = "")
    {
        if (ip == "")
            ip = connectionUIManager.GetIP();

        Client = new Client();
        Client.Connect(ip);
    }

    public void SendUsername()
    {
        ClientSend.Username(connectionUIManager.GetName());
    }

    public void Clear()
    {
        if (Client != null)
        {
            Client.Disconnect();
            Client = null;
        }

        if (Server != null)
        {
            Server.Stop();
            Server = null;
        }

        isHost = false;
        
        GameManager.Instance.ChangeGameState(GameState.None);
    }

    private void OnApplicationQuit()
    {
        Clear();
    }
}
