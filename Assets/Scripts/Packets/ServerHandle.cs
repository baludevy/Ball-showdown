using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public static class ServerHandle
{
    public static void Username(NetPeer from, NetDataReader reader)
    {
        string username = reader.GetString();

        Player player = new Player
        {
            id = (ushort)from.Id,
            username = username
        };

        GameManager.Instance.AddPlayer(player);

        Debug.Log($"Player {username}, id: {from.Id} connected");

        ServerSend.PlayerConnected(player);
    }

    public static void SelectBall(NetPeer from, NetDataReader reader)
    {
        int index = reader.GetByte();
        
        BallSelectionManager.Instance.HandleBallSelection((ushort)from.Id, index);
    }
}
