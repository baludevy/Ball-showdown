using System.Collections.Generic;
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

    public static void SelectableBalls(NetDataReader reader)
    {
        int ballCount = reader.GetInt();

        List<BallType> ballTypes = new List<BallType>();

        for (int i = 0; i < ballCount; i++)
        {
            BallType ball = (BallType)reader.GetByte();
            ballTypes.Add(ball);
        }
        
        BallSelectionManager.Instance.ownSelectableBalls.Clear();
        foreach (BallType ballType in ballTypes)
        {
            BallEntry ball = BallCatalog.Instance.GetBall(ballType);
            BallSelectionManager.Instance.ownSelectableBalls.Add(ball);
        }
        
        UIManager.Instance.ballSelectionUI.DisplayBallCards();
    }

    public static void SpawnBall(NetDataReader reader)
    {
        ushort ownerId = reader.GetUShort();
        BallType ballType = (BallType)reader.GetByte();
        
        BallManager.Instance.SpawnBall(ownerId, ballType);
    }

    public static void BallPosition(NetDataReader reader)
    {
        int snapshotCount = reader.GetInt();
        uint tick = reader.GetUInt();
        
        List<BallUpdate> updates = new List<BallUpdate>();

        for (int i = 0; i < snapshotCount; i++)
        {
            updates.Add(new BallUpdate
            {
                id = reader.GetUShort(),
                position = reader.GetVector3(),
            });
        }
        
        BallManager.Instance.ApplyUpdates(tick, updates);
    }
}