using System;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public class Client
{
    private EventBasedNetListener listener;
    private NetManager client;

    private NetPeer serverPeer;

    public Client()
    {
        listener = new EventBasedNetListener();
        client = new NetManager(listener);
    }

    private static void RegisterPackets()
    {
        PacketRouter.RegisterClientPacket((ushort)ServerPacket.OwnID, ClientHandle.OwnID);
        PacketRouter.RegisterClientPacket((ushort)ServerPacket.CurrentGameState, ClientHandle.CurrentGameState);
        PacketRouter.RegisterClientPacket((ushort)ServerPacket.PlayerConnected, ClientHandle.PlayerConnected);
        PacketRouter.RegisterClientPacket((ushort)ServerPacket.PlayerDisconnected, ClientHandle.PlayerDisconnected);
        PacketRouter.RegisterClientPacket((ushort)ServerPacket.SelectableBalls, ClientHandle.SelectableBalls);
        PacketRouter.RegisterClientPacket((ushort)ServerPacket.SpawnBall, ClientHandle.SpawnBall);
        PacketRouter.RegisterClientPacket((ushort)ServerPacket.BallPosition, ClientHandle.BallPosition);
        PacketRouter.RegisterClientPacket((ushort)ServerPacket.BallHealth, ClientHandle.BallHealth);
        PacketRouter.RegisterClientPacket((ushort)ServerPacket.DestroyBall, ClientHandle.DestroyBall);
        PacketRouter.RegisterClientPacket((ushort)ServerPacket.DamageMarker, ClientHandle.DamageMarker);
        PacketRouter.RegisterClientPacket((ushort)ServerPacket.PlayerWon, ClientHandle.PlayerWon);
        PacketRouter.RegisterClientPacket((ushort)ServerPacket.VerityUpgrade, ClientHandle.VerityUpgrade);
    }

    public void Connect(string ip, int port = 6900)
    {
        AssignEvents();
        RegisterPackets();

        client.Start();
        client.Connect(ip, port, NetworkSettings.ConnectionKey);
    }

    public void Disconnect()
    {
        client.DisconnectAll();
        client.Stop();
    }

    public void Update()
    {
        client.PollEvents();
    }

    private void AssignEvents()
    {
        listener.PeerConnectedEvent += OnClientConnected;
        listener.PeerDisconnectedEvent += OnClientDisconnected;
        listener.NetworkReceiveEvent += OnNetworkReceive;
    }

    private void OnClientConnected(NetPeer peer)
    {
        serverPeer = peer;
        
        NetworkManager.Instance.SendUsername();
    }

    private void OnClientDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log($"Disconnected: {disconnectInfo.Reason}");
        NetworkManager.Instance.Clear();
    }

    private void OnNetworkReceive(NetPeer from, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
    {
        PacketRouter.ProcessClientMessage(reader);
    }

    public void SendPacket(ClientPacket packetId, Action<NetDataWriter> writeData = null, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
    {
        NetDataWriter writer = new NetDataWriter();

        writer.Put((ushort)packetId);
        writeData?.Invoke(writer);

        if (serverPeer != null)
            serverPeer.Send(writer, deliveryMethod);
    }
}
