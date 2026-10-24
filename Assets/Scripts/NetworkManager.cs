using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public Client Client;
    public Server Server;

    [SerializeField] private ConnectionUIManager connectionUIManager;

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
        
        Join("localhost");
    }

    public void Join(string ip = "")
    {
        if (ip == "")
            ip = connectionUIManager.GetIP();
        
        Client = new Client();
        Client.Connect(ip);
    }

    private void Clear()
    {
        Client.Disconnect();
        Client = null;
        
        Server.Stop();
        Server = null;
    }

    private void OnApplicationQuit()
    {
        Clear();
    }
}
