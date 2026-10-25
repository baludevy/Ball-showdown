using LiteNetLib.Utils;
using UnityEngine;

public static class ClientHandle
{
    public static void CurrentGameState(NetDataReader reader)
    {
        GameState gameState = (GameState)reader.GetUShort();
        
        GameManager.Instance.ChangeGameState(gameState);
    }

    public static void PlayerConnected(NetDataReader reader)
    {
        ushort id = reader.GetUShort();
        string username = reader.GetString();
        
        GameManager.Instance.AddPlayer(new Player
        {
            id = id,
            username = username
        });
        
        Debug.Log($"Player {username}, id: {id} connected");
    }

    public static void PlayerDisconnected(NetDataReader reader)
    {
        ushort id = reader.GetUShort();
        
        GameManager.Instance.RemovePlayer(id);
        Debug.Log($"Player {id} disconnected");
    }
}