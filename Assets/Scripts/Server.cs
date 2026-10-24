using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public class Server
{
    private EventBasedNetListener listener;
    private NetManager server;

    public Server()
    {
        listener = new EventBasedNetListener();
        server = new NetManager(listener);
    }

    public void Start(int port = 6900)
    {
        Debug.Log($"Starting server on {port}...");
        
        server.Start(port);
        AssignEvents();
    }

    public void Stop()
    {
        server.Stop();
        
        Debug.Log("Server stopped");
    }

    public void Update()
    {
        server.PollEvents();
    }
    
    private void AssignEvents()
    {
        listener.ConnectionRequestEvent += OnConnectionRequest;
        listener.PeerConnectedEvent += OnPeerConnectedEvent;
    }

    private void OnConnectionRequest(ConnectionRequest request)
    {
        if (server.ConnectedPeersCount < NetworkSettings.MaxPlayers)
            request.AcceptIfKey(NetworkSettings.ConnectionKey);
        else
            request.Reject();
    }

    private void OnPeerConnectedEvent(NetPeer peer)
    {
        Debug.Log($"Incoming connection from {peer.Address}");
        
        NetDataWriter writer = new NetDataWriter();
        writer.Put("csa teso");
        
        peer.Send(writer, DeliveryMethod.ReliableOrdered);
    }
}