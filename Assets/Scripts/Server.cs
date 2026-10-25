using System;
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

    private static void RegisterPackets()
    {
        PacketRouter.RegisterServerPacket((ushort)ClientPacket.Username, ServerHandle.Username);
    }

    public void Start(int port = 6900)
    {
        Debug.Log($"Starting server on {port}...");

        server.Start(port);
        AssignEvents();
        RegisterPackets();
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
        listener.PeerDisconnectedEvent += OnPeerDisconnectedEvent;
        listener.NetworkReceiveEvent += OnNetworkReceive;
    }

    private void OnConnectionRequest(ConnectionRequest request)
    {
        if (server.ConnectedPeersCount < NetworkSettings.MaxPlayers)
            request.AcceptIfKey(NetworkSettings.ConnectionKey);
        else
            request.Reject();
    }

    private static void OnPeerConnectedEvent(NetPeer peer)
    {
        Debug.Log($"Incoming connection from {peer.Address}");
        
        foreach (Player player in GameManager.Instance.players)
        {
            ServerSend.PlayerConnected(peer.Id, player);
        }
        
        ServerSend.CurrentGameState(peer.Id, GameManager.Instance.gameState);
    } 
    
    private static void OnPeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log($"Player {peer.Id} disconnected, reason: {disconnectInfo.Reason}");
        
        GameManager.Instance.RemovePlayer((ushort)peer.Id);
        
        ServerSend.PlayerDisconnected((ushort)peer.Id);
    }

    private static void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
    {
        PacketRouter.ProcessServerMessage(peer, reader);
    }

    public void SendPacketToAll(ServerPacket packetId, Action<NetDataWriter> writeData = null, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
    {
        NetDataWriter writer = new NetDataWriter();

        writer.Put((ushort)packetId);
        writeData?.Invoke(writer);

        server.SendToAll(writer, deliveryMethod);
    }

    public void SendPacketToAllExcept(ServerPacket packetId, int peerIdExcept, Action<NetDataWriter> writeData = null, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
    {
        NetDataWriter writer = new NetDataWriter();

        writer.Put((ushort)packetId);
        writeData?.Invoke(writer);

        NetPeer excludedPeer = null;

        foreach (NetPeer peer in server)
        {
            if (peer.Id == peerIdExcept)
            {
                excludedPeer = peer;
                break;
            }
        }

        server.SendToAll(writer, deliveryMethod, excludedPeer);
    }

    public void SendPacketTo(ServerPacket packetId, int peerId, Action<NetDataWriter> writeData = null, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
    {
        NetDataWriter writer = new NetDataWriter();

        writer.Put((ushort)packetId);
        writeData?.Invoke(writer);

        foreach (NetPeer peer in server)
        {
            if (peer.Id == peerId)
            {
                peer.Send(writer, deliveryMethod);
                break;
            }
        }
    }
}
