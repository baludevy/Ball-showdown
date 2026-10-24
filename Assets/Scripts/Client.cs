using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public class Client
{
    private EventBasedNetListener listener;
    private NetManager client;

    public Client()
    {
        listener = new EventBasedNetListener();
        client = new NetManager(listener);
    }

    public void Connect(string ip, int port = 6900)
    {
        client.Start();
        client.Connect(ip, port, NetworkSettings.ConnectionKey);

        AssignEvents();
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
        listener.NetworkReceiveEvent += OnNetworkReceive;
    }

    private void OnNetworkReceive(NetPeer from, NetDataReader reader, byte channel, DeliveryMethod deliveryMethod)
    {
        Debug.Log($"We got: {reader.GetString()}, from: {from.Address}");
    }
}
